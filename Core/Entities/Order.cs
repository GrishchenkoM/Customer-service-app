using System;

namespace Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime? ShippingDateTime { get; set; }
        public string Note { get; set; }

        public string ManagerId { get; set; }
        public string UserId { get; set; }
    }
}
