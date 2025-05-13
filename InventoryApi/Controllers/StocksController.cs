using InventoryAPI.Application.Stocks;
using InventoryAPI.Application.Stocks.Command;
using InventoryAPI.Application.Stocks.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers;

public class StocksController(IGetStock getStock, IUpdateStock createProductStock) : ControllerBase
{
    [HttpGet("product/{id}/stock")]
    public async Task<ActionResult> Get(int id)
    {
        ProductStockDto productStock = await getStock.Handle(id);
        return Ok(productStock);
    }

    [HttpPost("product/{productId}/stock/{stock}")]
    public async Task<IActionResult> Post(int productId, int stock)
    {
        if (productId <= 0)
        {
            return BadRequest("Stock data is required.");
        }

        var command = new UpdateStockCommand(productId, stock);
        ProductStockDto product = await createProductStock.Handle(command);
        return Ok(product);
    }
}
