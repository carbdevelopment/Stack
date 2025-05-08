using UnityEngine;

public class GradientSkyboxScript : MonoBehaviour
{
    [Header("Gradient Settings")]
    [Range(0,1)] public float bottomLevel = 0f;     
    [Range(0,1)] public float topLevel = 1f;        
    [Range(0.1f,5f)] public float blendPower = 1f;       
    public float transitionSpeed = 0.5f;

    [Header("Light Factors")]
    [Range(0,1)] public float bottomLightFactor = 0.3f;
    [Range(0,1)] public float topLightFactor = 0.5f;

    public Material skyboxMaterial;
    private Color currentBottomColor;
    private Color currentTopColor;

    void Start()
    {
        if (skyboxMaterial == null)
            skyboxMaterial = new Material(Shader.Find("Custom/GradientSkybox"));

        Color topBase = ColorManager.Instance.GetCurrentSkyboxTopColor();
        Color bottomAlt = ColorManager.Instance.GetAlternateSkyboxBottomColor();

        currentTopColor = Color.Lerp(topBase, Color.white, topLightFactor);
        currentBottomColor = Color.Lerp(bottomAlt, Color.white, bottomLightFactor);

        UpdateMaterial();
        RenderSettings.skybox = skyboxMaterial;
    }

    void Update()
    {
        Color targetTop = Color.Lerp(ColorManager.Instance.GetCurrentSkyboxTopColor(), Color.white, topLightFactor);
        Color targetBottom = Color.Lerp(ColorManager.Instance.GetAlternateSkyboxBottomColor(), Color.white, bottomLightFactor);

        currentTopColor = Color.Lerp(currentTopColor, targetTop, transitionSpeed * Time.deltaTime);
        currentBottomColor = Color.Lerp(currentBottomColor, targetBottom, transitionSpeed * Time.deltaTime);

        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        if (skyboxMaterial == null)
            return;

        skyboxMaterial.SetColor("_TopColor", currentTopColor);
        skyboxMaterial.SetColor("_BottomColor", currentBottomColor);
        skyboxMaterial.SetFloat("_BottomLevel", bottomLevel);
        skyboxMaterial.SetFloat("_TopLevel", topLevel);
        skyboxMaterial.SetFloat("_BlendPower", blendPower);
    }
}