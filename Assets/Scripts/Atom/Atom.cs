using UnityEngine;

public class Atom : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 15f;

    [Header("Spawn Mode")]
    public bool useRandomSpawn = true;
    
    [Header("Lifetime")]
    [SerializeField] private float minLifeTime = 15f;
    [SerializeField] private float maxLifeTime = 30f;
    
    private Transform[] possiblePositions;
    private float dropToYPos;

    private void Start()
    {
        // ===============================
        // MODE 1: RANDOM SPAWN (AtomSpawner)
        // ===============================
        if (useRandomSpawn)
        {
            GameObject spawn1 = GameObject.Find("Spawn Atom (1)");
            GameObject spawn2 = GameObject.Find("Spawn Atom (2)");
            GameObject spawn3 = GameObject.Find("Spawn Atom (3)");

            possiblePositions = new Transform[]
            {
                spawn1 ? spawn1.transform : null,
                spawn2 ? spawn2.transform : null,
                spawn3 ? spawn3.transform : null
            };

            possiblePositions = System.Array.FindAll(possiblePositions, p => p != null);

            if (possiblePositions.Length > 0)
            {
                Transform randomTransform =
                    possiblePositions[Random.Range(0, possiblePositions.Length)];

                transform.position = randomTransform.position;
            }
            else
            {
                Debug.LogWarning("[Atom] Spawn Atom (1–3) tidak ditemukan di scene");
            }
        }

        // ===============================
        // MODE 2: DROP DARI BAKTERI
        // (posisi SUDAH DISET dari luar)
        // ===============================
        dropToYPos = Random.Range(2f, -3f);

        // ===============================
        // AUTO DESTROY (CLEANUP)
        // ===============================
        float lifeTime = Random.Range(minLifeTime, maxLifeTime);
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (transform.position.y >= dropToYPos)
        {
            transform.position -= Vector3.up * speed * Time.deltaTime;
        }
    }
    
}
