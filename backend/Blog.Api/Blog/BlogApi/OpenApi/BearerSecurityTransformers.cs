//scalar open api ayarları
//bearer, http isteğinde tokenı gönderme biçiminin adıdır.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BlogApi.OpenApi
{
    internal sealed class BearerSecuritySchemeTransformer(  //Bu API JWT Bearer Authentication kullanıyor.
        IAuthenticationSchemeProvider authenticationSchemeProvider)
        : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var authenticationSchemes =
                await authenticationSchemeProvider.GetAllSchemesAsync();

            if (authenticationSchemes.Any(x => x.Name == "Bearer"))
            {
                document.Components ??= new OpenApiComponents();

                document.Components.SecuritySchemes =
                    new Dictionary<string, IOpenApiSecurityScheme>
                    {
                        ["Bearer"] = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            In = ParameterLocation.Header,
                            BearerFormat = "JWT"
                        }
                    };
            }
        }
    }

    internal sealed class AuthOperationTransformer
        : IOpenApiOperationTransformer
    {
        public Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            var hasAuthorize =
                context.Description.ActionDescriptor.EndpointMetadata
                    .OfType<IAuthorizeData>()
                    .Any();

            if (!hasAuthorize)
            {
                return Task.CompletedTask;
            }

            operation.Security ??= [];

            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        "Bearer",
                        context.Document)] = []
                });

            return Task.CompletedTask;
        }
    }
}