using DotNet.Practices.SqlErrors.TransientFaults;

namespace DotNet.Practices.SqlErrors.Tests.TransientFaults;

public class TransientSqlErrorsTests
{
    [TestCase(40613, true)]
    [TestCase(40501, true)]
    [TestCase(49918, true)]
    [TestCase(2627, false)]
    [TestCase(1205, false)]
    [TestCase(-2, false)]
    public void TransientSqlErrors_Contains_ClassifiesTheNumber(int number, bool expected)
    {
        Assert.That(TransientSqlErrors.Contains(number), Is.EqualTo(expected));
    }
}
