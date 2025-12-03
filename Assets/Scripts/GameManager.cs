using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Capsule Data")]
    public GameObject currentCapsule;       // Prefab kapsul yang dipilih player
    public Sprite currentCapsuleSprite;     // Sprite kapsul untuk UI/preview
    private GameObject previewInstance;     // Instance preview kapsul yang mengikuti kursor

    [Header("Tile References")]
    public Transform tiles; // Parent dari semua tile
    public LayerMask tileMask;

    private Camera mainCam;
    private Tile currentTile;

    [Header("Suns Data")]
    public int suns;
    public TextMeshProUGUI sunsText;

    [Header("Atom Settings")]
    public LayerMask AtomLayer;
    [SerializeField] private Transform atomValueTarget;
    [SerializeField] private float atomMoveSpeed = 6f;

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
            collider.enabled = false; // nonaktifkan collider agar tidak interaktif

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

            // Klik kiri untuk menanam capsule
            if (Input.GetMouseButtonDown(0) && currentTile && !currentTile.hasCapsule)
            {
                GameObject newCapsule = Instantiate(currentCapsule, currentTile.transform.position, Quaternion.identity);
                newCapsule.transform.SetParent(currentTile.transform);

                // Tandai tile sudah terisi kapsul
                currentTile.hasCapsule = true;
                currentTile.currentCapsule = newCapsule;

                // 🔥 BAGIAN PENTING: deteksi tipe kapsul dan isi ownerTile
                AssignOwnerTileToCapsule(newCapsule, currentTile);

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

    /// <summary>
    /// Mengecek tipe kapsul (Merah, Biru, Kuning) lalu set ownerTile-nya.
    /// </summary>
    private void AssignOwnerTileToCapsule(GameObject capsule, Tile tile)
    {
        var red = capsule.GetComponent<CapsuleRed>();
        if (red != null)
        {
            red.ownerTile = tile;
            Debug.Log($"[GameManager] CapsuleRed di {tile.name} terdaftar.");
            return;
        }

        var blue = capsule.GetComponent<CapsuleBlue>();
        if (blue != null)
        {
            blue.ownerTile = tile;
            Debug.Log($"[GameManager] CapsuleBlue di {tile.name} terdaftar.");
            return;
        }

        var yellow = capsule.GetComponent<CapsuleYellow>();
        if (yellow != null)
        {
            yellow.ownerTile = tile;
            Debug.Log($"[GameManager] CapsuleYellow di {tile.name} terdaftar.");
            return;
        }

        Debug.LogWarning($"[GameManager] Tidak ada skrip Capsule ditemukan di {capsule.name}");
    }

    private void HandleAtomClick(Vector3 mouseWorld)
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D atomHit = Physics2D.Raycast(mouseWorld, Vector2.zero, Mathf.Infinity, AtomLayer);
            if (atomHit.collider != null)
            {
                GameObject atom = atomHit.collider.gameObject;
                Debug.Log("Atom clicked: " + atom.name);

                if (atomValueTarget != null)
                {
                    StartCoroutine(MoveAtomToTargetAndDestroy(atom, atomValueTarget.position));
                }
                else
                {
                    Debug.LogWarning("Atom Value target belum di-assign!");
                    Destroy(atom);
                }
            }
        }
    }

    private IEnumerator MoveAtomToTargetAndDestroy(GameObject atom, Vector3 targetPos)
    {
        while (atom != null && Vector3.Distance(atom.transform.position, targetPos) > 0.05f)
        {
            atom.transform.position = Vector3.MoveTowards(atom.transform.position, targetPos, atomMoveSpeed * Time.deltaTime);
            yield return null;
        }

        if (atom != null)
        {
            Destroy(atom);
            int[] possibleValues = { 10, 20, 30 };
            int randomValue = possibleValues[Random.Range(0, possibleValues.Length)];
            suns += randomValue;
            Debug.Log($"Atom reached Atom Value and destroyed! +{randomValue}");
        }
    }
    
    
}
