using UnityEngine;

public class ShooterAnimationEventBlue : MonoBehaviour
{
    [SerializeField] private BasicShooterBlueSoldier shooter;

    // DIPANGGIL DARI ANIMATION EVENT
    public void FireProjectile()
    {
        if (shooter != null)
            shooter.FireProjectileFromAnimation();
    }
}
