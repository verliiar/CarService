using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CarService.Api;

public sealed class BearerOpenApiTransformer(IApiDescriptionGroupCollectionProvider descriptions) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };
        var actions = descriptions.ApiDescriptionGroups.Items.SelectMany(x => x.Items).ToList();
        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations ?? [])
            {
                var action = actions.FirstOrDefault(x => "/" + x.RelativePath == path.Key &&
                    string.Equals(x.HttpMethod, operation.Key.ToString(), StringComparison.OrdinalIgnoreCase));
                if (action?.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any() == true)
                {
                    continue;
                }

                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
                operation.Value.Responses ??= new OpenApiResponses();
                operation.Value.Responses.TryAdd("401", new OpenApiResponse { Description = "Authentication required" });
                operation.Value.Responses.TryAdd("403", new OpenApiResponse { Description = "Insufficient permissions" });
            }
        }

        return Task.CompletedTask;
    }
}
