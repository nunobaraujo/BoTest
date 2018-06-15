namespace Contracts
{
    public interface IPaymentDue
    {
        string Id { get; }

        string Description { get; }
        int DueDays { get; }
        
    }
}