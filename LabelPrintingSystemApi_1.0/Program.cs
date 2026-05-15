using LabelPrintingSystemApi_1._0.MappingProfiles;
using LabelPrintingSystemApi_1._0.Middleware;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.Services;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using Scalar.AspNetCore;




var builder = WebApplication.CreateBuilder(args);

/// logi
/// 
builder.Logging.AddFilter("LuckyPennySoftware.AutoMapper.License", LogLevel.None);
//builder.Logging.ClearProviders();
builder.Host.UseNLog();


// Add services to the container.

/// Kontrolery 
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


// error handling 

builder.Services.AddScoped<ErrorHandlingMiddleware>();


///swager, scallar
builder.Services.AddOpenApi();



/// bazy danych 
builder.Services.AddDbContext<DatabaseContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});


/// serwisy 
builder.Services.AddScoped<IUserService, UserService>();

// AutoMapper
builder.Services.AddAutoMapper(config => { }, typeof(UserMappingProfile).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ErrorHandlingMiddleware>();

//// development 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
