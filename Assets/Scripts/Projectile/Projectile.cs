using UnityEngine;

public class Projectile : MonoBehaviour
{

    public int damage;

    public float speed = 0.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position += new Vector3(speed * Time.fixedDeltaTime, 0f, 0f);
    }
}
