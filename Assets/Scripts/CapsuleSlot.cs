using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CapsuleSlot : MonoBehaviour
{

    public Sprite capsuleSprite;

    public GameObject capsuleObject;

    public int Price;
    
    public TextMeshProUGUI priceText;

    private GameManager gms;

    private void Start()
    {
        gms = GameObject.Find("GameManager").GetComponent<GameManager>();
        GetComponent<Button>().onClick.AddListener(BuyPlant);
    }

    private void OnValidate()
    {
        if (capsuleSprite)
        {
            icon.enabled = true;
            icon.sprite = capsuleSprite;
            priceText.text = Price.ToString();
        }
        else
        {
            icon.enabled = false;
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    private void BuyPlant()
    {
        if (gms.suns >= Price && !gms.currentCapsule)
        {
            gms.suns -= Price;
            Debug.Log("CapsuleSlot clicked! Buying plant: " + capsuleObject.name);
            gms.BuyPlant(capsuleObject, capsuleSprite);
        }
    }

    public Image icon;

    // Update is called once per frame
    void Update()
    {
        
    }
}
