namespace Catalog.API.Controllers;

public class CatalogController(IMediator mediator,ILogger<CatalogController> logger) : ApiController
{
    [HttpGet]
    [Route("[action]/{id}",Name = "GetProductById")]
    [ProducesResponseType(typeof(ProductResponse),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ProductResponse>> GetProductByIdAsync(string id)
    {
        var query = new GetProductByIdQuery(id);
        var result= await mediator.Send(query);
        return Ok(result);
    }
    [HttpGet]
    [Route("[action]/{productName}",Name = "GetProductByProductName")]
    [ProducesResponseType(typeof(IList<ProductResponse>),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IList<ProductResponse>>> GetProductByProductNameAsync(string productName)
    {
        var query = new GetProductByNameQuery(productName);
        var result= await mediator.Send(query);
        logger.LogInformation($"Product with {productName} fetched");
        return Ok(result);
    }

    [HttpGet]
    [Route("GetAllProducts", Name = "GetAllProducts")]
    [ProducesResponseType(typeof(Pagination<ProductResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IList<ProductResponse>>> GetAllProductsAsync([FromQuery] CatalogSpecParams catalogSpecParams)
    {
        var query = new GetAllProductsQuery(catalogSpecParams);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetAllBrands", Name = "GetAllBrands")]
    [ProducesResponseType(typeof(IList<BrandResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IList<BrandResponse>>> GetAllBrandsAsync()
    {
        var query = new GetAllBrandsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetAllTypes", Name = "GetAllTypes")]
    [ProducesResponseType(typeof(IList<TypesResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IList<TypesResponse>>> GetAllTypes()
    {
        var query = new GetAllTypesQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
    [HttpGet]
    [Route("[action]/{brandName}", Name = "GetAllProductByBrandName")]
    [ProducesResponseType(typeof(IList<ProductResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IList<ProductResponse>>> GetAllProductByBrandName(string brandName)
    {
        var query = new GetProductByBrandNameQuery(brandName);
        var result = await mediator.Send(query);
        return result==null || result.Count == 0 ? NotFound() : Ok(result);
    }
    [HttpPost]
    [Route("CreateProduct")]
    [ProducesResponseType(typeof(ProductResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    [HttpPut]
    [Route("UpdateProduct")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> UpdateProduct([FromBody] UpdateProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    [HttpDelete]
    [Route("{id}",Name="DeleteProduct")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteProduct(string id)
    {
        var command = new DeleteProductByIdCommand(id);
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
}