using System.Reflection;
using System.Text.Json.Serialization;
using ExpenseControl.Api.Data;
using ExpenseControl.Api.Mapper;
using ExpenseControl.Api.Middleware;
using ExpenseControl.Api.Model.Repository;
using ExpenseControl.Api.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ----- Serviços da aplicação -----
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPeopleService, PeopleService>();
builder.Services.AddScoped<PeopleMapper>();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<TransactionMapper>();

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

    // Habilita os atributos [SwaggerOperation], [SwaggerResponse] etc.
    // (equivalente aos @Operation/@ApiResponse do springdoc)
    options.EnableAnnotations();

    // Lê os comentários /// dos Controllers/DTOs e exibe no Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// ----- Migrations automáticas ao iniciar -----
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
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