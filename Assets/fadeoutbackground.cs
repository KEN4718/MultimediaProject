using UnityEngine;
using UnityEngine.UI;

public class FadeOutBackground : MonoBehaviour
{
    public Image backgroundImage;  // 需要淡出的背景图像
    public float fadeDuration = 2f;  // 淡出持续时间

    private float currentTime = 0f;
    private Color initialColor;

    void Start()
    {
        if (backgroundImage != null)
        {
            // 获取背景的初始颜色
            initialColor = backgroundImage.color;

            // 开始执行淡出效果
            StartCoroutine(FadeOut());
        }
        else
        {
            Debug.LogError("未设置背景图像，请在Inspector中设置！");
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration); // 计算透明度
            backgroundImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha); // 设置颜色
            yield return null; // 等待下一帧
        }

        // 确保完全透明
        backgroundImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
    }
}
