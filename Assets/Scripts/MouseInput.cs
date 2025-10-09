using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public Player Player;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Player.Shoot();
        }
    }
}
