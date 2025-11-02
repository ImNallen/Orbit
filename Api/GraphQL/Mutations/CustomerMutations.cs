using Api.GraphQL.Inputs;
using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Customers.Commands.CreateCustomer;
using Application.Customers.Commands.UpdateCustomerContactInfo;
using Application.Customers.Commands.UpdateCustomerAddress;
using Application.Customers.Commands.SuspendCustomer;
using Application.Customers.Commands.ActivateCustomer;
using Application.Customers.Commands.DeleteCustomer;
using Domain.Abstractions;
using HotChocolate.Authorization;
using MediatR;

namespace Api.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for customer operations.
/// </summary>
[ExtendObjectType("Mutation")]
public sealed class CustomerMutations
{
    /// <summary>
    /// Creates a new customer.
    /// Requires customers:create permission.
    /// </summary>
    [Authorize(Policy = "customers:create")]
    public async Task<CreateCustomerPayload> CreateCustomerAsync(
        CreateCustomerInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(
            input.Email,
            input.FirstName,
            input.LastName,
            input.PhoneNumber,
            input.Street,
            input.City,
            input.State,
            input.Country,
            input.ZipCode);

        Result<CreateCustomerResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return CreateCustomerPayload.Failure(
                new CustomerError(result.Error.Code, result.Error.Message));
        }

        CreateCustomerResult customerResult = result.Value;

        var customer = new CustomerType
        {
            CustomerId = customerResult.CustomerId,
            Email = customerResult.Email,
            FirstName = customerResult.FirstName,
            LastName = customerResult.LastName,
            PhoneNumber = input.PhoneNumber,
            Address = new AddressType
            {
                Street = input.Street,
                City = input.City,
                State = input.State,
                Country = input.Country,
                ZipCode = input.ZipCode
            },
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return CreateCustomerPayload.Success(customer);
    }

    /// <summary>
    /// Updates customer contact information.
    /// Requires customers:update permission.
    /// </summary>
    [Authorize(Policy = "customers:update")]
    public async Task<UpdateCustomerContactInfoPayload> UpdateCustomerContactInfoAsync(
        UpdateCustomerContactInfoInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCustomerContactInfoCommand(
            input.CustomerId,
            input.Email,
            input.FirstName,
            input.LastName,
            input.PhoneNumber);

        Result<UpdateCustomerContactInfoResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return UpdateCustomerContactInfoPayload.Failure(
                new CustomerError(result.Error.Code, result.Error.Message));
        }

        return UpdateCustomerContactInfoPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Updates customer address.
    /// Requires customers:update permission.
    /// </summary>
    [Authorize(Policy = "customers:update")]
    public async Task<UpdateCustomerAddressPayload> UpdateCustomerAddressAsync(
        UpdateCustomerAddressInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCustomerAddressCommand(
            input.CustomerId,
            input.Street,
            input.City,
            input.State,
            input.Country,
            input.ZipCode);

        Result<UpdateCustomerAddressResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return UpdateCustomerAddressPayload.Failure(
                new CustomerError(result.Error.Code, result.Error.Message));
        }

        return UpdateCustomerAddressPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Suspends a customer account.
    /// Requires customers:update permission.
    /// </summary>
    [Authorize(Policy = "customers:update")]
    public async Task<SuspendCustomerPayload> SuspendCustomerAsync(
        SuspendCustomerInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new SuspendCustomerCommand(input.CustomerId);
        Result<SuspendCustomerResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return SuspendCustomerPayload.Failure(
                new CustomerError(result.Error.Code, result.Error.Message));
        }

        return SuspendCustomerPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Activates a suspended customer account.
    /// Requires customers:update permission.
    /// </summary>
    [Authorize(Policy = "customers:update")]
    public async Task<ActivateCustomerPayload> ActivateCustomerAsync(
        ActivateCustomerInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ActivateCustomerCommand(input.CustomerId);
        Result<ActivateCustomerResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return ActivateCustomerPayload.Failure(
                new CustomerError(result.Error.Code, result.Error.Message));
        }

        return ActivateCustomerPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Deletes a customer account (soft delete).
    /// Requires customers:delete permission.
    /// </summary>
    [Authorize(Policy = "customers:delete")]
    public async Task<DeleteCustomerPayload> DeleteCustomerAsync(
        DeleteCustomerInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCustomerCommand(input.CustomerId);
        Result<DeleteCustomerResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return DeleteCustomerPayload.Failure(
                new CustomerError(result.Error.Code, result.Error.Message));
        }

        return DeleteCustomerPayload.Success(result.Value.Message);
    }
}

