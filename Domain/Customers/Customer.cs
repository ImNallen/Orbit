using Domain.Abstractions;
using Domain.Shared.ValueObjects;

namespace Domain.Customers;

public class Customer : Entity 
{
    private Customer(
        Guid id,
        Email email,
        FullName fullName,
        Address address) 
        : base(id)
    {
        Email = email;
        FullName = fullName;
        Address = address;
    }
    
    private Customer() {}
    
    public Email Email { get; private set; }
    
    public FullName FullName { get; private set; }
    
    public Address Address { get; private set; }
    
    public static Customer Create(
        Email email,
        FullName fullName,
        Address address)
    {
        var customer = new Customer(
            Guid.CreateVersion7(),
            email,
            fullName,
            address);
        
        // TODO: Add domain event
        
        return customer;
    }
}
