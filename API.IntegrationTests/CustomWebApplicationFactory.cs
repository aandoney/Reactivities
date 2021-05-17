using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
using System;

namespace API.FunctionalTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup: class
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {            
            var host = builder.Build();

            // Get service provider.
            var serviceProvider = host.Services;

            // Create a scope to obtain a reference to the database
            // context (AppDbContext).
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DataContext>();
                var userManager = scopedServices.GetRequiredService<UserManager<AppUser>>();

                var logger = scopedServices
                    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                //db.Database.EnsureCreated();

                //try
                //{
                //    // Seed the database with test data.
                //    Seed.SeedData(db, userManager).GetAwaiter().GetResult();
                //}
                //catch (Exception ex)
                //{
                //    logger.LogError(ex, "An error occurred seeding the " +
                //                        $"database with test messages. Error: {ex.Message}");
                //}
            }

            host.Start();
            return host;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            builder
                .UseSolutionRelativeContentRoot("API")
                .UseEnvironment("Testing")
                .ConfigureServices(services =>
                {
                    // Remove the app's ApplicationDbContext registration.
                    //var descriptor = services.SingleOrDefault(
                    //    d => d.ServiceType ==
                    //        typeof(DbContextOptions<DataContext>));

                    //if (descriptor != null)
                    //{
                    //    services.Remove(descriptor);
                    //}

                    //// Add ApplicationDbContext using an in-memory database for testing.
                    //services.AddDbContext<DataContext>(options =>
                    //{
                    //    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    //});

                    ////services.AddScoped<IMediator, NoOpMediator>();
                });
        }
    }
}
