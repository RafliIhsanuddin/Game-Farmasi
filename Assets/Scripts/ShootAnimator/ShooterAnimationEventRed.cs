using UnityEngine;

public class ShooterAnimationEventRed : MonoBehaviour
{
    [SerializeField] private BasicShooterRedSoldier shooter;

    // DIPANGGIL DARI ANIMATION EVENT
    public void FireProjectile()
    {
        if (shooter != null)
            shooter.FireProjectileFromAnimation();
    }
}
