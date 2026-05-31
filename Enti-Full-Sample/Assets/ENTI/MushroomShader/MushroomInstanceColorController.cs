using UnityEngine;

public class MushroomInstanceColorController : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string mushroomTag = "Mushroom";

    [Header("Random color range")]
    [SerializeField] private Color minColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color maxColor = new Color(1f, 0.3f, 0.3f, 1f);

    private static readonly int InstanceColorID = Shader.PropertyToID("_InstanceColor");

    private void Start()
    {
        GameObject[] mushrooms = GameObject.FindGameObjectsWithTag(mushroomTag);

        foreach (GameObject mushroom in mushrooms)
        {
            if (mushroom == null) continue;

            Renderer r = mushroom.GetComponent<Renderer>();
            if (r == null) continue;

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);

            Color randomColor = new Color(
                Random.Range(minColor.r, maxColor.r),
                Random.Range(minColor.g, maxColor.g),
                Random.Range(minColor.b, maxColor.b),
                1f
            );

            block.SetColor(InstanceColorID, randomColor);
            r.SetPropertyBlock(block);
        }
    }
}