using StarterAssets;
using UnityEngine;

public class FlowmapController : MonoBehaviour
{
    private static Color NeutralFlow = new Color(0.5f, 0.5f, 0.0f, 1.0f);

    [Header("References")]
    [SerializeField] private ThirdPersonController playerController;
    [SerializeField] private Material simulationMaterial;
    [SerializeField] private Material waterMaterial;
    [SerializeField] private RenderTexture bufferA;
    [SerializeField] private RenderTexture bufferB;
    [SerializeField] private Transform plane;

    [Header("Simulation Area")]
    [SerializeField] private Vector3 simulationCenter = new Vector3(183f, 8.66f, 203f);
    [SerializeField] private Vector3 simulationSize = new Vector3(5.0f, 1.0f, 5.0f);

    [Header("Player Influence")]
    [SerializeField, Min(0.0f)] private float playerRadius;
    [SerializeField, Range(0.0f, 1.0f)] private float playerHardness;

    private Vector3 lastPlayerPosition;
    private Vector3 lastPlanarVelocity;
    private RenderTexture readBuffer;
    private RenderTexture writeBuffer;

    private void Start()
    {
        if (!CanSimulate())
        {
            enabled = false;
            return;
        }

        readBuffer = bufferA;
        writeBuffer = bufferB;
        simulationCenter = plane.position;
        simulationSize = new Vector3(5.0f, 1.0f, 5.0f);
        ClearBuffer(bufferA);
        ClearBuffer(bufferB);

        lastPlayerPosition = playerController.transform.position;
        waterMaterial.SetTexture("_Flowmap", readBuffer);
    }

    private void Update()
    {
        if (!CanSimulate())
        {
            return;
        }

        Vector3 playerPosition = playerController.transform.position;
        Vector3 playerVelocity = playerPosition - lastPlayerPosition;
        Vector3 planarVelocity = new Vector3(-playerVelocity.x, 0.0f, -playerVelocity.z);
        bool isPlayerMoving = planarVelocity.sqrMagnitude > 0.000001f;

        if (isPlayerMoving)
        {
            lastPlanarVelocity = planarVelocity.normalized;
        }
   
        lastPlayerPosition = playerPosition;

        simulationMaterial.SetTexture("_PreviousFrame", readBuffer);
        simulationMaterial.SetVector("_Velocity", lastPlanarVelocity);
        simulationMaterial.SetVector("_Position", playerPosition);
        simulationMaterial.SetVector("_SimulationCenter", simulationCenter);
        simulationMaterial.SetVector("_SimulationSize", simulationSize);

        Graphics.Blit(readBuffer, writeBuffer, simulationMaterial);

        (readBuffer, writeBuffer) = (writeBuffer, readBuffer);
        waterMaterial.SetTexture("_Flowmap", readBuffer);
    }

    private bool CanSimulate()
    {
        if (playerController != null &&
            simulationMaterial != null &&
            waterMaterial != null &&
            bufferA != null &&
            bufferB != null)
        {
            return true;
        }

        return false;
    }

    private static void ClearBuffer(RenderTexture buffer)
    {
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = buffer;
        GL.Clear(false, true, NeutralFlow);
        RenderTexture.active = previousActive;
    }
}
