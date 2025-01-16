using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Localization;

public class FadeInTextAndImage : MonoBehaviour
{
    public CanvasGroup imageCanvasGroup;   // 图片的 CanvasGroup
    public CanvasGroup textCanvasGroup;    // 文字的 CanvasGroup
    public TextMeshProUGUI dialogueText;   // 对话文本
    public Image imageComponent;            // 显示图片的 UI Image 组件

    public float fadeDuration = 1f;        // 淡入时间
    public float displayDuration = 3f;     // 显示时间（持续多久后开始淡出）

    private bool isFading = false;

    // 显示指定内容的方法
    public void ShowContent(LocalizedString dialogueKey, Sprite image)
    {
        if (!isFading)
        {
            StartCoroutine(FadeInAndOut(dialogueKey, image));
        }
    }

    private IEnumerator FadeInAndOut(LocalizedString dialogueKey, Sprite image)
    {
        isFading = true;

        // 获取本地化文本
        var localizedResult = dialogueKey.GetLocalizedStringAsync();
        yield return localizedResult;

        // 获取本地化后的字符串
        string text = localizedResult.Result;
        dialogueText.text = text;

        // 更新图片
        imageComponent.sprite = image;

        // 淡入效果：逐渐改变透明度
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            imageCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            textCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }

        // 确保最终透明度为 1
        imageCanvasGroup.alpha = 1f;
        textCanvasGroup.alpha = 1f;

        // 显示一段时间
        yield return new WaitForSeconds(displayDuration);

        // 淡出效果：逐渐改变透明度
        time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            imageCanvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            textCanvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            yield return null;
        }

        // 确保最终透明度为 0
        imageCanvasGroup.alpha = 0f;
        textCanvasGroup.alpha = 0f;

        isFading = false;
    }
}
