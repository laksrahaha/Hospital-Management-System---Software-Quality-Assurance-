using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReserveHealth.Api.Data;

namespace ReserveHealth.Tests
{
    public class ReserveHealthWebApplicationFactory
        : WebApplicationFactory<Program>
    {
        private readonly string _databaseName =
            $"ReserveHealthTests_{Guid.NewGuid()}";


        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");


            // Provides safe JWT settings specifically
            // for the automated test environment.
            builder.ConfigureAppConfiguration(
                (context, configuration) =>
                {
                    var testSettings =
                        new Dictionary<string, string?>
                        {
                            ["Jwt:Issuer"] =
                                "ReserveHealth.Api",

                            ["Jwt:Audience"] =
                                "ReserveHealth.Frontend",

                            ["Jwt:Key"] =
                                "ReserveHealth-Test-Key-2026-Secure-JWT-Authentication"
                        };


                    configuration.AddInMemoryCollection(
                        testSettings
                    );
                }
            );


            builder.ConfigureServices(
                services =>
                {
                    // Remove the production SQL Server
                    // configuration completely.
                    services.RemoveAll<
                        IDbContextOptionsConfiguration<
                            ReserveHealthContext>>();

                    services.RemoveAll<
                        DbContextOptions<
                            ReserveHealthContext>>();

                    services.RemoveAll<
                        ReserveHealthContext>();


                    // Replace SQL Server with an isolated
                    // in-memory database for automated tests.
                    services.AddDbContext<
                        ReserveHealthContext>(
                            options =>
                            {
                                options.UseInMemoryDatabase(
                                    _databaseName
                                );
                            }
                        );
                }
            );
        }
    }
}