using UnityEngine;

public class RedProjectile : MonoBehaviour
{
    public int damage;
    public float speed = 0.0f;

    /*[Header("Rotation")]
    public float rotationSpeed = 360f; // derajat per detik*/
    
    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 25f;
    
    private void Start()
    {
        // ⏱️ hancur otomatis setelah 20 detik
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // ➜ tetap maju ke depan
        transform.position += new Vector3(
            speed * Time.deltaTime,
            0f,
            0f
        );

        /*// ➜ putar di sumbu Z
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);*/
    }
    
    
    
}
