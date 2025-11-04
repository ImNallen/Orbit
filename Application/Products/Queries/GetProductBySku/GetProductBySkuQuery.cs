using Application.Products.Queries.GetProductById;
using Domain.Abstractions;
using MediatR;

namespace Application.Products.Queries.GetProductBySku;

/// <summary>
/// Query to get a product by SKU.
/// </summary>
public sealed record GetProductBySkuQuery(string Sku) : IRequest<Result<ProductDto, DomainError>>;

