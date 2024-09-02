using System.Text;
using Bank.Loan.Application.Infrastructure;
using Bank.Loan.Application.Repository;
using Bank.Loan.Domain;
using Bank.Loan.Infrastructure.Messaging;
using Bank.Loan.Persistence;
using Bank.Loan.Persistence.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//TODO add logging for before app startup
builder.Configuration.AddJsonFile("appsecrets.json");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Bank.Loan.Application.Ref>());

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

//TODO: add db healthcheck

builder.Services.AddSingleton<IRabbitMqService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new RabbitMqService(configuration);
}); //TODO appsetingize it
builder.Services.AddSingleton<IPendingLoanService, PendingLoanService>();

builder.Services.AddScoped<IRepository<Loan>, EfCoreRepository<Loan>>();

builder.Services.AddValidatorsFromAssemblyContaining<Bank.Loan.Domain.Ref>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true, //TODO explore
        ValidAudience = builder.Configuration.GetSection("Jwt")["ValidAudience"], //TODO change to options pattern or other
        ValidIssuer = builder.Configuration.GetSection("Jwt")["ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt")["Secret"]!))
    };
}); 

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Seq(context.Configuration["Seq:ServerUrl"]));

var app = builder.Build();

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();