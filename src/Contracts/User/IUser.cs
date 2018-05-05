using System;

namespace Contracts.User
{
    public interface IUser
    {
        string Id { get; }
        string UserName { get; }
        string PasswordHash { get; }
        string FirstName { get; }
        string LastName { get; }
        DateTime CreationDate { get; }
        string Address { get; }
        string PostalCode { get; }
        string City { get; }
        string Country { get; }
        string Theme { get; }
        int? Accentcolor { get; }
        string Language { get; }
        string Pin { get; }
        string Email { get; }
    }
}
