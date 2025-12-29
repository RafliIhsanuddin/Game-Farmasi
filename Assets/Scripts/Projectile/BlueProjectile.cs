using System;
using UnityEngine;

public class BlueProjectile : MonoBehaviour
{
    public int damage;
    public float speed = 0.0f;

    [Header("Rotation")]
    public float rotationSpeed = 360f; // derajat per detik
    
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

        // ➜ putar di sumbu Z
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<BacteriaControllerRed>(out var red))
        {
            red.ProjectileDead();
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerPurple>(out var purple))
        {
            purple.ProjectileHit(damage);
            Destroy(gameObject);
            return;
        }
    }
}
