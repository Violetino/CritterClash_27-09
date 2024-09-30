using UnityEngine;

public class SkyboxOffsetController : MonoBehaviour
{
    public Material skyboxMaterial; // Reference to the skybox material
    public Vector2 offsetSpeed = new Vector2(0.1f, 0.1f); // Speed of offset change per axis

    private Vector2 currentOffset;

    void Start()
    {
        if (skyboxMaterial == null)
        {
            Debug.LogError("Skybox material is not assigned!");
            return;
        }

        // Initialize currentOffset to the material's current texture offset
        currentOffset = skyboxMaterial.GetTextureOffset("_MainTex");
    }

    void Update()
    {
        // Calculate new offset based on time and speed
        currentOffset += offsetSpeed * Time.deltaTime;

        // Apply the offset to the material
        skyboxMaterial.SetTextureOffset("_MainTex", currentOffset);
    }
}

