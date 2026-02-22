using System.Reflection;
using Discounts.API.Extensions.Swagger;
using Discounts.API.Middleware;
using Discounts.Infra.Extensions;
using Discounts.Application.Extensions;
using Discounts.Application.Validators.Offers;
using Discounts.Infra.Persistence;
using Discounts.Infra.Persistence.Seeding;
using Discounts.Infra.Security;
using Discounts.WorkerService.Extensions;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Discounts.API.Validation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationDbContext(builder.Configuration);
//builder.Services.AddMemoryCache();
builder.Services.AddInfrastructure();
builder.Services.AddHostedServices();
builder.Services.AddApplication();

//add jwt 
builder.Services.AddJwtAuthentication(builder.Configuration);

//add validation
builder.Services.AddValidatorsFromAssemblyContaining<CreateOfferCommandValidator>();
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.OverrideDefaultResultFactoryWith<CustomAutoValidationResultFactory>();
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.AddSwaggerExtensions());

//add mapster
builder.Services.AddMapster();
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//add global error handler mw
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

//run seeding
var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.Database.MigrateAsync();
var transaction  = await context.Database.BeginTransactionAsync();
try
{
    await SeedDataGenerator.SeedAsync(context: context);
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}


app.Run();