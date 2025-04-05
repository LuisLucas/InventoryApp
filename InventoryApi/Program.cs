
using InventoryApi.Infrastructure.Data;
using InventoryAPI.Application.Common;
using InventoryAPI.Application.Products.Queries;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi
{
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

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string" + "'DefaultConnection' not found.");
            builder.Services.AddDbContext<InventoryContext>(options =>
                                                                    options.UseSqlServer(connectionString));

            // Register IApplicationDbContext interface with ApplicationDbContext implementation
            builder.Services.AddScoped<IDbContext>(provider => provider.GetService<InventoryContext>());

            builder.Services.AddTransient<IGetProduct, GetProduct>();

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
