using System;

namespace Contracts.Requests
{
    [Serializable]
    public class DateIntervalRequest : BearerTokenRequest
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
