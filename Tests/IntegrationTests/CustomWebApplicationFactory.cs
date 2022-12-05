using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tests.IntegrationTests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql("User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=ORM_test");
            });
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            try
            {
                var entry = new Entry
                {
                    Text = "Some random text",
                    EntryDatas = new List<EntryData>
                    {
                        new() {Description = "Some random text", Amount = 11},
                        new() {Description = "Another description", Amount = 4}
                    }
                };
                db.Entries.Add(entry);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        });
    }
}