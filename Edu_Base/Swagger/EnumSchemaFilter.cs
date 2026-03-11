using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;

namespace Edu_Base.Swagger
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (!type.IsEnum)
                return;

            // Make sure the schema is described as string
            schema.Type = "string";
            schema.Enum.Clear();

            var names = System.Enum.GetNames(type);
            foreach (var name in names)
            {
                schema.Enum.Add(new OpenApiString(name));
            }
        }
    }
}
