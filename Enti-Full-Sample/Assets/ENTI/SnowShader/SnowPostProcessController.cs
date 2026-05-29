using UnityEngine;

public sealed class SnowPostProcessController : MonoBehaviour
{
    private static readonly int SnowAmountId = Shader.PropertyToID("_SnowAmount");

    [SerializeField] private Material snowMaterial;
    [SerializeField, Range(0f, 1f)] private float snowPivotAmount = 0.35f;
    [SerializeField, Min(0.01f)] private float normalRiseDurationToPivot = 5f;
    [SerializeField, Min(0.01f)] private float slowRiseDurationToFull = 25f;
    [SerializeField, Min(0.01f)] private float slowFallDurationToPivot = 20f;
    [SerializeField, Min(0.01f)] private float normalFallDurationToZero = 5f;
    [SerializeField] private string stencilLayerName = "SnowStencilMask";
    [SerializeField] private string[] stencilMaskObjectNames = { "Cube_physics" };

    private float elapsedTime;

    private void Awake()
    {
        ApplyStencilLayer();
        SetSnowAmount(0f);
    }

    private void OnEnable()
    {
        elapsedTime = 0f;
        ApplyStencilLayer();
        SetSnowAmount(0f);
    }

    private void OnDisable()
    {
        SetSnowAmount(0f);
    }

    private void Update()
    {
        if (snowMaterial == null || !Application.isPlaying)
            return;

        elapsedTime += Time.deltaTime;
        SetSnowAmount(EvaluateSnowAmount(elapsedTime));
    }

    private void OnValidate()
    {
        snowPivotAmount = Mathf.Clamp01(snowPivotAmount);
        normalRiseDurationToPivot = Mathf.Max(0.01f, normalRiseDurationToPivot);
        slowRiseDurationToFull = Mathf.Max(0.01f, slowRiseDurationToFull);
        slowFallDurationToPivot = Mathf.Max(0.01f, slowFallDurationToPivot);
        normalFallDurationToZero = Mathf.Max(0.01f, normalFallDurationToZero);
    }

    private float EvaluateSnowAmount(float time)
    {
        float climbDuration = normalRiseDurationToPivot + slowRiseDurationToFull;
        float fallDuration = slowFallDurationToPivot + normalFallDurationToZero;
        float loopTime = Mathf.Repeat(time, climbDuration + fallDuration);

        if (loopTime < climbDuration)
            return EvaluateRisingSnow(loopTime);

        return EvaluateFallingSnow(loopTime - climbDuration);
    }

    private float EvaluateRisingSnow(float time)
    {
        if (time < normalRiseDurationToPivot)
            return Mathf.Lerp(0f, snowPivotAmount, time / normalRiseDurationToPivot);

        float slowPhase = (time - normalRiseDurationToPivot) / slowRiseDurationToFull;
        return Mathf.Lerp(snowPivotAmount, 1f, slowPhase);
    }

    private float EvaluateFallingSnow(float time)
    {
        if (time < slowFallDurationToPivot)
            return Mathf.Lerp(1f, snowPivotAmount, time / slowFallDurationToPivot);

        float normalPhase = (time - slowFallDurationToPivot) / normalFallDurationToZero;
        return Mathf.Lerp(snowPivotAmount, 0f, normalPhase);
    }

    private void SetSnowAmount(float amount)
    {
        if (snowMaterial != null)
            snowMaterial.SetFloat(SnowAmountId, Mathf.Clamp01(amount));
    }

    private void ApplyStencilLayer()
    {
        int layer = LayerMask.NameToLayer(stencilLayerName);
        if (layer < 0 || stencilMaskObjectNames == null)
            return;

        foreach (string objectName in stencilMaskObjectNames)
        {
            if (string.IsNullOrWhiteSpace(objectName))
                continue;

            GameObject root = GameObject.Find(objectName);
            if (root == null)
                continue;

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                child.gameObject.layer = layer;
        }
    }
}
