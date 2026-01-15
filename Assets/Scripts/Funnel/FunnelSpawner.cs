using UnityEngine;

public class FunnelSpawner : MonoBehaviour
{
    public Animator animator;
    public Transform spawnPoint;
    public GameObject atomPrefab;

    [Header("Cooldown Settings")]
    public float cooldownDuration = 1.0f;
    private float nextAvailableTime = 0f;
    private bool spawnRequested = false;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            Transform sp = transform.Find("SpawnPoint");
            if (sp != null) spawnPoint = sp;
        }

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public bool IsAvailable()
    {
        return Time.time >= nextAvailableTime;
    }

    public void TriggerSpawnAnimation()
    {
        nextAvailableTime = Time.time + cooldownDuration;
        animator.SetTrigger("Spawn");
    }

    public void OnSpawnPointEvent()
    {
        spawnRequested = true;
    }

    private void LateUpdate()
    {
        if (!spawnRequested) return;
        spawnRequested = false;

        if (spawnPoint == null) return;

        Vector3 pos = spawnPoint.position;

        var atom = Instantiate(atomPrefab, pos, Quaternion.identity);

        Atom a = atom.GetComponent<Atom>();
        if (a != null)
            a.source = Atom.AtomSource.Funnel;

        Debug.Log($"[FunnelSpawner] Spawn at {pos}");
    }
}
