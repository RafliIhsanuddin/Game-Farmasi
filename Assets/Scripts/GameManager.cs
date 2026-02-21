using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private TextMeshProUGUI sunsEndingText;

    private int selectedPrice;

    [Header("Atom Settings")]
    public LayerMask AtomLayer;
    [SerializeField] private Transform atomValueTarget;
    [SerializeField] private float atomMoveSpeed = 6f;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        // =====================================================
        // UPDATE SUN UI (BEHAVIOUR LAMA - TIDAK DIUBAH)
        // =====================================================

        if (sunsText != null)
            sunsText.text = suns.ToString();

        if (sunsEndingText != null)
            sunsEndingText.text = $"final sun value : {suns}";

        GameData.Data.FinalSuns = suns;

        // =====================================================
        // CAMERA SAFETY
        // =====================================================

        if (mainCam == null) return;

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // =====================================================
        // HANDLE ATOM CLICK (BEHAVIOUR LAMA - TIDAK DIUBAH)
        // =====================================================

        HandleAtomClick(mouseWorld);

        // =====================================================
        // BLOCK INTERACTION SAAT PAUSE / GAMEOVER
        // =====================================================

        if (IsInteractionBlocked())
        {
            HidePreview();
            currentTile = null;
            return;
        }

        // =====================================================
        // JIKA BELUM PILIH CAPSULE
        // =====================================================

        if (currentCapsule == null)
        {
            HidePreview();
            return;
        }

        // =====================================================
        // PASTIKAN PREVIEW ADA
        // =====================================================

        if (previewInstance == null)
        {
            CreatePreviewInstance();
        }

        // =====================================================
        // RAYCAST TILE
        // =====================================================

        RaycastHit2D hit =
            Physics2D.Raycast(
                mouseWorld,
                Vector2.zero,
                Mathf.Infinity,
                tileMask
            );

        if (hit.collider)
        {
            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile != null && !tile.hasCapsule)
            {
                currentTile = tile;

                previewInstance.SetActive(true);
                previewInstance.transform.position =
                    tile.transform.position;
            }
            else
            {
                HidePreview();
                currentTile = null;
            }

            // =====================================================
            // PLACE CAPSULE (BEHAVIOUR LAMA + TAMBAHAN COOLDOWN)
            // =====================================================

            if (Input.GetMouseButtonDown(0)
                && currentTile != null
                && !currentTile.hasCapsule)
            {
                if (suns >= selectedPrice)
                {
                    suns -= selectedPrice;

                    GameObject newCapsule =
                        Instantiate(
                            currentCapsule,
                            currentTile.transform.position,
                            Quaternion.identity
                        );

                    newCapsule.transform.SetParent(
                        currentTile.transform
                    );

                    currentTile.hasCapsule = true;
                    currentTile.currentCapsule = newCapsule;

                    AssignOwnerTileToCapsule(
                        newCapsule,
                        currentTile
                    );

                    // =====================================================
                    // NEW (Cooldown Integration)
                    // =====================================================

                    CapsuleSlot slot =
                        FindSlotByCapsule(currentCapsule);

                    if (slot != null)
                    {
                        slot.OnCapsulePlaced();
                    }

                    // =====================================================

                    CancelSelection();
                }
            }
        }
        else
        {
            HidePreview();
            currentTile = null;
        }
    }

    // =====================================================
    // NEW METHOD (Cooldown Integration)
    // =====================================================

    private CapsuleSlot FindSlotByCapsule(GameObject capsule)
    {
        CapsuleSlot[] slots =
            FindObjectsByType<CapsuleSlot>(
                FindObjectsSortMode.None
            );

        foreach (CapsuleSlot slot in slots)
        {
            if (slot.capsuleObject == capsule)
                return slot;
        }

        return null;
    }

    // =====================================================
    // INTERACTION BLOCK CHECK (BEHAVIOUR LAMA)
    // =====================================================

    private bool IsInteractionBlocked()
    {
        if (PauseManager.IsPaused)
            return true;

        if (WaveManager.isGameOver)
            return true;

        return false;
    }

    // =====================================================
    // PREVIEW SYSTEM (BEHAVIOUR LAMA)
    // =====================================================

    private void HidePreview()
    {
        if (previewInstance != null)
            previewInstance.SetActive(false);
    }

    private void CreatePreviewInstance()
    {
        if (currentCapsule == null) return;

        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance =
            Instantiate(currentCapsule);

        previewInstance.name =
            "Preview_" + currentCapsule.name;

        var sr =
            previewInstance.GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.color =
                new Color(1f, 1f, 1f, 0.5f);

        var collider =
            previewInstance.GetComponent<Collider2D>();

        if (collider != null)
            collider.enabled = false;

        previewInstance.SetActive(false);
    }

    // =====================================================
    // SELECT CAPSULE (BEHAVIOUR LAMA)
    // =====================================================

    public void SelectPlant(
        GameObject capsule,
        Sprite sprite,
        int price)
    {
        if (IsInteractionBlocked()) return;

        currentCapsule = capsule;
        currentCapsuleSprite = sprite;
        selectedPrice = price;

        CreatePreviewInstance();
    }

    // =====================================================
    // CANCEL SELECTION (BEHAVIOUR LAMA)
    // =====================================================

    public void CancelSelection()
    {
        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = null;

        currentCapsule = null;
        currentCapsuleSprite = null;
        selectedPrice = 0;
        currentTile = null;
    }

    // =====================================================
    // ASSIGN OWNER TILE (BEHAVIOUR LAMA - TIDAK DIUBAH)
    // =====================================================

    private void AssignOwnerTileToCapsule(
        GameObject capsule,
        Tile tile)
    {
        // Capsule
        if (capsule.TryGetComponent(out CapsuleRed red)) { red.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out CapsuleBlue blue)) { blue.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out CapsuleYellow yellow)) { yellow.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out CapsuleCream cream)) { cream.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out CapsuleGreen green)) { green.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out CapsuleOrange orange)) { orange.ownerTile = tile; return; }

        // Basic Shooter
        if (capsule.TryGetComponent(out BasicShooterBlue shooterBlue)) { shooterBlue.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterYellow shooterYellow)) { shooterYellow.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterRed shooterRed)) { shooterRed.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterCream shooterCream)) { shooterCream.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterGreen shooterGreen)) { shooterGreen.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterOrange shooterOrange)) { shooterOrange.ownerTile = tile; return; }

        // Soldier Shooter
        if (capsule.TryGetComponent(out BasicShooterYellowSoldier yellowSoldier)) { yellowSoldier.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterRedSoldier redSoldier)) { redSoldier.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterGreenSoldier greenSoldier)) { greenSoldier.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterBlueSoldier blueSoldier)) { blueSoldier.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterCreamSoldier creamSoldier)) { creamSoldier.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterOrangeSoldier orangeSoldier)) { orangeSoldier.ownerTile = tile; return; }
        if (capsule.TryGetComponent(out BasicShooterBlackSoldier blackSoldier)) { blackSoldier.ownerTile = tile; return; }

        Debug.LogWarning("[GameManager] Capsule/Shooter tidak dikenali: " + capsule.name);
    }

    // =====================================================
    // ATOM CLICK SYSTEM (BEHAVIOUR LAMA)
    // =====================================================

    private void HandleAtomClick(Vector3 mouseWorld)
    {
        if (IsInteractionBlocked()) return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit =
                Physics2D.Raycast(
                    mouseWorld,
                    Vector2.zero,
                    Mathf.Infinity,
                    AtomLayer
                );

            if (hit.collider != null)
            {
                SoundManager.Instance?.PlayAtomClick();

                StartCoroutine(
                    MoveAtomToTargetAndDestroy(
                        hit.collider.gameObject,
                        atomValueTarget.position
                    )
                );
            }
        }
    }

    private IEnumerator MoveAtomToTargetAndDestroy(
        GameObject atom,
        Vector3 targetPos)
    {
        while (atom != null &&
            Vector3.Distance(
                atom.transform.position,
                targetPos) > 0.05f)
        {
            atom.transform.position =
                Vector3.MoveTowards(
                    atom.transform.position,
                    targetPos,
                    atomMoveSpeed *
                    Time.deltaTime
                );

            yield return null;
        }

        if (atom != null)
        {
            Destroy(atom);

            if (atomRewardValues.Count > 0)
            {
                suns += atomRewardValues[
                    Random.Range(
                        0,
                        atomRewardValues.Count)];
            }
        }
    }
}