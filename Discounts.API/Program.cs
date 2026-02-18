using System.Reflection;
using Discounts.API.Extensions.Swagger;
using Discounts.API.Middleware;
using Discounts.Infra.Extensions;
using Discounts.Application.Extensions;
using Discounts.Application.Validators.Offers;
using Discounts.Infra.Security;
using FluentValidation;
using Mapster;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationDbContext(builder.Configuration);
//builder.Services.AddMemoryCache();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();

//add jwt 
builder.Services.AddJwtAuthentication(builder.Configuration);

//add validation
builder.Services.AddValidatorsFromAssemblyContaining<CreateOfferCommandValidator>();

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

app.Run();