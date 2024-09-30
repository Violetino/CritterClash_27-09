using UnityEngine;

public class FaceObjectToCam : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Try to get the main camera at the start
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null)
        {
            // Try to find the main camera again in case it wasn't available at the start
            mainCamera = Camera.main;

            // If it's still null, exit the update to avoid null reference
            if (mainCamera == null)
                return;
        }

        // Only proceed if the camera is found and active
        if (mainCamera.gameObject.activeInHierarchy)
        {
            transform.LookAt(mainCamera.transform.position);
        }
    }
}
