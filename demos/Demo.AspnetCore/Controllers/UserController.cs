using Colder.Api.Abstractions.Controllers;
using Demo.AspnetCore.Dtos;
using Demo.AspnetCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System;

namespace Demo.AspnetCore.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(GroupName = "group1")]
public class UserController : CRUDControllerBase<long, User, UserDto, UserExtendDto, UserSearchDto, DemoDbContext>
{
    public UserController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
