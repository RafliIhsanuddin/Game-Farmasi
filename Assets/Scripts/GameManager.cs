using UnityEngine;

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
        if (currentCapsule == null)
        {
            if (previewInstance != null)
                previewInstance.SetActive(false);
            return;
        }

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

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
    
    
}
