using UnityEngine;

public class MushroomGlobalsController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float affectRadius = 1.5f;
    [SerializeField] private float affectContrast = 1f;
    [SerializeField] private float affectIntensity = 0.5f;

    private static readonly int PlayerPositionID = Shader.PropertyToID("_PlayerPos");
    private static readonly int AffectRadiusID = Shader.PropertyToID("_AffectRadius");
    private static readonly int AffectContrastID = Shader.PropertyToID("_AffectContrast");
    private static readonly int AffectIntensityID = Shader.PropertyToID("_AffectIntensity");

    private void Update()
    {
        if (player == null) return;

        Vector3 p = player.position;

        Shader.SetGlobalVector(PlayerPositionID, new Vector4(p.x, p.y, p.z, 0f));
        Shader.SetGlobalFloat(AffectRadiusID, affectRadius);
        Shader.SetGlobalFloat(AffectContrastID, affectContrast);
        Shader.SetGlobalFloat(AffectIntensityID, affectIntensity);
    }
}