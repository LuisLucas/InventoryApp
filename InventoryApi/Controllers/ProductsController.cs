using InventoryApi.Models;
using InventoryAPI.Application.Common;
using InventoryAPI.Application.Products;
using InventoryAPI.Application.Products.Command.Create;
using InventoryAPI.Application.Products.Command.Delete;
using InventoryAPI.Application.Products.Command.Update;
using InventoryAPI.Application.Products.Queries;
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
        foreach (var product in products)
        {
            var links = new List<Link>();
            var absoluteUri = linkGenerator.GetUriByAction(
                HttpContext,
                action: "Get",
                controller: "Products",
                values: new { id = product.Id }); 
            var link_self = new Link(absoluteUri, "self", "GET");
            links.Add(link_self);

            var absoluteUriUpdate = linkGenerator.GetUriByAction(
                HttpContext,
                action: "Put",
                controller: "Products",
                values: new { id = product.Id });
            var link_update = new Link(absoluteUriUpdate, "update_product", "PUT");
            links.Add(link_update);

            var absoluteUridelete = linkGenerator.GetUriByAction(
                HttpContext,
                action: "Delete",
                controller: "Products",
                values: new { id = product.Id });
            var link_delete = new Link(absoluteUridelete, "delete_product", "DELETE");
            links.Add(link_delete);

            productsModel.Add(
                new ProductModel(){
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Sku = product.Sku,
                    Price = product.Price,
                    CreatedAt = product.CreatedAt,
                    CreatedBy = product.CreatedBy,
                    LastUpdatedAt = product.LastUpdatedAt,
                    LastUpdatedBy = product.LastUpdatedBy,
                    Links = links });
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
