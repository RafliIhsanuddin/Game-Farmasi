using UnityEngine;

public class Atom : MonoBehaviour
{
    public enum AtomSource { Funnel, Random, Bacteria }
    public AtomSource source = AtomSource.Random;

    // 👇 DIBALIKIN BUAT BACTERIA
    public bool useRandomSpawn = true;

    [Header("Movement")]
    [SerializeField] private float speed = 15f;

    [Header("Lifetime")]
    [SerializeField] private float minLifeTime = 15f;
    [SerializeField] private float maxLifeTime = 30f;

    private float dropToYPos;

    private void Start()
    {
        // bacteria masih jalan seperti semula
        if (source == AtomSource.Bacteria)
        {
            SetupDrop();
            return;
        }

        // random spawner masih bisa
        if (useRandomSpawn && source == AtomSource.Random)
        {
            SetupRandomSpawn();
            SetupDrop();
            return;
        }

        // funnel drop
        if (source == AtomSource.Funnel)
        {
            SetupDrop();
            return;
        }
    }

    private void SetupRandomSpawn()
    {
        GameObject spawn1 = GameObject.Find("Spawn Atom (1)");
        GameObject spawn2 = GameObject.Find("Spawn Atom (2)");
        GameObject spawn3 = GameObject.Find("Spawn Atom (3)");

        Transform[] slots =
        {
            spawn1 ? spawn1.transform : null,
            spawn2 ? spawn2.transform : null,
            spawn3 ? spawn3.transform : null
        };

        slots = System.Array.FindAll(slots, s => s != null);

        if (slots.Length > 0)
            transform.position = slots[Random.Range(0, slots.Length)].position;
    }

    private void SetupDrop()
    {
        dropToYPos = Random.Range(2f, -3f);

        float lifeTime = Random.Range(minLifeTime, maxLifeTime);
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (transform.position.y >= dropToYPos)
            transform.position -= Vector3.up * speed * Time.deltaTime;
    }
    
}
