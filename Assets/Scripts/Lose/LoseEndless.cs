using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseEndless : MonoBehaviour
{
    [Header("Animator Settings (Optional)")]
    [SerializeField] private Animator animator;

    [Header("GameObjects To Activate After Delay (Optional)")]
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    [Header("GameObjects To Disable On Lose (Optional)")]
    [SerializeField] private List<GameObject> objectsToDisableOnLose = new List<GameObject>();

    [Header("Delay Settings")]
    [SerializeField] private float activationDelay = 2f;

    [Header("Enemy Layer Mask (Required)")]
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("Pause Delay After Trigger (Seconds)")]
    [SerializeField] private float pauseDelay = 7f;

    private bool hasTriggered = false;
    private bool hasSaved = false;

    private void Start()
    {
        if (objectsToActivate != null && objectsToActivate.Count > 0)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isEnemy = (enemyLayerMask.value & (1 << other.gameObject.layer)) != 0;

        if (!hasTriggered && isEnemy)
        {
            hasTriggered = true;

            WaveManager.isGameOver = true;

            Debug.Log("[LoseEndless] Triggered by: " + other.gameObject.name);

            // ✅ Penting: pastikan nucleus final dihitung SEBELUM autosave (score & rank)
            ForceFinalNucleusCountNow();

            TryAutoSave();

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopBgmLoop();
                Debug.Log("[LoseEndless] BGM stopped immediately.");
            }

            DisableObjectsOnLose();

            if (animator != null)
            {
                animator.Play("DeathAnimation");
                Debug.Log("[LoseEndless] DeathAnimation played");
            }

            StartCoroutine(ActivateObjectsAfterDelay());
            StartCoroutine(PauseAfterDelay());
        }
    }

    private void ForceFinalNucleusCountNow()
    {
        var counter = Object.FindFirstObjectByType<ListCounterManager>();
        if (counter != null)
        {
            counter.ForceFinalCount();
        }
        else
        {
            Debug.LogWarning("[LoseEndless] ListCounterManager not found. Nucleus may be outdated.");
        }
    }

    private void TryAutoSave()
    {
        if (hasSaved) return;
        hasSaved = true;

        // ✅ Sumber nama utama: UserManager.CurrentUser
        // fallback: GameData.Data.PlayerName
        // fallback terakhir: "Player"
        string playerName =
            (UserManager.Instance != null && !string.IsNullOrEmpty(UserManager.Instance.CurrentUser))
                ? UserManager.Instance.CurrentUser
                : (!string.IsNullOrEmpty(GameData.Data.PlayerName) ? GameData.Data.PlayerName : "Player");

        // tetap sinkron ke GameData biar UI lain yang pakai GameData aman
        GameData.Data.PlayerName = playerName;

        int flags   = GameData.Data.CurrentFlags;
        int suns    = GameData.Data.FinalSuns;

        // ✅ nucleus sudah di-force sebelum TryAutoSave dipanggil
        int nucleus = GameData.Data.TotalActiveObjects;

        int score = (nucleus * 3) + (suns * 1) + (flags * 10);
        GameData.Data.FinalScore = score;

        Debug.Log($"[LoseEndless] Autosave Score = Flags:{flags}, Suns:{suns}, Nucleus:{nucleus}, Score:{score}");

        int rank = HighscoreTable.Insert(
            playerName,
            flags,
            suns,
            nucleus,
            score
        );

        GameData.Data.FinalRank = rank;

        Debug.Log($"[LoseEndless] Leaderboard Rank = {rank + 1}");
    }

    private void DisableObjectsOnLose()
    {
        if (objectsToDisableOnLose == null || objectsToDisableOnLose.Count == 0)
            return;

        foreach (GameObject obj in objectsToDisableOnLose)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        Debug.Log("[LoseEndless] Objects disabled.");
    }

    private IEnumerator ActivateObjectsAfterDelay()
    {
        if (objectsToActivate == null || objectsToActivate.Count == 0)
            yield break;

        yield return new WaitForSeconds(activationDelay);

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    private IEnumerator PauseAfterDelay()
    {
        yield return new WaitForSeconds(pauseDelay);

        // (tetap dipertahankan) force final count lagi sebelum pause
        // tidak mengubah behavior yang sudah ada, hanya redundant & aman
        var counter = Object.FindFirstObjectByType<ListCounterManager>();
        if (counter != null)
        {
            counter.ForceFinalCount();
        }

        Time.timeScale = 0f;

        Debug.Log("[LoseEndless] GAME PAUSED");
    }

    public void RetryLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        Time.timeScale = 1f;
        WaveManager.isGameOver = false;

        SceneManager.LoadScene(currentSceneName);
    }
}
