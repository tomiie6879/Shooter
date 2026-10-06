using UnityEngine;

public sealed class MapReferences : MonoBehaviour
{
    public Transform playerSpawn;
    public Transform[] enemySpawnPoints;

    public bool IsValid()
    {
        if (playerSpawn == null ||
            enemySpawnPoints == null ||
            enemySpawnPoints.Length == 0)
        {
            return false;
        }

        foreach (Transform point in enemySpawnPoints)
        {
            if (point == null)
                return false;
        }

        return true;
    }
}