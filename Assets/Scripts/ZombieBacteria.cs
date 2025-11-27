using UnityEngine;

public class ZombieBacteria : MonoBehaviour
{
    public float speed;

    public float health;

    private void FixedUpdate()
    {
        transform.position -= new Vector3(speed, 0, 0);
    }
}
