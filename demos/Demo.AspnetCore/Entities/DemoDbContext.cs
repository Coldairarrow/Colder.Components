using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace Demo.AspnetCore.Entities;

public class DemoDbContext : DbContext
{
    static DemoDbContext()
    {
        //开启兼容模式 https://www.npgsql.org/efcore/release-notes/6.0.html?tabs=annotations
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    private static readonly ValueConverter<DateTime, DateTime> _dateTimeConverter
            = new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Local));
    public DemoDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //DateTime默认为Local
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(_dateTimeConverter);
            }
        }
    }
}
