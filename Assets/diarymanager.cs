using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

public class DiaryManager : MonoBehaviour
{
    public TextMeshProUGUI diaryText;     // 日记文本
    public GameObject diaryBox;           // 日记框
    public GameObject backgroundPanel;    // 背景面板
    public AudioSource audioSource;       // 用于播放音效的AudioSource
    public Image theDiaryImage;

    public GameObject innerThoughtPanel;  // 心里对话背景面板
    public TextMeshProUGUI innerThoughtText; // 心里对话的文本

    private Queue<DialogueData> diaryQueue;
    private bool isTyping;
    private System.Action onDiaryEnd; // 回调函数，用于触发心里对话

    public CanvasGroup diaryCanvasGroup; // 控制日记框的透明度
    public CanvasGroup textCanvasGroup;  // 控制文字的透明度
    public float fadeDuration = 1f;    // Fade In 持续时间


    [System.Serializable]
    public class DialogueData
    {
        public LocalizedString dialogue;   // 日记内容
        public AudioClip audioClip;        // 每句日记的音频
        public Sprite diarySprite;
    }

    public bool isInDiary = false;       // 标记是否正在进行日记显示

    void Start()
    {
        diaryQueue = new Queue<DialogueData>();
        diaryBox.SetActive(false);
        backgroundPanel.SetActive(false);
        innerThoughtPanel.SetActive(false); // 默认隐藏心里对话
        isTyping = false;

        // 初始化 CanvasGroup 的透明度
        if (diaryCanvasGroup != null) diaryCanvasGroup.alpha = 0;
        if (textCanvasGroup != null) textCanvasGroup.alpha = 0;
    }

    public void StartDiary(List<DialogueData> dialogues, System.Action diaryEndCallback = null)
    {
        GameStateController.Instance.SetScriptActiveState(false);
        if (isInDiary) return; // 如果已经在显示日记中，不允许启动新的日记显示

        isInDiary = true; // 开始显示日记
        diaryQueue.Clear();

        foreach (var dialogue in dialogues)
        {
            diaryQueue.Enqueue(dialogue);
        }

        onDiaryEnd = diaryEndCallback;

        diaryBox.SetActive(true);
        backgroundPanel.SetActive(true);
        isTyping = false;
        StartCoroutine(StartDiaryWithDelay());
        DisplayNextDiary();
    }



    public void DisplayNextDiary()
    {
        if (isTyping) return;

        if (diaryQueue.Count == 0)
        {
            EndDiary();
            return;
        }

        var nextDiary = diaryQueue.Dequeue();
        SettheDiaryImage(nextDiary.diarySprite);
        StartCoroutine(TypeText(nextDiary.dialogue));

        if (nextDiary.audioClip != null)
        {
            audioSource.clip = nextDiary.audioClip;
            audioSource.Play();
        }
    }
    private void SettheDiaryImage(Sprite sprite)
    {
        theDiaryImage.sprite = sprite;
        theDiaryImage.gameObject.SetActive(sprite != null);
    }
    private IEnumerator TypeText(LocalizedString localizedText)
    {
        isTyping = true;
        diaryText.text = "";

        // 异步加载本地化字符串
        var localizedResult = localizedText.GetLocalizedStringAsync();
        yield return localizedResult;

        // 获取本地化后的字符串
        string text = localizedResult.Result;

        foreach (char letter in text.ToCharArray())
        {
            diaryText.text += letter;
        }

        isTyping = false;
    }

    public void DisplayInnerThought(Sprite innerThoughtSprite, LocalizedString[] innerThoughtText, AudioClip innerThoughtAudio, float duration)
    {
        // 设置心里对话的背景图片
        if (innerThoughtSprite != null)
        {
            innerThoughtPanel.GetComponent<Image>().sprite = innerThoughtSprite;
        }

        // 显示心里对话文本
        StartCoroutine(SetInnerThoughtText(innerThoughtText));

        // 播放心里对话音频
        if (innerThoughtAudio != null)
        {
            audioSource.clip = innerThoughtAudio;
            audioSource.Play();
        }

        // 显示心里对话面板
        Debug.Log("innerThoughtPanel active: " + innerThoughtPanel.activeSelf);
        innerThoughtPanel.SetActive(true);

        // 启动协程，延迟清除
        StartCoroutine(ClearInnerThoughtAfterDelay(duration));
    }

    private IEnumerator ClearInnerThoughtAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        innerThoughtPanel.gameObject.SetActive(false); // 隐藏心里对话图片
        innerThoughtText.text = ""; // 清空文本
    }
    private IEnumerator SetInnerThoughtText(LocalizedString[] localizedTexts)
    {
        string combinedText = "";

        foreach (var localizedText in localizedTexts)
        {
            var stringOperation = localizedText.GetLocalizedStringAsync();
            yield return stringOperation; // 等待本地化字符串加载完成

            if (stringOperation.IsDone)
            {
                combinedText += stringOperation.Result + "\n"; // 拼接每个本地化字符串，使用换行符分隔
            }
        }

        // 设置最终的文本内容
        innerThoughtText.text = combinedText.TrimEnd('\n'); // 移除最后一个多余的换行符
    }

    private IEnumerator StartDiaryWithDelay()
    {
        // 图片的 Fade In
        if (diaryCanvasGroup != null)
        {
            yield return StartCoroutine(FadeIn(diaryCanvasGroup, fadeDuration));
        }

        // 等待一定时间间隔
        float delay = 0f; // 设置时间间隔为 1 秒
        yield return new WaitForSeconds(delay);

        // 文字的 Fade In
        if (textCanvasGroup != null)
        {
            yield return StartCoroutine(FadeIn(textCanvasGroup, fadeDuration));
        }
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration, System.Action onComplete = null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        onComplete?.Invoke();
    }
    public void EndDiary()
    {
        isInDiary = false; // 结束日记显示
        diaryBox.SetActive(false);
        backgroundPanel.SetActive(false);
        onDiaryEnd?.Invoke();
        if (diaryCanvasGroup != null) diaryCanvasGroup.alpha = 0;
        if (textCanvasGroup != null) textCanvasGroup.alpha = 0;
        onDiaryEnd = null;
        GameStateController.Instance.SetScriptActiveState(true);
    }
    void Update()
    {
        // 按下鼠标左键继续对话
        if (Input.GetMouseButtonDown(0) && diaryBox.activeSelf && !isTyping)
        {
            if (isTyping)
            {

                return;
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop(); // 停止播放音频
                }
                DisplayNextDiary();
            }
        }

    }
}
