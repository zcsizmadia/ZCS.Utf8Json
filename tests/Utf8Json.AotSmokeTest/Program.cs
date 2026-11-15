using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AotSmoke.Models;
using Utf8Json;
using Utf8Json.Resolvers;

// Native AOT smoke test: publish with PublishAot=true and run the native executable.
// Uses only AOT-safe resolvers (generated + builtin + enum); referencing StandardResolver here
// would produce IL2026/IL3050 warnings, which the build treats as errors.
static class Program
{
    static int failures;

    static int Main()
    {
        Console.WriteLine("IsDynamicCodeSupported=" + RuntimeFeature.IsDynamicCodeSupported);

        var resolver = CompositeResolver.Create(
            new IJsonFormatter[0],
            new IJsonFormatterResolver[] { AotSmoke.Resolvers.GeneratedResolver.Instance, BuiltinResolver.Instance, EnumResolver.Default });

        var order = new Order
        {
            Id = 42,
            Customer = "ACME",
            Status = Status.Active,
            PreviousStatus = Status.Retired,
            Total = 12.5m,
            Created = new DateTime(2026, 11, 15, 10, 30, 0, DateTimeKind.Utc),
            Priority = null,
            Tags = new List<Guid> { new Guid("6f9619ff-8b86-d011-b42d-00cf4fc964ff") },
            Counters = new Dictionary<string, long> { { "views", 1L << 40 } },
            Numbers = new[] { 1, 2, 3 },
            Lines = new List<OrderLine> { new OrderLine { Sku = "A-1", Quantity = 2, Price = 3.25 } },
            ShipTo = new Address("Springfield", "12345"),
        };

        // explicit resolver
        var json = JsonSerializer.ToJsonString(order, resolver);
        Console.WriteLine(json);
        var back = JsonSerializer.Deserialize<Order>(json, resolver);
        Check("Id", back.Id, 42);
        Check("Customer (DataMember name)", back.Customer, "ACME");
        Check("json uses DataMember name", json.Contains("\"customer\":\"ACME\""), true);
        Check("Status", back.Status, Status.Active);
        Check("PreviousStatus (Nullable<enum>)", back.PreviousStatus, (Status?)Status.Retired);
        Check("Total", back.Total, 12.5m);
        Check("Created", back.Created, order.Created);
        Check("Priority (null)", back.Priority, (int?)null);
        Check("Tags (List<Guid>)", back.Tags[0], order.Tags[0]);
        Check("Counters (Dictionary<string,long>)", back.Counters["views"], 1L << 40);
        Check("Numbers", string.Join(",", back.Numbers), "1,2,3");
        Check("Lines", back.Lines[0].Sku + "x" + back.Lines[0].Quantity + "@" + back.Lines[0].Price, "A-1x2@3.25");
        Check("ShipTo (struct, constructor)", back.ShipTo.City + " " + back.ShipTo.Zip, "Springfield 12345");

        // builtins and enums through the same resolver
        Check("int", JsonSerializer.ToJsonString(5, resolver), "5");
        Check("string[]", JsonSerializer.ToJsonString(new[] { "a", "b" }, resolver), "[\"a\",\"b\"]");
        Check("enum by name", JsonSerializer.ToJsonString(Status.Retired, resolver), "\"Retired\"");
        Check("enum by value", JsonSerializer.ToJsonString(Status.Retired, EnumResolver.UnderlyingValue), "2");

        // the README recipe: register once, then use the resolver-less API
        CompositeResolver.RegisterAndSetAsDefault(AotSmoke.Resolvers.GeneratedResolver.Instance, BuiltinResolver.Instance, EnumResolver.Default);
        Check("default resolver roundtrip", JsonSerializer.Deserialize<Order>(JsonSerializer.Serialize(order)).Lines[0].Price, 3.25);

        Console.WriteLine(failures == 0 ? "PASS" : "FAIL: " + failures);
        return failures == 0 ? 0 : 1;
    }

    static void Check<T>(string name, T actual, T expected)
    {
        var ok = EqualityComparer<T>.Default.Equals(actual, expected);
        if (!ok) failures++;
        Console.WriteLine((ok ? "OK    " : "FAIL  ") + name + " => " + actual + (ok ? "" : " (expected " + expected + ")"));
    }
}
