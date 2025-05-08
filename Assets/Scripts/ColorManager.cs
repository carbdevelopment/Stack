using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    [Header("Reference Colors")]
    public Color[] referenceColors;

    [Header("Palette Generation Settings")]
    public int paletteSize = 10;
    public float hueVariation = 0.1f;

    private Color[] currentPalette;
    private int currentIndex = 0;

    private void Awake() => Instance = this;

    public void GenerateNewPalette()
    {
        if (referenceColors == null || referenceColors.Length == 0)
        {
            currentPalette = new Color[] { Color.white };
            currentIndex = 0;
            return;
        }

        currentPalette = new Color[paletteSize];
        Color baseColor = referenceColors[Random.Range(0, referenceColors.Length)];
        currentPalette[0] = baseColor;

        Color.RGBToHSV(baseColor, out float h, out float s, out float v);

        for (int i = 1; i < paletteSize; i++)
        {
            float newH = Mathf.Repeat(h + Random.Range(-hueVariation, hueVariation), 1f);
            float newS = Mathf.Clamp01(s + Random.Range(-0.1f, 0.1f));
            float newV = Mathf.Clamp01(v + Random.Range(-0.1f, 0.1f));
            currentPalette[i] = Color.HSVToRGB(newH, newS, newV);
        }

        for (int i = 1; i < paletteSize; i++)
        {
            int j = Random.Range(1, paletteSize);
            Color temp = currentPalette[i];
            currentPalette[i] = currentPalette[j];
            currentPalette[j] = temp;
        }

        currentIndex = 0;
    }

    public Color GetNextBlockColor()
    {
        if (currentPalette == null || currentPalette.Length == 0)
            GenerateNewPalette();

        Color col = currentPalette[currentIndex];
        currentIndex++;
        if (currentIndex >= currentPalette.Length)
        {
            currentIndex = 0;
            GenerateNewPalette();
        }
        return col;
    }

    public Color GetCurrentSkyboxBottomColor()
    {
        if (currentPalette == null || currentPalette.Length == 0)
            GenerateNewPalette();
        return currentPalette[0];
    }

    public Color GetCurrentSkyboxTopColor() => Color.Lerp(GetCurrentSkyboxBottomColor(), Color.white, 0.5f);

    public Color GetAlternateSkyboxBottomColor()
    {
        if (currentPalette == null || currentPalette.Length < 2)
            GenerateNewPalette();
        return currentPalette.Length >= 2 ? currentPalette[1] : currentPalette[0];
    }
}