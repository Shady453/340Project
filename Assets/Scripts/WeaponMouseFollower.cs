using UnityEngine;

public class WeaponMouseFollower : MonoBehaviour
{

    //public SpriteRenderer HandSpriteRenderer;
    //public SpriteRenderer WeaponSpriteRenderer;

    private float RotationOffsetDegrees = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWeaponRotation();
        
        // flip weapon sprite if needed
    }

    private void UpdateWeaponRotation()
    {
        // get angle between object and mouse
        Vector3 mousePosition = Input.mousePosition;
        //print("Mouse position is " + mousePosition);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //print("Mouse world is at " + mouseWorldPosition);
        mouseWorldPosition.z = 0f;
        Vector3 direction = mouseWorldPosition - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //+ RotationOffsetDegrees;
        
        // rotate object
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }
}
