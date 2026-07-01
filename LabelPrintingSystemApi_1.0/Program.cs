using LabelPrintingSystemApi_1._0.Middleware;
using LabelPrintingSystemApi_1._0.Models.Contexts;
using LabelPrintingSystemApi_1._0.OpenApi;
using LabelPrintingSystemApi_1._0.Services.Auth;
using LabelPrintingSystemApi_1._0.Services.Dispatchers;
using LabelPrintingSystemApi_1._0.Services.Interfaces;
using LabelPrintingSystemApi_1._0.Services.Kartoteki;
using LabelPrintingSystemApi_1._0.Services.Konfiguracja;
using LabelPrintingSystemApi_1._0.Services.PrintJobs;
using LabelPrintingSystemApi_1._0.Services.PrintLabel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using Scalar.AspNetCore;
using System.Text;




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


builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

///swager, scallar
builder.Services.AddOpenApi(options =>
{   //dodanie autoryzacji JWT do swager
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    // Poprawny opis typów int w Swagger UI.
    options.AddSchemaTransformer<IntegerSchemaTransformer>();
});



/// bazy danych 
builder.Services.AddDbContext<DatabaseContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddDbContext<IdentityContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// Identity
// add identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    string jwtKey = builder.Configuration["Jwt:Key"]!;
    string jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
    string jwtAudience = builder.Configuration["Jwt:Audience"]!;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();



/// serwisy 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IPrinterService, PrinterService>();
builder.Services.AddScoped<ILabelTemplateService, LabelTemplateService>();
builder.Services.AddScoped<IPrintLabelService, PrintLabelService>();
builder.Services.AddScoped<IPrintJobService, PrintJobService>();

builder.Services
    .AddHttpClient<IPrintJobDispatcher, NiceLabelPrintJobDispatcher>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(15);
    });


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ErrorHandlingMiddleware>();

//// development 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.MapScalarApiReference();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "LPS API v1");
    });
}

app.UseCors("ReactClient");

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
