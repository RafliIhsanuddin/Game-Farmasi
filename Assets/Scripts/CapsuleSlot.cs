using UnityEngine;
using UnityEngine.UI;


public class CapsuleSlot : MonoBehaviour
{

    public Sprite capsuleSprite;

    public GameObject capsuleObject;

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
        }
        else
        {
            icon.enabled = false;
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    private void BuyPlant()
    {
        
    }

    public Image icon;

    // Update is called once per frame
    void Update()
    {
        
    }
}
