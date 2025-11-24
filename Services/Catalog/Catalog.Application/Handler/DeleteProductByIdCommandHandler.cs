namespace Catalog.Application.Handler;

public class DeleteProductByIdCommandHandler : IRequestHandler<DeleteProductByIdCommand, bool>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductByIdCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<bool> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await _productRepository.DeleteProductAsync(request.Id);
        return deleteResult;
    }
}

