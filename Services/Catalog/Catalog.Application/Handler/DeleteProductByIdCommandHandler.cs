namespace Catalog.Application.Handler;

public class DeleteProductByIdCommandHandler(IProductRepository productRepository)
    : IRequestHandler<DeleteProductByIdCommand, bool>
{
    public async Task<bool> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await productRepository.DeleteProductAsync(request.Id);
        return deleteResult;
    }
}

