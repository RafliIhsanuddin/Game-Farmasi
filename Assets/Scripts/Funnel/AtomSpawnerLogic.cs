using UnityEngine;
using System.Collections.Generic;

public class AtomSpawnerLogic : MonoBehaviour
{
    [SerializeField] private List<FunnelSpawner> funnels;
    [SerializeField] private Vector2 spawnDelay = new Vector2(1f, 3f);

    private bool spawning = true;
    private int lastIndex = -1;

    void Start()
    {
        ScheduleNext();
    }

    void ScheduleNext()
    {
        if (!spawning) return;
        Invoke(nameof(TrySpawn), Random.Range(spawnDelay.x, spawnDelay.y));
    }

    void TrySpawn()
    {
        if (!spawning) return;

        int count = funnels.Count;
        if (count == 0) return;

        int startIndex = (lastIndex + 1) % count;
        int index = startIndex;
        FunnelSpawner chosen = null;

        for (int i = 0; i < count; i++)
        {
            var f = funnels[index];
            if (f.IsAvailable() && index != lastIndex)
            {
                chosen = f;
                break;
            }
            index = (index + 1) % count;
        }

        if (chosen == null)
            chosen = funnels[startIndex];

        lastIndex = funnels.IndexOf(chosen);

        chosen.TriggerSpawnAnimation();
        ScheduleNext();
    }

    public void StopSpawning()
    {
        spawning = false;
        CancelInvoke(nameof(TrySpawn));
    }
}
