using InventoryApi.Infrastructure.Data;
using InventoryAPI.Application.Common;
using InventoryAPI.Application.Products.Command;
using InventoryAPI.Application.Products.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryAPI.Startup {
    public static class DependecyInjection
    {
        public static void AddApplication(IServiceCollection services) {
            services.AddTransient<IGetProduct, GetProductQueryHandler>();
            services.AddTransient<IGetProducts, GetProductsQueryHandler>();
            services.AddTransient<ICreateProduct, CreateProductCommandHandler>();

        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) 
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<InventoryContext>(options => options.UseSqlServer(connectionString));

            // Register IApplicationDbContext interface with ApplicationDbContext implementation
            services.AddScoped<IDbContext>(provider => provider.GetService<InventoryContext>());

            return services;
        }
    }
}
