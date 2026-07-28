using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using ExpenseControl.Api.Data;
using ExpenseControl.Api.Mapper;
using ExpenseControl.Api.Middleware;
using ExpenseControl.Api.Model.Entity;
using ExpenseControl.Api.Model.Repository;
using ExpenseControl.Api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

// -----------------------------------------------------------------------------
// Composition root da API ExpenseControl: registra serviços de DI, configura
// autenticação JWT, Swagger/OpenAPI, o pipeline de middlewares HTTP, aplica
// migrations automaticamente e garante um usuário administrativo padrão no
// primeiro start (seed), para permitir login imediato em ambiente de dev.
// -----------------------------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

// ----- Serviços da aplicação -----
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPeopleService, PeopleService>();
builder.Services.AddScoped<PeopleMapper>();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<TransactionMapper>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserMapper>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// ----- Swagger / OpenAPI (equivalente ao springdoc-openapi) -----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "ExpenseControl API",
        Version = "v1",
        Description = "API para controle de gastos domésticos, com gestão de pessoas e transações."
    });

    options.EnableAnnotations();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // ----- Suporte a JWT no Swagger (botão "Authorize") -----
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "Insira apenas o token JWT (sem o prefixo 'Bearer '). O Swagger adiciona o prefixo automaticamente."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>(Array.Empty<string>())
    });
});

var app = builder.Build();

// ----- Migrations automáticas ao iniciar -----
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    // ----- Seed de usuário padrão (apenas se ainda não existir nenhum) -----
    if (!dbContext.Users.Any())
    {
        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();

        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var defaultUser = new User
        {
            Username = config["DefaultAdminUser:Username"]!,
            PasswordHash = authService.RegisterPassword(config["DefaultAdminUser:Password"]!),
            People = null
        };

        dbContext.Users.Add(defaultUser);
        dbContext.SaveChanges();
    }
}

// ----- Pipeline HTTP -----
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ExpenseControl API v1");
        options.RoutePrefix = string.Empty; // Swagger UI na raiz do site
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();