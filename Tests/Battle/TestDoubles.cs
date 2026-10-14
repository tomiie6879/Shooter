using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public class MonoBehaviour {}
    public struct Vector3 {}
    public class Transform { public Vector3 position; }
    public static class Mathf { public static int Min(int a,int b) => Math.Min(a,b); }
}
public class EnemyData {}
public class PlayerHealth { public bool IsAlive = true; }
public class SpawnSchedule
{
    public EnemyData enemy; public float startTime,endTime,interval;
    public int countPerSpawn,maxAlive,spawnPointIndex=-1;
}
public class BossSchedule
{
    public EnemyData enemy; public float spawnTime; public int spawnPointIndex=-1;
}
public class LevelData
{
    public float duration;
    public SpawnSchedule[] spawnSchedules;
    public BossSchedule boss;
    public bool HasBoss => boss != null && boss.enemy != null;
}
public class EnemyHealth
{
    public event Action<EnemyHealth> Died, Despawned;
    public void Die() { Died?.Invoke(this); Despawn(); }
    public void Despawn() { Despawned?.Invoke(this); Died=null; Despawned=null; }
}
public class CombatPoolManager
{
    public bool IsInitialized = true;
    public int Capacity=100;
    public readonly List<EnemyHealth> Spawned = new List<EnemyHealth>();
    public int AvailableEnemies(EnemyData data) => Capacity-Spawned.Count;
    public bool TrySpawnEnemy(EnemyData data,UnityEngine.Vector3 position,PlayerHealth player,out EnemyHealth enemy)
    {
        enemy=null;
        if (AvailableEnemies(data)<=0) return false;
        enemy=new EnemyHealth(); Spawned.Add(enemy); return true;
    }
}
public static class BattleChecks
{
    static void Check(bool ok,string name) { if(!ok) throw new Exception(name); }
    public static string Run()
    {
        var p=new BattleProgress(120,false);
        p.Tick(119); Check(!p.CanWin,"no early victory");
        p.Tick(2); Check(p.CanWin && p.Remaining==0,"no-boss time completion");
        p=new BattleProgress(120,true);
        p.MarkBossDefeated(); p.Tick(120); Check(!p.CanWin,"unspawned boss cannot complete");
        p.MarkBossSpawned(); Check(!p.CanWin,"alive boss overtime");
        p.MarkBossDefeated(); Check(p.CanWin,"late boss victory");
        p=new BattleProgress(120,true); p.MarkBossSpawned(); p.MarkBossDefeated();
        Check(!p.CanWin,"early boss still waits"); p.Tick(120); Check(p.CanWin,"early boss time completion");

        var data=new EnemyData();
        var level=new LevelData { duration=10, spawnSchedules=new[]{
            new SpawnSchedule {enemy=data,startTime=2,endTime=8,interval=2,countPerSpawn=2,maxAlive=3}
        }};
        var pool=new CombatPoolManager(); var spawner=new EnemySpawner();
        spawner.Initialize(pool,new PlayerHealth(),new[]{new UnityEngine.Transform()});
        spawner.BeginBattle(level); spawner.Tick(0); Check(pool.Spawned.Count==0,"start gate");
        spawner.Tick(2); Check(pool.Spawned.Count==2,"batch size");
        spawner.Tick(3); Check(pool.Spawned.Count==2,"interval gate");
        spawner.Tick(4); Check(pool.Spawned.Count==3,"alive cap");
        pool.Spawned[0].Despawn(); spawner.Tick(6); Check(pool.Spawned.Count==4,"despawn releases cap");
        spawner.Tick(8); Check(pool.Spawned.Count==4,"end exclusive");

        level.boss=new BossSchedule {enemy=data,spawnTime=10};
        level.spawnSchedules=Array.Empty<SpawnSchedule>();
        pool=new CombatPoolManager {Capacity=0};
        spawner.Initialize(pool,new PlayerHealth(),new[]{new UnityEngine.Transform()});
        int born=0,dead=0;
        spawner.BossSpawned+=()=>born++; spawner.BossDefeated+=()=>dead++;
        spawner.BeginBattle(level); spawner.Tick(10); Check(born==0,"full pool not a boss spawn");
        pool.Capacity=1; spawner.Tick(10); spawner.Tick(10); Check(born==1,"boss retries and spawns once");
        pool.Spawned[0].Despawn(); Check(dead==0,"despawn is not boss death");
        spawner.BeginBattle(level); pool.Spawned.Clear(); spawner.Tick(10);
        pool.Spawned[0].Die(); Check(dead==1,"boss death event");
        spawner.StopSpawning(); spawner.Tick(10); Check(born==2,"stop prevents spawning");

        pool=new CombatPoolManager {Capacity=2};
        level.boss.spawnTime=9;
        level.spawnSchedules=new[]{ new SpawnSchedule {enemy=data,startTime=0,endTime=10,interval=1,countPerSpawn=5,maxAlive=5}};
        spawner.Initialize(pool,new PlayerHealth(),new[]{new UnityEngine.Transform()});
        spawner.BeginBattle(level); spawner.Tick(0); Check(pool.Spawned.Count==1,"reserve boss capacity");
        spawner.Tick(9); Check(pool.Spawned.Count==2,"boss gets reserved capacity");
        return "PASS: battle progress / spawn lifecycle checks.";
    }
}
