using System.Dynamic;
using Microsoft.AspNetCore.Http;

namespace Fydhia.Library;

public abstract class HypermediaTypeFormatter
{
    public abstract void Format(ExpandoObject responseObject, HttpContext httpContext);
}