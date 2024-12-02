using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Authorization
{
    public class AttributeAuthorizer
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public AttributeAuthorizer(
            RequestDelegate next,
            IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<UseAttributeAuthorizer>() == null)
            {
                // Skip middleware logic and call the next middleware
                await _next(context);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var retriever = scope.ServiceProvider.GetRequiredService<Retriever>();
            var responseBuilder = scope.ServiceProvider.GetRequiredService<ResponseBuilder>();

            // Save the original response body stream
            var originalBodyStream = context.Response.Body;
            var path = context.Request.Path;
            var method = context.Request.Method;

            // Extract the Bearer token from the Authorization header
            if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) ||
                !authorizationHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Authorization header is missing or invalid");
            }

            var token = authorizationHeader.ToString().Substring("Bearer ".Length).Trim();
            var roleId = GetRoleIdFromToken(token);

            if (roleId == null)
            {
                throw new InvalidOperationException("Role ID is missing from the token");
            }

            // Create a new memory stream to capture the response
            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                // Call the next middleware in the pipeline
                await _next(context);

                // Process the response here before sending it to the client
                memoryStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                // Modify or log the response body as needed
                var processedBody = ProcessResponseBody(responseBody, roleId, method, path, retriever, responseBuilder);

                // Write the modified response back to the original body stream
                var modifiedBytes = Encoding.UTF8.GetBytes(processedBody);
                context.Response.Body = originalBodyStream;
                await context.Response.Body.WriteAsync(modifiedBytes, 0, modifiedBytes.Length);
            }
        }

        private string ProcessResponseBody(
            string responseBody,
            string roleId,
            string method,
            string path,
            Retriever retriever,
            ResponseBuilder responseBuilder)
        {
            var authAttrs = retriever.GetAuthorizedAttributes(roleId, method, path);
            return responseBuilder.BuildResponse(responseBody, authAttrs);
        }

        private string? GetRoleIdFromToken(string token)
        {
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                if (jwtHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
                    throw new InvalidOperationException("Invalid token");

                var roleIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
                return roleIdClaim?.Value;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to extract role ID from token", ex);
            }
        }
    }
}
