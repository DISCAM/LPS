using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace LabelPrintingSystemApi_1._0.OpenApi
{
    public sealed class IntegerSchemaTransformer
        : IOpenApiSchemaTransformer
    {
        public Task TransformAsync(
            OpenApiSchema schema,
            OpenApiSchemaTransformerContext context,
            CancellationToken cancellationToken
        )
        {
            Type type = context.JsonTypeInfo.Type;

            Type actualType = Nullable.GetUnderlyingType(type) ?? type;

            bool isIntegerType =
                actualType == typeof(int) ||
                actualType == typeof(long) ||
                actualType == typeof(short) ||
                actualType == typeof(byte) ||
                actualType == typeof(sbyte) ||
                actualType == typeof(uint) ||
                actualType == typeof(ulong) ||
                actualType == typeof(ushort);

            if (!isIntegerType)
            {
                return Task.CompletedTask;
            }

            schema.Type = JsonSchemaType.Integer;
            schema.Pattern = null;

            schema.Format = actualType switch
            {
                Type currentType when currentType == typeof(long) => "int64",
                Type currentType when currentType == typeof(ulong) => "int64",
                _ => "int32",
            };

            return Task.CompletedTask;
        }
    }
}