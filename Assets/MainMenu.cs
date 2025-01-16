using System.Collections; // 确保使用协程命名空间
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup backgroundCanvasGroup; // 背景图的 CanvasGroup
    public float fadeDuration ; // 淡入持续时间（秒）
    public float backgroundDelay ; // 背景图显示开始延迟时间（秒）
    public float totalDelay ; // 总延迟时间（秒）

    public void PlayGame()
    {
        StartCoroutine(DelayedActions());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator DelayedActions()
    {
        // 等待背景图显示的延迟时间
        yield return new WaitForSeconds(backgroundDelay);

        // 渐入背景图
        if (backgroundCanvasGroup != null)
        {
            yield return StartCoroutine(FadeIn(backgroundCanvasGroup));
        }

        // 等待剩余时间
        yield return new WaitForSeconds(totalDelay - backgroundDelay - fadeDuration);

        // 切换场景
        SceneManager.LoadScene(1); // 替换为你的目标场景索引或名称
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration); // 线性渐入
            yield return null; // 等待下一帧
        }

        canvasGroup.alpha = 1f; // 确保最终完全显示
    }
}