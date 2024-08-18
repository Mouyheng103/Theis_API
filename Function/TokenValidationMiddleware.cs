namespace API.Function
{

    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            var tokenService = context.RequestServices.GetRequiredService<TokenService>();

            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (await tokenService.IsTokenBlacklisted(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token is invalid.");
                return;
            }
            await _next(context);
        }
    }
}
