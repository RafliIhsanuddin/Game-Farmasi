using UnityEngine;

public class ShooterAnimationEventGreen : MonoBehaviour
{
    [SerializeField] private BasicShooterGreenSoldier shooter;

    // DIPANGGIL DARI ANIMATION EVENT
    public void FireProjectile()
    {
        if (shooter != null)
            shooter.FireProjectileFromAnimation();
    }
}
