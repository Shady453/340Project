using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public Gun Gun;

    public void Shoot()
    {
        Gun.Shoot();
    }
}
