using ExpenseControl.Api.Data;
using ExpenseControl.Api.Mapper;
using ExpenseControl.Api.Middleware;
using ExpenseControl.Api.Model.Repository;
using ExpenseControl.Api.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("ExpenseControl.db"));

builder.Services.AddScoped<IPeopleService, PeopleService>();
builder.Services.AddScoped<PeopleMapper>();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<TransactionMapper>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();