using InventoryAPI.Application.Products.Command.Create;
using InventoryAPI.Application.Products.Command.Delete;
using InventoryAPI.Application.Products.Command.Update;
using InventoryAPI.Application.Products;
using InventoryAPI.Application.Stocks.Queries;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Application.Stocks;
using InventoryAPI.Application.Stocks.Command;

namespace InventoryApi.Controllers {
    public class StocksController(IGetStock getStock, ICreateStock createProductStock) : ControllerBase 
    {
        [HttpGet]
        public async Task<ActionResult> Get(int productId) 
        {
            var productStock = await getStock.Handle(productId);
            return Ok(productStock);
        } 

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductStockDto productStockDto) {
            if (productStockDto == null) {
                return BadRequest("Stock data is required.");
            }

            var command = new CreateStockCommand(productStockDto.productId, productStockDto.stock);
            var product = await createProductStock.Handle(command);
            return Ok(product);
        }
    }
}
