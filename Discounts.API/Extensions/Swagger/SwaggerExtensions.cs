using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Discounts.API.Extensions.Swagger;

public static class SwaggerExtensions
{
    public static SwaggerGenOptions AddSwaggerExtensions(this SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1",new OpenApiInfo
        {
            Title = "DiscountsAPI",
            Version = "v1"
        });
        
        //add jwt
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        });
        
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

        return options;
    }
}