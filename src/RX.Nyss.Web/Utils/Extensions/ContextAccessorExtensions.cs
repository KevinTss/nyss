﻿using Microsoft.AspNetCore.Http;

namespace RX.Nyss.Web.Utils.Extensions
{
    public static class ContextAccessorExtensions
    {
        public static int? GetResourceParameter(this IHttpContextAccessor httpContextAccessor, string parameterName) =>
            httpContextAccessor.HttpContext.Request.GetResourceParameter(parameterName);

        public static int? GetResourceParameter(this HttpRequest request, string parameterName)
        {
            if (request.RouteValues.ContainsKey(parameterName) && int.TryParse(request.RouteValues[parameterName].ToString(), out var idFromRoute))
            {
                return idFromRoute;
            }

            if (request.Query.ContainsKey(parameterName) && int.TryParse(request.Query[parameterName].ToString(), out var idFromQuery))
            {
                return idFromQuery;
            }

            return null;
        }
    }
}
