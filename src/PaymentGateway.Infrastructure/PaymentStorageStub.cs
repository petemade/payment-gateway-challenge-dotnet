using System.Collections.Concurrent;

using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Infrastructure;

internal static class PaymentStorageStub
{
    private static readonly IDictionary<Guid, Payment> Storage = new ConcurrentDictionary<Guid, Payment>();

    internal static Guid Store(Payment payment)
    {
        Guid id = Guid.NewGuid();
        Storage.Add(id, payment);
        return id;
    }

    internal static Payment? Retrieve(Guid id)
    {
        return Storage.ContainsKey(id) ? Storage[id] : null;
    }
}