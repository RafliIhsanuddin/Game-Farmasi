using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Capsule Data")]
    public GameObject currentCapsule;       // prefab kapsul asli
    public Sprite currentCapsuleSprite;     // sprite kapsul
    private GameObject previewInstance;     // instance preview dinamis

    [Header("Tile References")]
    public Transform tiles; // parent semua tile
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

        // Hapus preview lama (kalau masih ada)
        if (previewInstance != null)
            Destroy(previewInstance);

        // Buat preview baru dari prefab kapsul yang sama
        previewInstance = Instantiate(currentCapsule);
        previewInstance.name = "Preview_" + capsule.name;

        // Bikin transparan supaya terlihat beda dari kapsul asli
        var sr = previewInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(1f, 1f, 1f, 0.5f);

        // Pastikan tidak interaktif
        var collider = previewInstance.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        previewInstance.SetActive(false);

        Debug.Log("Capsule selected: " + capsule.name + " (dynamic preview created)");
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

            // Klik kiri untuk menanam kapsul
            if (Input.GetMouseButtonDown(0) && currentTile && !currentTile.hasCapsule)
            {
                GameObject newCapsule = Instantiate(currentCapsule, currentTile.transform.position, Quaternion.identity);
                newCapsule.transform.SetParent(currentTile.transform);
                currentTile.hasCapsule = true;

                Debug.Log("Planted capsule on tile: " + currentTile.name);

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
