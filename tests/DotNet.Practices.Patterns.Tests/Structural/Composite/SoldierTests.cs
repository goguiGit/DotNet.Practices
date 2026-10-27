using DotNet.Practices.Patterns.Structural.Composite;

namespace DotNet.Practices.Patterns.Tests.Structural.Composite;

public class SoldierTests
{
    [Test]
    public void AddSubordinate_WithLowerRank_AddsToSubordinates()
    {
        var sergeant = new Soldier("Smith", Rank.Sergeant);
        var soldier = new Soldier("Jones", Rank.Private);

        sergeant.AddSubordinate(soldier);

        Assert.That(sergeant.Subordinates, Is.EqualTo(new[] { soldier }));
    }

    [Test]
    public void AddSubordinate_WhenCommanderIsPrivate_ThrowsInvalidOperationException()
    {
        var soldier = new Soldier("Jones", Rank.Private);
        var other = new Soldier("Brown", Rank.Private);

        Assert.That(() => soldier.AddSubordinate(other), Throws.InvalidOperationException);
    }

    [Test]
    public void AddSubordinate_WithSameOrHigherRank_ThrowsArgumentException()
    {
        var sergeant = new Soldier("Smith", Rank.Sergeant);
        var captain = new Soldier("Miller", Rank.Captain);

        Assert.That(() => sergeant.AddSubordinate(captain), Throws.ArgumentException);
    }

    [Test]
    public void GiveOrder_ReachesWholeTroopRecursively()
    {
        var captain = new Soldier("Miller", Rank.Captain);
        var sergeant = new Soldier("Smith", Rank.Sergeant);
        var soldier = new Soldier("Jones", Rank.Private);
        captain.AddSubordinate(sergeant);
        sergeant.AddSubordinate(soldier);

        captain.GiveOrder(Order.Shoot);

        Assert.Multiple(() =>
        {
            Assert.That(sergeant.ReceivedOrders, Is.EqualTo(new[] { Order.Shoot }));
            Assert.That(soldier.ReceivedOrders, Is.EqualTo(new[] { Order.Shoot }));
            Assert.That(captain.ReceivedOrders, Is.Empty);
        });
    }

    [Test]
    public void GiveOrder_WhenPrivate_ThrowsInvalidOperationException()
    {
        var soldier = new Soldier("Jones", Rank.Private);

        Assert.That(() => soldier.GiveOrder(Order.Salute), Throws.InvalidOperationException);
    }
}
