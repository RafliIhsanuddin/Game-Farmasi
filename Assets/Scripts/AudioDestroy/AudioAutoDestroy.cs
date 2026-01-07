using UnityEngine;

public class AudioAutoDestroy : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 3f;

    private void Start()
    {
        Destroy(gameObject, destroyDelay);
    }
}
