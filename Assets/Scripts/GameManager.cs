using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject currentCapsule;
    
    public Sprite currentCapsuleSprite;

    public Transform tiles;
    
    public LayerMask tileMask;





    public void BuyPlant(GameObject capsule, Sprite sprite)
    {
        currentCapsule = capsule;
        currentCapsuleSprite = sprite;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, tileMask);
        
        
        foreach(Transform tile in tiles)
            tile.GetComponent<SpriteRenderer>().enabled = false;


        if (hit.collider && currentCapsule)
        {
            hit.collider.GetComponent<SpriteRenderer>().sprite = currentCapsuleSprite;
            hit.collider.GetComponent<SpriteRenderer>().enabled = true;
        }
        
        
        
    }
    
    
    
    
}
