using UnityEngine;

public class ShooterAnimationEventYellow : MonoBehaviour
{
    [SerializeField] private BasicShooterYellowSoldier shooter;

    // DIPANGGIL DARI ANIMATION EVENT
    public void FireProjectile()
    {
        if (shooter != null)
            shooter.FireProjectileFromAnimation();
    }
}
