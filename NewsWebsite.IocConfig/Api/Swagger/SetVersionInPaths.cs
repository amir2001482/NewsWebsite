using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NewsWebsite.IocConfig.Api.Swagger
{
    public class SetVersionInPaths : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var updatedPaths = new OpenApiPaths();

            foreach (var entry in swaggerDoc.Paths)
                updatedPaths.Add(entry.Key.Replace("v{version}", swaggerDoc.Info.Version), entry.Value);

            swaggerDoc.Paths = updatedPaths;
        }
    }

}
