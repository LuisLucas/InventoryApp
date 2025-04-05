
using InventoryApi.Infrastructure.Data;
using InventoryAPI.Application.Common;
using InventoryAPI.Application.Products.Command;
using InventoryAPI.Application.Products.Queries;
using InventoryAPI.Startup;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi {
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                    .AddJsonOptions(options => {
                        options.JsonSerializerOptions.WriteIndented = true;
                    });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            DependecyInjection.AddInfrastructure(builder.Services, builder.Configuration);
            DependecyInjection.AddApplication(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
