using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Capsule Data")]
    public GameObject currentCapsule;       // Prefab kapsul asli yang dipilih player
    public Sprite currentCapsuleSprite;     // Sprite kapsul untuk UI/preview
    private GameObject previewInstance;     // Dinamis preview instance kapsul

    [Header("Tile References")]
    public Transform tiles; // Parent dari semua tile
    public LayerMask tileMask;

    private Camera mainCam;
    private Tile currentTile;

    public int suns;
    
    public TextMeshProUGUI sunsText;


    public LayerMask AtomLayer;

    void Start()
    {
        mainCam = Camera.main;
        Debug.Log("GameManager started. Main camera found: " + (mainCam != null));
    }

    public void BuyPlant(GameObject capsule, Sprite sprite)
    {
        currentCapsule = capsule;
        currentCapsuleSprite = sprite;

        // Hapus preview sebelumnya
        if (previewInstance != null)
            Destroy(previewInstance);

        // Buat preview kapsul untuk mengikuti kursor
        previewInstance = Instantiate(currentCapsule);
        previewInstance.name = "Preview_" + capsule.name;

        var sr = previewInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(1f, 1f, 1f, 0.5f); // transparan

        var collider = previewInstance.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false; // tidak boleh interaktif

        previewInstance.SetActive(false);
    }

    void Update()
    {
        
        sunsText.text = suns.ToString();
        
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        
        
        
        HandleAtomClick(mouseWorld);
        
        
        if (currentCapsule == null)
        {
            if (previewInstance != null)
                previewInstance.SetActive(false);
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero, Mathf.Infinity, tileMask);

        if (hit.collider)
        {
            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile && !tile.hasCapsule)
            {
                currentTile = tile;
                previewInstance.SetActive(true);
                previewInstance.transform.position = tile.transform.position;
            }
            else
            {
                previewInstance.SetActive(false);
                currentTile = null;
            }

            // Klik kiri untuk menanam capsules
            if (Input.GetMouseButtonDown(0) && currentTile && !currentTile.hasCapsule)
            {
                GameObject newCapsule = Instantiate(currentCapsule, currentTile.transform.position, Quaternion.identity);
                newCapsule.transform.SetParent(currentTile.transform);

                // Set data tile
                currentTile.hasCapsule = true;
                currentTile.currentCapsule = newCapsule;

                // Beri referensi tile ke kapsul
                CapsuleRed capsuleScript = newCapsule.GetComponent<CapsuleRed>();
                if (capsuleScript != null)
                {
                    capsuleScript.ownerTile = currentTile;
                }

                // Reset sistem setelah menanam
                Destroy(previewInstance);
                previewInstance = null;
                currentCapsule = null;
                currentCapsuleSprite = null;
                currentTile = null;
            }
        }
        else
        {
            if (previewInstance != null)
                previewInstance.SetActive(false);
            currentTile = null;
        }


       

    }
    
    
    private void HandleAtomClick(Vector3 mouseWorld)
    {
        // gambar garis ray di Scene view biar bisa kelihatan arah klik
        Debug.DrawRay(mouseWorld, Vector3.forward * 10f, Color.yellow, 1f);
        //Debug.Log("Raycast fired from mouseWorld: " + mouseWorld);

        // cek apakah tombol kiri ditekan
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse click detected, checking raycast...");

            RaycastHit2D atomHit = Physics2D.Raycast(mouseWorld, Vector2.zero, Mathf.Infinity, AtomLayer);

            if (atomHit.collider != null)
            {
                Debug.Log("✅ Raycast HIT object: " + atomHit.collider.name + " on layer: " + LayerMask.LayerToName(atomHit.collider.gameObject.layer));
                Destroy(atomHit.collider.gameObject);
                Debug.Log("💥 Atom destroyed by GameManager!");
            }
            else
            {
                Debug.Log("❌ Raycast missed! No collider found on AtomLayer.");
            }
        }
    }
    
    
}
