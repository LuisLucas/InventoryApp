using HateoasLib.Attributes;
using HateoasLib.Interfaces;
using HateoasLib.Models;
using HateoasLib.Models.ResponseModels;
using InventoryAPI.Application.Products;
using InventoryAPI.Application.Products.Command.Create;
using InventoryAPI.Application.Products.Command.Delete;
using InventoryAPI.Application.Products.Command.Update;
using InventoryAPI.Application.Products.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableHateoas(typeof(ProductDto))]
public class ProductsController(IGetProducts getProducts,
                                ICreateProduct createProduct,
                                IUpdateProduct updateProduct,
                                IDeleteProduct deleteProduct,
                                IHateoas<ProductsController, ProductDto> hateoasMeta) : ControllerBase
{
    // GET: api/<ProductsController>
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        IEnumerable<ProductDto> products = await getProducts.Handle();

        var values = new Tuple<string, Func<ProductDto, object>>("id", new Func<ProductDto, object>((product) => product.Id));
        var itemActions = new List<ControllerAction<ProductDto, object>>()
        {
            new("Get", values, "self", "GET"),
            new("Put", values, "update_product", "PUT"),
            new("Delete", values, "delete_product", "DELETE"),
        };
        var listActions = new List<ControllerAction>()
        {
            new("Get", new { }, "self", "GET"),
            new("Post", new { }, "create_product", "POST"),
        };

        CollectionResource<ProductDto> collectionResource = hateoasMeta
                                                                .CreateCollectionResponse(products, listActions, itemActions);


        return Ok(collectionResource);
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
