using UnityEngine;

public class ShooterAnimationEventOrange : MonoBehaviour
{
    [SerializeField] private BasicShooterOrangeSoldier shooter;

    // DIPANGGIL DARI ANIMATION EVENT
    public void FireProjectile()
    {
        if (shooter != null)
            shooter.FireProjectileFromAnimation();
    }
}
