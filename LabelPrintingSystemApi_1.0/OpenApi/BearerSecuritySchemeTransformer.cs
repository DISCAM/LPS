using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace LabelPrintingSystemApi_1._0.OpenApi
{
    public sealed class BearerSecuritySchemeTransformer
        : IOpenApiDocumentTransformer
    {
        private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;

        public BearerSecuritySchemeTransformer(
            IAuthenticationSchemeProvider authenticationSchemeProvider
        )
        {
            this.authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken
        )
        {
            var authenticationSchemes =
                await authenticationSchemeProvider.GetAllSchemesAsync();

            var hasBearerScheme = authenticationSchemes.Any(
                (scheme) => scheme.Name == "Bearer"
            );

            if (!hasBearerScheme)
            {
                return;
            }

            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes ??=
                new Dictionary<string, IOpenApiSecurityScheme>();

            document.Components.SecuritySchemes["Bearer"] =
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                };

            foreach (
                var operation in document.Paths.Values.SelectMany(
                    (path) => path.Operations
                )
            )
            {
                operation.Value.Security ??= [];

                operation.Value.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(
                            "Bearer",
                            document
                        )] = [],
                    }
                );
            }
        }
    }
}