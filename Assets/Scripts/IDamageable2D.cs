using UnityEngine;

// "interface" = a contract saying any class that implements it
// MUST have the listed functions.
public interface IDamageable2D
{
    // Called when something wants to deal damage.
    // 'amount' is how much damage, 'hitPoint' is where it hit, 'hitDir' is the direction.
    void TakeDamage(float amount, Vector2 hitPoint, Vector2 hitDir);
}