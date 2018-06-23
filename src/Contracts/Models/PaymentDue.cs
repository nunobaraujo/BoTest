using MessagePack;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class PaymentDue : IPaymentDue
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int DueDays { get; set; }
    }
}
