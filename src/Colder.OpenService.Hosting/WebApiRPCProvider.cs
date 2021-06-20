using Colder.OpenService.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Colder.OpenService.Hosting
{
    internal class WebApiRPCProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            return (typeof(IOpenService).IsAssignableFrom(typeInfo)
                || typeof(ControllerBase).IsAssignableFrom(typeInfo))
                && !typeInfo.IsAbstract
                && !typeInfo.IsInterface;
        }
    }
}
