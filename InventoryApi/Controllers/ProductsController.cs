using InventoryAPI.Application.Common;
using InventoryAPI.Application.Products;
using InventoryAPI.Application.Products.Command.Create;
using InventoryAPI.Application.Products.Command.Delete;
using InventoryAPI.Application.Products.Command.Update;
using InventoryAPI.Application.Products.Queries;
using InventoryAPI.Hateoas;
using InventoryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IGetProducts getProducts,
                                ICreateProduct createProduct,
                                IUpdateProduct updateProduct,
                                IDeleteProduct deleteProduct,
                                LinkGenerator linkGenerator) : ControllerBase
{
    // GET: api/<ProductsController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductModel>>> Get()
    {
        IEnumerable<ProductDto> products = await getProducts.Handle();

        var productsModel = new List<ProductModel>();
        foreach (ProductDto product in products)
        {
            var actions = new List<ControllerAction>()
            {
                new("Get", new { id = product.Id }, "self", "GET"),
                new("Put", new { id = product.Id }, "update_product", "PUT"),
                new("Delete", new { id = product.Id }, "delete_product", "DELETE"),
            };

            List<Link> links = GenerateLinks.BuildLinks(
                linkGenerator,
                "Products",
                actions,
                HttpContext.Request.Scheme,
                HttpContext.Request.Host);

            productsModel.Add(
                new ProductModel()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Sku = product.Sku,
                    Price = product.Price,
                    CreatedAt = product.CreatedAt,
                    CreatedBy = product.CreatedBy,
                    LastUpdatedAt = product.LastUpdatedAt,
                    LastUpdatedBy = product.LastUpdatedBy,
                    Links = links
                });
        }
        return Ok(productsModel);
    }

    // GET api/<ProductsController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        if (id == 0)
        {
            return BadRequest("Id cannot be zero.");
        }

        ProductDto product = await getProducts.Handle(id);
        if (product == null)
        {
            return BadRequest("Product not found");
        }

        return Ok(product);
    }

    // POST api/<ProductsController>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductDto productDto)
    {
        if (productDto == null)
        {
            return BadRequest("Product data is required.");
        }

        var command = new CreateProductCommand(productDto.Name, productDto.Description, productDto.Sku, productDto.Price);
        ProductDto product = await createProduct.Handle(command);
        return Ok(product);
    }

    // PUT api/<ProductsController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] ProductDto productDto)
    {
        if (id == 0)
        {
            return BadRequest("Product id is required.");
        }

        if (productDto == null)
        {
            return BadRequest("Product data is required.");
        }

        var command = new UpdateProductCommand(id, productDto.Name, productDto.Description, productDto.Sku, productDto.Price);
        ProductDto updatedProduct = await updateProduct.Handle(command);
        return Ok(updatedProduct);
    }

    // DELETE api/<ProductsController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id == 0)
        {
            return BadRequest("Product id is required.");
        }

        var command = new DeleteProductCommand(id);
        bool isProductDeleted = await deleteProduct.Handle(command);
        return Ok(isProductDeleted ? "Success" : "Failure");
    }
}
