using HateoasLib.Attributes;
using HateoasLib.Interfaces;
using HateoasLib.Models.ResponseModels;
using InventoryApi.Models;
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
                                IHateoas<ProductsController, ProductDto> hateoasMeta,
                                IHateoas<ProductModel> hateoas) : ControllerBase
{
    // GET: api/<ProductsController>
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        IEnumerable<ProductDto> products = await getProducts.Handle();

        var values = new Tuple<string, Func<ProductDto, object>>("id", new Func<ProductDto, object>((product) => product.Id));
        var itemActions = new List<HateoasLib.Models.ControllerAction<ProductDto, object>>()
        {
            new("Get", values, "self", "GET"),
            new("Put", values, "update_product", "PUT"),
            new("Delete", values, "delete_product", "DELETE"),
        };
        var listActions = new List<HateoasLib.Models.ControllerAction>()
        {
            new("Get", new { }, "self", "GET"),
            new("Post", new { }, "create_product", "POST"),
        };

        CollectionResource<ProductDto> collectionResource = hateoasMeta
                                                                .CreateCollectionResponse(products, listActions, itemActions);
        
        PaginatedResource<ProductModel> paginated = hateoas.CreatePaginatedResponse(
                products.Select(product =>
                            new ProductModel(product.Id, product.Name, product.Description, product.Sku, product.Price, product.CreatedAt, product.CreatedBy, product.LastUpdatedAt, product.LastUpdatedBy)), typeof(ProductsController), 1, 10, 30);

        var producModelList = products.Select(product =>
                            new ProductModel(product.Id, product.Name, product.Description, product.Sku, product.Price, product.CreatedAt, product.CreatedBy, product.LastUpdatedAt, product.LastUpdatedBy));
        CollectionResource<ProductModel> collectionResource2 = hateoas
                            .CreateCollectionResponse(producModelList, typeof(ProductsController));

        return Ok(collectionResource2);
    }

    // GET api/<ProductsController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
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

        var prodModel = new ProductModel(product.Id, product.Name, product.Description, product.Sku, product.Price, product.CreatedAt, product.CreatedBy, product.LastUpdatedAt, product.LastUpdatedBy);
        Resource<ProductModel> resource = hateoas
                                            .CreateResponse(prodModel, typeof(ProductsController));

        return Ok(resource);
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
