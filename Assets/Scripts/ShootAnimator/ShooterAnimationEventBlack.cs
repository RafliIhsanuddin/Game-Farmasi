using UnityEngine;

public class ShooterAnimationEventBlack : MonoBehaviour
{
    [SerializeField] private BasicShooterBlackSoldier shooter;

    // DIPANGGIL DARI ANIMATION EVENT
    public void FireProjectile()
    {
        if (shooter != null)
            shooter.FireProjectileFromAnimation();
    }
}