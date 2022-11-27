using Colder.Api.Abstractions.Controllers;
using Demo.AspnetCore.Dtos;
using Demo.AspnetCore.Entities;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Demo.AspnetCore.Controllers;

[AllowAnonymous]
public class UserController : CRUDControllerBase<long, User, UserDto, UserExtendDto, UserSearchDto, DemoDbContext>
{
    public UserController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
