using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegistrationSystem.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationSystem.Api.Tests
{
    public static class TestHelpers
    {
        public static WebApplicationFactory<Program> UseSqlite(WebApplicationFactory<Program> factory, string databaseName)
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Remove(services.Single(descriptor => descriptor.ServiceType == typeof(ApplicationDbContext)));
                    services.Remove(services.Single(descriptor => descriptor.ServiceType == typeof(DbContextOptions)));
                    services.Remove(services.Single(descriptor => descriptor.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)));
                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(GetConnectionString(databaseName)).EnableSensitiveDataLogging());
                });
            });
        }

        public static string GetConnectionString(string databaseName)
        {
            return $"DataSource=file:{databaseName}?mode=memory";
        }
    }
}
