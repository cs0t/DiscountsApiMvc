using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
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
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

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

//add versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

//add mapster
builder.Services.AddMapster();
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
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