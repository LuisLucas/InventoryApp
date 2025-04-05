using InventoryAPI.Application.Products;
using InventoryAPI.Application.Products.Command;
using InventoryAPI.Application.Products.Queries;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventoryApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IGetProducts _getProducts;
        private readonly ICreateProduct _createProduct;

        public ProductsController(IGetProducts getProducts, ICreateProduct createProduct)
        {
            this._getProducts = getProducts;
            this._createProduct = createProduct;
        }

        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get()
        {
            var products = await this._getProducts.Handle();
            return Ok(products);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(int id)
        {
            var product = await this._getProducts.Handle(id);
            if (product == null) {
                return BadRequest("Product not found");
            }

            return Ok(product);
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductDto productDto)
        {
            if (productDto == null) {
                return BadRequest("Product data is required.");
            }

            var command = new CreateProductCommand() {
                Name = productDto.Name,
                Description = productDto.Description,
                Sku = productDto.Sku,
                Price = productDto.Price
            };
            var productId = await this._createProduct.Handle(command);

            return Ok(productId);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductDto productDto)
        {
            if (id == 0) {
                return BadRequest("Product id is required.");
            }

            if (productDto == null) {
                return BadRequest("Product data is required.");
            }

            /*var command = new UpdateProductCommand() {
                Name = productDto.Name,
                Description = productDto.Description,
                Sku = productDto.Sku,
                Price = productDto.Price
            };*/

            return Ok();
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
