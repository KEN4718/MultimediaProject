using UnityEngine;

public class BackgroundEffect : MonoBehaviour
{
    public RectTransform background; // 绑定背景图片的 RectTransform
    public float amplitude = 5f; // 晃动的幅度
    public float frequency = 1f; // 晃动的频率

    private Vector3 originalPosition;

    void Start()
    {
        if (background != null)
        {
            originalPosition = background.anchoredPosition;
        }
    }

    void Update()
    {
        if (background != null)
        {
            float offsetX = Mathf.Sin(Time.time * frequency) * amplitude;
            float offsetY = Mathf.Cos(Time.time * frequency * 0.5f) * amplitude;
            background.anchoredPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
        }
    }
}
