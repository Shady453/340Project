using UnityEngine;

public class WeaponMouseFollower : MonoBehaviour
{

    public SpriteRenderer HandSpriteRenderer;
    public SpriteRenderer WeaponSpriteRenderer;

    private float RotationOffsetDegrees = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // get angle between object and mouse
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 direction = mouseWorldPosition - HandSpriteRenderer.transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + RotationOffsetDegrees;
        
        // rotate object
        HandSpriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        
        // flip weapon sprite if needed
    }
}
