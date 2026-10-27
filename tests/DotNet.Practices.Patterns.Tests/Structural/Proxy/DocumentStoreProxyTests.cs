using DotNet.Practices.Patterns.Structural.Proxy.Interfaces;
using DotNet.Practices.Patterns.Structural.Proxy.Proxies;
using DotNet.Practices.Patterns.Structural.Proxy.Stores;

namespace DotNet.Practices.Patterns.Tests.Structural.Proxy;

public class DocumentStoreProxyTests
{
    private static ArchivedDocumentStore StoreWithPayslip()
    {
        var store = new ArchivedDocumentStore();
        store.Store("Ana-payslip", "Payslip Ana\nNet: 1600.00");
        return store;
    }

    [Test]
    public void CachingDocumentStoreProxy_Read_ReturnsTheArchivedDocument()
    {
        IDocumentStore proxy = new CachingDocumentStoreProxy(StoreWithPayslip());

        Assert.That(proxy.Read("Ana-payslip"), Is.EqualTo("Payslip Ana\nNet: 1600.00"));
    }

    [Test]
    public void CachingDocumentStoreProxy_Read_LoadsFromTheArchiveOnlyOnce()
    {
        var store = StoreWithPayslip();
        IDocumentStore proxy = new CachingDocumentStoreProxy(store);

        proxy.Read("Ana-payslip");
        proxy.Read("Ana-payslip");

        Assert.That(store.Loads, Is.EqualTo(1));
    }

    [Test]
    public void ProtectedDocumentStoreProxy_Read_PassesThroughForHr()
    {
        IDocumentStore proxy = new ProtectedDocumentStoreProxy(StoreWithPayslip(), role: "HR");

        Assert.That(proxy.Read("Ana-payslip"), Is.EqualTo("Payslip Ana\nNet: 1600.00"));
    }

    [Test]
    public void ProtectedDocumentStoreProxy_Read_ThrowsForAnyOtherRole()
    {
        IDocumentStore proxy = new ProtectedDocumentStoreProxy(StoreWithPayslip(), role: "Sales");

        Assert.That(() => proxy.Read("Ana-payslip"), Throws.TypeOf<UnauthorizedAccessException>());
    }
}
