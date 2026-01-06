using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Capsule Data")]
    public GameObject currentCapsule;
    public Sprite currentCapsuleSprite;
    private GameObject previewInstance;

    [Header("Tile References")]
    public Transform tiles;
    public LayerMask tileMask;

    private Camera mainCam;
    private Tile currentTile;
    
    [Header("Atom Reward Values")]
    [SerializeField] private List<int> atomRewardValues = new List<int>()
    {
        10,
        20,
        30
    };


    [Header("Suns Data")]
    public int suns;
    public TextMeshProUGUI sunsText;
    private int selectedPrice;

    [Header("Atom Settings")]
    public LayerMask AtomLayer;
    [SerializeField] private Transform atomValueTarget;
    [SerializeField] private float atomMoveSpeed = 6f;

    void Start()
    {
        mainCam = Camera.main;
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

            if (tile != null && !tile.hasCapsule)
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

            if (Input.GetMouseButtonDown(0) && currentTile != null && !currentTile.hasCapsule)
            {
                if (suns >= selectedPrice)
                {
                    suns -= selectedPrice;

                    GameObject newCapsule = Instantiate(
                        currentCapsule,
                        currentTile.transform.position,
                        Quaternion.identity
                    );

                    newCapsule.transform.SetParent(currentTile.transform);

                    currentTile.hasCapsule = true;
                    currentTile.currentCapsule = newCapsule;

                    AssignOwnerTileToCapsule(newCapsule, currentTile);
                    CancelSelection();
                }
            }
        }
        else
        {
            if (previewInstance != null)
                previewInstance.SetActive(false);
            currentTile = null;
        }
    }

    // ===============================
    // UI PLANT SELECTION
    // ===============================
    public void SelectPlant(GameObject capsule, Sprite sprite, int price)
    {
        currentCapsule = capsule;
        currentCapsuleSprite = sprite;
        selectedPrice = price;

        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = Instantiate(currentCapsule);
        previewInstance.name = "Preview_" + capsule.name;

        var sr = previewInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(1f, 1f, 1f, 0.5f);

        var collider = previewInstance.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        previewInstance.SetActive(false);
    }

    public void CancelSelection()
    {
        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = null;
        currentCapsule = null;
        currentCapsuleSprite = null;
        selectedPrice = 0;
    }

    // ===============================
    // ASSIGN OWNER TILE (LENGKAP)
    // ===============================
    private void AssignOwnerTileToCapsule(GameObject capsule, Tile tile)
    {
        if (capsule.TryGetComponent(out CapsuleRed red))
        {
            red.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out CapsuleBlue blue))
        {
            blue.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out CapsuleYellow yellow))
        {
            yellow.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out CapsuleCream cream))
        {
            cream.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out CapsuleGreen green))
        {
            green.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out CapsuleOrange orange))
        {
            orange.ownerTile = tile;
            return;
        }

        // ================================
        // 🔵 TAMBAHAN PENTING UNTUK SHOOTER
        // ================================
        if (capsule.TryGetComponent(out BasicShooterBlue shooterBlue))
        {
            shooterBlue.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out BasicShooterYellow shooterYellow))
        {
            shooterYellow.ownerTile = tile;
            return;
        }
        
        if (capsule.TryGetComponent(out BasicShooterRed shooterRed))
        {
            shooterRed.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out BasicShooterCream shooterCream))
        {
            shooterCream.ownerTile = tile;
            return;
        }
        
        // ================================
        // 🟢 BASIC SHOOTER GREEN
        // ================================
        if (capsule.TryGetComponent(out BasicShooterGreen shooterGreen))
        {
            shooterGreen.ownerTile = tile;
            return;
        }

        // ================================
        // 🟧 BASIC SHOOTER ORANGE
        // ================================
        if (capsule.TryGetComponent(out BasicShooterOrange shooterOrange))
        {
            shooterOrange.ownerTile = tile;
            return;
        }
        
        // ===============================
        // 🟡 SHOOTER SOLDIER (FINAL SYSTEM)
        // ===============================
        if (capsule.TryGetComponent(out BasicShooterYellowSoldier yellowSoldier))
        {
            yellowSoldier.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out BasicShooterRedSoldier redSoldier))
        {
            redSoldier.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out BasicShooterGreenSoldier greenSoldier))
        {
            greenSoldier.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out BasicShooterBlueSoldier blueSoldier))
        {
            blueSoldier.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out BasicShooterCreamSoldier creamSoldier))
        {
            creamSoldier.ownerTile = tile;
            return;
        }

        if (capsule.TryGetComponent(out BasicShooterOrangeSoldier orangeSoldier))
        {
            orangeSoldier.ownerTile = tile;
            return;
        }

        // ===============================
        // FALLBACK WARNING
        // ===============================
        Debug.LogWarning($"[GameManager] Capsule/Soldier tidak dikenali: {capsule.name}");
    }

    // ===============================
    // ATOM HANDLING
    // ===============================
    private void HandleAtomClick(Vector3 mouseWorld)
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero, Mathf.Infinity, AtomLayer);
            if (hit.collider != null)
            {
                StartCoroutine(
                    MoveAtomToTargetAndDestroy(hit.collider.gameObject, atomValueTarget.position)
                );
            }
        }
    }

    private IEnumerator MoveAtomToTargetAndDestroy(GameObject atom, Vector3 targetPos)
    {
        while (atom != null && Vector3.Distance(atom.transform.position, targetPos) > 0.05f)
        {
            atom.transform.position = Vector3.MoveTowards(
                atom.transform.position,
                targetPos,
                atomMoveSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (atom != null)
        {
            Destroy(atom);

            // ======== MODIFIKASI DI SINI SAJA ========
            if (atomRewardValues.Count > 0)
            {
                suns += atomRewardValues[Random.Range(0, atomRewardValues.Count)];
            }
        }
    }
    
    
}
