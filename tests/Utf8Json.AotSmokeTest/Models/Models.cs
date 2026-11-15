using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// Input for Utf8Json.UniversalCodeGenerator. Regenerate Generated/Utf8JsonGenerated.cs after changing these
// (see README.md in this folder); CI fails if the checked-in output is stale.
namespace AotSmoke.Models
{
    public enum Status
    {
        Unknown = 0,
        Active = 1,
        Retired = 2,
    }

    public class Order
    {
        public int Id { get; set; }

        [DataMember(Name = "customer")]
        public string Customer { get; set; }

        public Status Status { get; set; }
        public Status? PreviousStatus { get; set; }
        public decimal Total { get; set; }
        public DateTime Created { get; set; }
        public int? Priority { get; set; }

        // value-type generic collections: DynamicGenericResolver cannot build these under native AOT,
        // the generated resolver registers closed formatters for them
        public List<Guid> Tags { get; set; }
        public Dictionary<string, long> Counters { get; set; }

        public int[] Numbers { get; set; }
        public List<OrderLine> Lines { get; set; }
        public Address ShipTo { get; set; }
    }

    public class OrderLine
    {
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    public struct Address
    {
        public string City { get; }
        public string Zip { get; }

        public Address(string city, string zip)
        {
            City = city;
            Zip = zip;
        }
    }
}
