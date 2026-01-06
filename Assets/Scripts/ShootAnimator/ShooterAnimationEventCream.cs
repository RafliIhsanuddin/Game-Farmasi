using UnityEngine;

public class ShooterAnimationEventCream : MonoBehaviour
{
    [SerializeField] private BasicShooterCreamSoldier shooter;

    // DIPANGGIL DARI ANIMATION EVENT
    public void FireProjectile()
    {
        if (shooter != null)
            shooter.FireProjectileFromAnimation();
    }
}
