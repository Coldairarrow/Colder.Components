using Colder.Api.Abstractions;
using Demo.AspnetCore.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

WebApplication.CreateBuilder(args).RunWebApiDefaults(services =>
{
    services.AddDbContext<DemoDbContext>(x => x.UseSqlite("Data Source=db.db"));
    services.AddAutoMapper(Assembly.GetExecutingAssembly());
});