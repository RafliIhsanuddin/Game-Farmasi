using UnityEngine;

public class EnemyGameObjectManager : MonoBehaviour
{
    [Header("All Enemy GameObjects")]
    [SerializeField] private GameObject defaultEnemy;
    [SerializeField] private GameObject purpleEnemy;
    [SerializeField] private GameObject redEnemy;
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
            purpleEnemy,
            redEnemy,
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

    public void ShowDefaultEnemy()   => ShowOnly(defaultEnemy);
    public void ShowPurpleEnemy()    => ShowOnly(purpleEnemy);
    public void ShowRedEnemy()       => ShowOnly(redEnemy);
    public void ShowFungiEnemy()     => ShowOnly(fungiEnemy);
    public void ShowVirusEnemy()     => ShowOnly(virusEnemy);
    public void ShowProtozoaEnemy()  => ShowOnly(protozoaEnemy);
    public void ShowHelminthEnemy()  => ShowOnly(helminthEnemy);
}
