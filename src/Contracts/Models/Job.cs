using System;

namespace Contracts.Models
{
    public class Job : IJob
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerRouteId { get; set; }
        public string ProductId { get; set; }
        public DateTime BeginDate { get; set; }
        public string ClientRef { get; set; }
        public DateTime CreationDate { get; set; }
        public int CurrentState { get; set; }
        public string Description { get; set; }
        public DateTime? FinishDate { get; set; }
        public string JobReference { get; set; }
        public string Notes { get; set; }
        public bool Option1 { get; set; }
        public bool Option2 { get; set; }
        public bool Option3 { get; set; }
        public bool Option4 { get; set; }
        public bool Option5 { get; set; }
        public bool Option6 { get; set; }
        public decimal ProductHeight { get; set; }
        public decimal ProductLength { get; set; }
        public int ProductUnitType { get; set; }
        public decimal ProductWidth { get; set; }
        public decimal TotalValue { get; set; }
        public string UserId { get; set; }
    }
}
