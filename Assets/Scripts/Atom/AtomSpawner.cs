using UnityEngine;

public class AtomSpawner : MonoBehaviour
{
    public GameObject atomPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnAtom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnAtom()
    {
        Instantiate(atomPrefab);
        Invoke("SpawnAtom", Random.Range(3, 5));
    }
}
