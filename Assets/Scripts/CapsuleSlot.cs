using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CapsuleSlot : MonoBehaviour
{

    [Header("Slot Data")]
        public Sprite capsuleSprite;
        public GameObject capsuleObject;
        public int Price;
    
        [Header("UI References")]
        public TextMeshProUGUI priceText;
        public Image icon;
    
        private GameManager gms;
    
        private void Start()
        {
            gms = GameObject.Find("GameManager").GetComponent<GameManager>();
            GetComponent<Button>().onClick.AddListener(SelectPlant);
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
    
        /// <summary>
        /// Saat slot diklik, hanya memilih kapsul (tanpa mengurangi suns)
        /// </summary>
        private void SelectPlant()
        {
            if (gms.currentCapsule == capsuleObject)
            {
                // Jika klik ulang slot yang sama → batal pilih
                gms.CancelSelection();
                Debug.Log("Selection canceled for: " + capsuleObject.name);
            }
            else
            {
                // Pilih kapsul baru (tanpa potong suns)
                gms.SelectPlant(capsuleObject, capsuleSprite, Price);
                Debug.Log("CapsuleSlot selected: " + capsuleObject.name);
            }
        }
}
