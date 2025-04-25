using InventoryApi.Hateoas;
using InventoryAPI.Startup;

namespace InventoryApi;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        DependecyInjection.AddInfrastructure(builder.Services, builder.Configuration);
        DependecyInjection.AddApplication(builder.Services);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<IHateoas, HateoasFactory>();


        WebApplication app = builder.Build();

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
