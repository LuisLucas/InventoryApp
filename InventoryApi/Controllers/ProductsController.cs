using InventoryAPI.Application.Products;
using InventoryAPI.Application.Products.Command.Create;
using InventoryAPI.Application.Products.Command.Delete;
using InventoryAPI.Application.Products.Command.Update;
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
        private readonly IUpdateProduct _updateProduct;
        private readonly IDeleteProduct _deleteProduct;

        public ProductsController(
            IGetProducts getProducts, 
            ICreateProduct createProduct,
            IUpdateProduct updateProduct,
            IDeleteProduct deleteProduct)
        {
            this._getProducts = getProducts;
            this._createProduct = createProduct;
            this._updateProduct = updateProduct;
            this._deleteProduct = deleteProduct;
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
            var product = await this._createProduct.Handle(command);
            return Ok(product);
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

            var command = new UpdateProductCommand() {
                Id = id,
                Name = productDto.Name,
                Description = productDto.Description,
                Sku = productDto.Sku,
                Price = productDto.Price
            };
            var updatedProduct = await this._updateProduct.Handle(command);
            return Ok(updatedProduct);
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) {
                return BadRequest("Product id is required.");
            }

            var command = new DeleteProductCommand() {
                Id = id
            };
            var updatedProduct = await this._deleteProduct.Handle(command);
            return Ok("Success");
        }
    }
}
