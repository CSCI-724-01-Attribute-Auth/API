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
            using var scope = _serviceProvider.CreateScope();
            var retriever = scope.ServiceProvider.GetRequiredService<Retriever>();
            var responseBuilder = scope.ServiceProvider.GetRequiredService<ResponseBuilder>();

            // Save the original response body stream
            var originalBodyStream = context.Response.Body;
            var path = context.Request.Path;
            var method = context.Request.Method;

            if (!context.Request.Headers.TryGetValue("X-Client-ID", out var clientId))
            {
                throw new InvalidOperationException("X-Client-ID header is missing");
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
                var processedBody = ProcessResponseBody(responseBody, clientId, method, path, retriever, responseBuilder);

                // Write the modified response back to the original body stream
                var modifiedBytes = Encoding.UTF8.GetBytes(processedBody);
                context.Response.Body = originalBodyStream;
                await context.Response.Body.WriteAsync(modifiedBytes, 0, modifiedBytes.Length);
            }
        }

        private string ProcessResponseBody(
            string responseBody,
            string clientId,
            string method,
            string path,
            Retriever retriever,
            ResponseBuilder responseBuilder)
        {
            var authAttrs = retriever.GetAuthorizedAttributes(clientId, method, path);
            return responseBuilder.BuildResponse(responseBody, authAttrs);
        }
    }
}
