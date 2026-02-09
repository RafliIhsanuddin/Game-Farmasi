using UnityEngine;

public class EnemyGameObjectManager : MonoBehaviour
{
    [Header("All Enemy GameObjects")]
    [SerializeField] private GameObject defaultEnemy;

    // =====================
    // BACTERIA
    // =====================
    [SerializeField] private GameObject bacteriaPurple;
    [SerializeField] private GameObject bacteriaRed;
    [SerializeField] private GameObject bacteriaYellow;
    [SerializeField] private GameObject bacteriaOrange;
    [SerializeField] private GameObject bacteriaPink;
    [SerializeField] private GameObject bacteriaBlue;
    [SerializeField] private GameObject bacteriaGreen;

    // =====================
    // OTHER ENEMIES
    // =====================
    [SerializeField] private GameObject fungiEnemy;
    [SerializeField] private GameObject virusEnemy;
    [SerializeField] private GameObject protozoaEnemy;
    [SerializeField] private GameObject helminthEnemy;

    [Header("Settings")]
    [SerializeField] private bool showDefaultOnStart = true;

    private GameObject[] allEnemies;

    private void Start()
    {
        allEnemies = new GameObject[]
        {
            defaultEnemy,

            bacteriaPurple,
            bacteriaRed,
            bacteriaYellow,
            bacteriaOrange,
            bacteriaPink,
            bacteriaBlue,
            bacteriaGreen,

            fungiEnemy,
            virusEnemy,
            protozoaEnemy,
            helminthEnemy
        };

        if (showDefaultOnStart)
        {
            ShowDefaultEnemy();
        }
    }

    private void ShowOnly(GameObject enemyToShow)
    {
        foreach (var enemy in allEnemies)
        {
            if (enemy != null)
                enemy.SetActive(enemy == enemyToShow);
        }
    }

    // =====================
    // DEFAULT
    // =====================
    public void ShowDefaultEnemy() => ShowOnly(defaultEnemy);

    // =====================
    // BACTERIA SHOW
    // =====================
    public void ShowBacteriaPurple() => ShowOnly(bacteriaPurple);
    public void ShowBacteriaRed()    => ShowOnly(bacteriaRed);
    public void ShowBacteriaYellow() => ShowOnly(bacteriaYellow);
    public void ShowBacteriaOrange() => ShowOnly(bacteriaOrange);
    public void ShowBacteriaPink()   => ShowOnly(bacteriaPink);
    public void ShowBacteriaBlue()   => ShowOnly(bacteriaBlue);
    public void ShowBacteriaGreen()  => ShowOnly(bacteriaGreen);

    // =====================
    // OTHER ENEMY SHOW
    // =====================
    public void ShowFungiEnemy()    => ShowOnly(fungiEnemy);
    public void ShowVirusEnemy()    => ShowOnly(virusEnemy);
    public void ShowProtozoaEnemy() => ShowOnly(protozoaEnemy);
    public void ShowHelminthEnemy() => ShowOnly(helminthEnemy);
}
