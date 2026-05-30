using StarterAssets;
using UnityEngine;

public class FlowmapController : MonoBehaviour
{
    //private static float PreviousFrameId = Shader.PropertyToID("_PreviousFrame");
    //private static float VelocityId = Shader.PropertyToID("_Velocity");
    //private static float PositionId = Shader.PropertyToID("_Position");
    //private static float SimulationCenterId = Shader.PropertyToID("_SimulationCenter");
    //private static float SimulationSizeId = Shader.PropertyToID("_SimulationSize");
    //private static float PlayerRadiusId = Shader.PropertyToID("_PlayerRadius");
    //private static float PlayerHardnessId = Shader.PropertyToID("_PlayerHardness");
    //private static float FlowmapId = Shader.PropertyToID("_Flowmap");

    private static Color NeutralFlow = new Color(0.5f, 0.5f, 0.5f, 1.0f);

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
    [SerializeField, Min(0.0f)] private float playerRadius = 0.05f;
    [SerializeField, Range(0.0f, 1.0f)] private float playerHardness = 0.5f;

    private Vector3 lastPlayerPosition;
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
   
        lastPlayerPosition = playerPosition;

        simulationMaterial.SetTexture("_PreviousFrame", readBuffer);
        simulationMaterial.SetVector("_Velocity", playerVelocity);
        simulationMaterial.SetVector("_Position", playerPosition);
        simulationMaterial.SetVector("_SimulationCenter", simulationCenter);
        simulationMaterial.SetVector("_SimulationSize", simulationSize);
        simulationMaterial.SetFloat("_PlayerRadius", playerRadius);
        simulationMaterial.SetFloat("_PlayerHardness", playerHardness);

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

        Debug.LogError("FlowmapController is missing one or more references.", this);
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
