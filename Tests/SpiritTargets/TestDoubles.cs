using System;
namespace UnityEngine {}
public sealed class TestObject { public bool activeInHierarchy = true; }
public sealed class EnemyHealth
{
    public bool IsAlive = true;
    public TestObject gameObject = new TestObject();
    public event Action<EnemyHealth> Died, Despawned;
    public void Die() { IsAlive = false; Died?.Invoke(this); }
    public void Return() { gameObject.activeInHierarchy = false; Despawned?.Invoke(this); Died = null; Despawned = null; }
}
public static class SpiritTargetChecks
{
    static void Check(bool condition, string name) { if (!condition) throw new Exception(name); }
    public static string Run()
    {
        var c = new SpiritTargetCoordinator();
        var owners = new object[]{new object(),new object(),new object(),new object()};
        var enemies = new[]{new EnemyHealth(),new EnemyHealth()};
        foreach (var owner in owners)
        {
            var target = c.CountOthers(owner,enemies[0]) <= c.CountOthers(owner,enemies[1]) ? enemies[0] : enemies[1];
            c.Assign(owner,target);
        }
        Check(c.CountOthers(new object(),enemies[0])==2,"2 enemies share four reservations evenly");
        Check(c.CountOthers(owners[0],enemies[0])==1,"exclude own reservation");
        c.Assign(owners[0],enemies[0]);
        Check(c.CountOthers(new object(),enemies[0])==2,"idempotent reservation");
        enemies[0].Die();
        Check(!c.IsAssigned(owners[0],enemies[0]),"death removes reservation immediately");
        enemies[1].Return();
        Check(c.CountOthers(new object(),enemies[1])==0,"pool return removes reservations");
        enemies[1].gameObject.activeInHierarchy=true;
        Check(!c.IsAssigned(owners[1],enemies[1]),"new lease does not inherit old reservation");
        c.Assign(owners[0],enemies[1]);
        c.Release(owners[0]);
        Check(c.CountOthers(new object(),enemies[1])==0,"disable releases reservation");
        foreach (var owner in owners) c.Assign(owner,enemies[1]);
        Check(c.CountOthers(new object(),enemies[1])==4,"single enemy can be shared");
        c.Clear();
        Check(c.CountOthers(new object(),enemies[1])==0,"battle stop clears reservations");
        c.Assign(owners[0],enemies[0]);
        Check(!c.IsAssigned(owners[0],enemies[0]),"dead enemy rejected");
        c.Assign(owners[0],null);
        return "PASS: spirit target reservation lifecycle.";
    }
}
