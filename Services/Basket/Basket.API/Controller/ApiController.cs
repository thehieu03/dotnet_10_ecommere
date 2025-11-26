namespace Basket.API.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiController: ControllerBase
{
    
}