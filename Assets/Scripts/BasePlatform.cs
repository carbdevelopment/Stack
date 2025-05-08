using UnityEngine;

public class BasePlatform : MonoBehaviour
{
    void Start()
    {
        if (ColorManager.Instance != null)
        {
            Color baseColor = ColorManager.Instance.GetCurrentSkyboxBottomColor();
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if(mr != null)
                mr.material.color = baseColor;
        }
    }
}