using Api.GraphQL.Inputs;
using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Commands.UpdateProductStock;
using Application.Products.Commands.ActivateProduct;
using Application.Products.Commands.DeactivateProduct;
using Application.Products.Commands.DeleteProduct;
using Domain.Abstractions;
using HotChocolate.Authorization;
using MediatR;

namespace Api.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for product operations.
/// </summary>
[ExtendObjectType("Mutation")]
public sealed class ProductMutations
{
    /// <summary>
    /// Creates a new product.
    /// Requires products:create permission.
    /// </summary>
    [Authorize(Policy = "products:create")]
    public async Task<CreateProductPayload> CreateProductAsync(
        CreateProductInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            input.Name,
            input.Description,
            input.Price,
            input.Sku,
            input.InitialStock ?? 0);

        Result<CreateProductResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return CreateProductPayload.Failure(
                new ProductError(result.Error.Code, result.Error.Message));
        }

        CreateProductResult productResult = result.Value;

        var product = new ProductType
        {
            ProductId = productResult.ProductId,
            Name = productResult.Name,
            Description = input.Description,
            Price = productResult.Price,
            Sku = productResult.Sku,
            StockQuantity = input.InitialStock ?? 0,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return CreateProductPayload.Success(product);
    }

    /// <summary>
    /// Updates product information.
    /// Requires products:update permission.
    /// </summary>
    [Authorize(Policy = "products:update")]
    public async Task<UpdateProductPayload> UpdateProductAsync(
        UpdateProductInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            input.ProductId,
            input.Name,
            input.Description,
            input.Price);

        Result<UpdateProductResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return UpdateProductPayload.Failure(
                new ProductError(result.Error.Code, result.Error.Message));
        }

        return UpdateProductPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Updates product stock quantity.
    /// Requires products:update permission.
    /// </summary>
    [Authorize(Policy = "products:update")]
    public async Task<UpdateProductStockPayload> UpdateProductStockAsync(
        UpdateProductStockInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductStockCommand(
            input.ProductId,
            input.Quantity);

        Result<UpdateProductStockResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return UpdateProductStockPayload.Failure(
                new ProductError(result.Error.Code, result.Error.Message));
        }

        return UpdateProductStockPayload.Success(
            result.Value.Message,
            result.Value.NewStockQuantity);
    }

    /// <summary>
    /// Activates an inactive product.
    /// Requires products:update permission.
    /// </summary>
    [Authorize(Policy = "products:update")]
    public async Task<ActivateProductPayload> ActivateProductAsync(
        ActivateProductInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ActivateProductCommand(input.ProductId);
        Result<ActivateProductResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return ActivateProductPayload.Failure(
                new ProductError(result.Error.Code, result.Error.Message));
        }

        return ActivateProductPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Deactivates an active product.
    /// Requires products:update permission.
    /// </summary>
    [Authorize(Policy = "products:update")]
    public async Task<DeactivateProductPayload> DeactivateProductAsync(
        DeactivateProductInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateProductCommand(input.ProductId);
        Result<DeactivateProductResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return DeactivateProductPayload.Failure(
                new ProductError(result.Error.Code, result.Error.Message));
        }

        return DeactivateProductPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Deletes a product.
    /// Requires products:delete permission.
    /// </summary>
    [Authorize(Policy = "products:delete")]
    public async Task<DeleteProductPayload> DeleteProductAsync(
        DeleteProductInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(input.ProductId);
        Result<DeleteProductResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return DeleteProductPayload.Failure(
                new ProductError(result.Error.Code, result.Error.Message));
        }

        return DeleteProductPayload.Success(result.Value.Message);
    }
}

