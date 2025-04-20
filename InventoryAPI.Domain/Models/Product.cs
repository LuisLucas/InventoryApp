
using InventoryAPI.Domain.Entities;

namespace InventoryAPI.Domain.Models;
public class Product(Entities.Product Product)
{
    public void UpdateProduct(string? name, string? description, string? sku, decimal? price)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (price == null || price.Value <= 0)
        {
            throw new Exception("Price cannot be negative");
        }

        Product.Name = name;
        Product.Description = description;
        Product.Sku = sku;
        Product.Price = price;
        Product.LastUpdatedBy = "Admin"; // Should be replaced by the user logged in
        Product.LastUpdatedAt = DateTime.UtcNow;
    }

    public Stock CreateStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new Exception("Stock can´t be negative.");
        }

        var stock = new Stock()
        {
            ProductId = Product.Id,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Admin",
            LastUpdatedAt = DateTime.UtcNow,
            LastUpdatedBy = "Admin"
        };
        return stock;
    }

    public void UpdateStock(Stock stock, int changeInStock)
    {
        if(stock.ProductId != Product.Id)
        {
            throw new Exception("Stock dont belong to this product.");
        }

        int? newStock = stock.Quantity - changeInStock;
        if (newStock <= 0)
        {
            throw new Exception("Stock can´t be negative.");
        }

        stock.Quantity = newStock;
    }
}
