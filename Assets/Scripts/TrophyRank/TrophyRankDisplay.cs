using UnityEngine;
using UnityEngine.UI;

public class TrophyRankDisplay : MonoBehaviour
{
    [Header("Target GameObject (yang memiliki komponen Image)")]
    [SerializeField] private GameObject trophyObject;

    [Header("Trophy Sprites (Rank 1 - 10)")]
    [SerializeField] private Sprite trophy1;
    [SerializeField] private Sprite trophy2;
    [SerializeField] private Sprite trophy3;
    [SerializeField] private Sprite trophy4;
    [SerializeField] private Sprite trophy5;
    [SerializeField] private Sprite trophy6;
    [SerializeField] private Sprite trophy7;
    [SerializeField] private Sprite trophy8;
    [SerializeField] private Sprite trophy9;
    [SerializeField] private Sprite trophy10;

    private Image trophyImage;

    private void Awake()
    {
        if (trophyObject == null)
        {
            Debug.LogError("[TrophyRankDisplay] trophyObject belum di assign!");
            return;
        }

        trophyImage = trophyObject.GetComponent<Image>();

        if (trophyImage == null)
        {
            Debug.LogError("[TrophyRankDisplay] GameObject tidak memiliki komponen Image!");
        }
    }

    private void Start()
    {
        UpdateTrophy();
    }

    public void UpdateTrophy()
    {
        if (trophyObject == null || trophyImage == null)
            return;

        // ambil rank dari GameData
        int rank = GameData.Data.FinalRank + 1;

        Debug.Log("[TrophyRankDisplay] Rank Player = " + rank);

        // jika rank lebih dari 10 → nonaktifkan object
        if (rank > 10 || rank <= 0)
        {
            trophyObject.SetActive(false);
            Debug.Log("[TrophyRankDisplay] Rank > 10 → Trophy disembunyikan");
            return;
        }

        trophyObject.SetActive(true);

        switch (rank)
        {
            case 1:
                trophyImage.sprite = trophy1;
                break;

            case 2:
                trophyImage.sprite = trophy2;
                break;

            case 3:
                trophyImage.sprite = trophy3;
                break;

            case 4:
                trophyImage.sprite = trophy4;
                break;

            case 5:
                trophyImage.sprite = trophy5;
                break;

            case 6:
                trophyImage.sprite = trophy6;
                break;

            case 7:
                trophyImage.sprite = trophy7;
                break;

            case 8:
                trophyImage.sprite = trophy8;
                break;

            case 9:
                trophyImage.sprite = trophy9;
                break;

            case 10:
                trophyImage.sprite = trophy10;
                break;
        }

        Debug.Log("[TrophyRankDisplay] Trophy sprite updated untuk rank " + rank);
    }
}