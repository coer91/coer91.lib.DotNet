using Microsoft.AspNetCore.Mvc.Filters; 
using Microsoft.AspNetCore.Http; 

namespace coer91.NET
{
    public static class HttpRequest
    { 
        #region ToHttpRequest   

        public static HttpRequestDTO ToHttpRequest(ActionExecutingContext context)
            => ToHttpRequest(context?.HttpContext);


        public static HttpRequestDTO ToHttpRequest(ActionExecutedContext context)
            => ToHttpRequest(context?.HttpContext);


        public static HttpRequestDTO ToHttpRequest(ExceptionContext context)
            => ToHttpRequest(context?.HttpContext);


        public static HttpRequestDTO ToHttpRequest(IHttpContextAccessor httpContextAccessor)
            => ToHttpRequest(httpContextAccessor?.HttpContext);


        public static HttpRequestDTO ToHttpRequest(HttpContext context)
        {
            try
            {
                var request = context is not null ? new HttpRequestDTO()
                {
                    Project       = Security.ProjectName,
                    Controller    = context.Request.RouteValues.TryGetValue("controller", out var controller) ? controller.ToString() : string.Empty,
                    Method        = context.Request.RouteValues.TryGetValue("action", out var action) ? action.ToString() : string.Empty,
                    HTTP          = context.Request.Method,
                    UserId        = int.TryParse(Security.GetClaimValue("UserId", context), out var userId) ? userId : 0,
                    User          = context.Request.Headers.TryGetValue("Clien-User", out var user) ? user : Security.GetClaimValue("User", context),                   
                    PartnerId     = int.TryParse(Security.GetClaimValue("PartnerId", context), out var partnerId) ? partnerId : 0,
                    Partner       = Security.GetClaimValue("Partner", context),                     
                    Email         = Security.GetClaimValue("Email", context),
                    Language      = Security.GetClaimValue("Language", context), 
                    UtcOffset     = context.Request.Headers.TryGetValue("Utc-Offset", out var utcOffset) && int.TryParse(utcOffset, out int utcOffsetInteger) ? utcOffsetInteger : 0,
                    Roles         = Security.GetClaimValue("Roles", context)?.Replace("[", string.Empty)?.Replace("]", string.Empty)?.Replace(" ", string.Empty)?.Split(',') ?? [],
                    JWTExpiration = Security.GetClaimValue("ExpirationDate", context).ToDateTime()
                } : null;

                foreach (var param in context.Request.Query) 
                    request.QueryParams.Add(param.Key, $"{param.Value}");

                foreach (var param in context.Request.RouteValues.Where(x => x.Key != "controller" && x.Key != "action"))
                    request.RouteParams.Add(param.Key, $"{param.Value}");

                return request;
            }

            catch
            {
                return null;
            }
        }

        #endregion 
    }
}