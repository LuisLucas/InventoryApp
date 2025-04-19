using InventoryAPI.Application.Stocks.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers {
    public class StocksController(IGetStock getStock) : ControllerBase 
    {
        [HttpGet]
        public async Task<ActionResult> Get(int productId) 
        {
            var stockQuantity = await getStock.Handle(productId);
            return Ok(stockQuantity);
        }
    }
}
