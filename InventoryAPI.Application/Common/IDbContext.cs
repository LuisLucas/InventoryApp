using InventoryAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Application.Common {
    public interface IDbContext
    {
        DbSet<Product> Products { get; }

        DbSet<Stock> Stocks { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
