using UnityEngine;

public class Atom : MonoBehaviour
{
    private Transform[] possiblePositions; // otomatis diisi lewat pencarian
    private float dropToYPos;
    [SerializeField] private float speed = 15f;
    
    private Camera mainCam;

    void Start()
    {
        // Cari GameObject dengan nama yang sudah ditentukan
        GameObject spawn1 = GameObject.Find("Spawn Atom (1)");
        GameObject spawn2 = GameObject.Find("Spawn Atom (2)");
        GameObject spawn3 = GameObject.Find("Spawn Atom (3)");

        // Simpan ke array jika ditemukan
        possiblePositions = new Transform[] 
        { 
            spawn1 != null ? spawn1.transform : null,
            spawn2 != null ? spawn2.transform : null,
            spawn3 != null ? spawn3.transform : null
        };

        // Filter null (kalau ada nama yang tidak ditemukan)
        possiblePositions = System.Array.FindAll(possiblePositions, p => p != null);

        if (possiblePositions.Length > 0)
        {
            // Pilih salah satu Transform secara acak
            Transform randomTransform = possiblePositions[Random.Range(0, possiblePositions.Length)];

            // Pindahkan posisi Atom ke posisi transform itu
            transform.position = randomTransform.position;

            // Tentukan posisi Y akhir secara acak
            dropToYPos = Random.Range(2f, -3f);
        }
        else
        {
            Debug.LogWarning("Tidak ditemukan GameObject bernama 'Spawn Atom (1–3)' di scene!");
        }
        
        Destroy(gameObject, Random.Range(15f, 30f));
    }

    private void Update()
    {
        if (transform.position.y >= dropToYPos)
        {
            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
        }
        
    }
    
}
