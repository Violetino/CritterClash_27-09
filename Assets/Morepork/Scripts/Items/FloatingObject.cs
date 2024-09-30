using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float amplitude = 0.5f;      // How much the object moves up and down
    public float frequency = 1f;        // The speed of the floating motion
    public float rotationSpeedX = 50f;  // Speed of the rotation around the X-axis in degrees per second
    public float rotationSpeedY = 30f;  // Speed of the rotation around the Y-axis in degrees per second
    public float rotationSpeedZ = 10f;  // Speed of the rotation around the Z-axis in degrees per second

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Floating motion
        Vector3 newPosition = startPosition;
        newPosition.y += Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = newPosition;

        // Rotation motion around each axis
        transform.Rotate(Vector3.right, rotationSpeedX * Time.deltaTime); // X-axis rotation
        transform.Rotate(Vector3.up, rotationSpeedY * Time.deltaTime);   // Y-axis rotation
        transform.Rotate(Vector3.forward, rotationSpeedZ * Time.deltaTime); // Z-axis rotation
    }
}


