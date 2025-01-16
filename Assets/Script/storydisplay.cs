using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class StoryDisplay : MonoBehaviour
{
    public GameObject storyCanvasObject;    // 故事Canvas的GameObject
    public TextMeshProUGUI storyText;       // 显示故事内容（TextMeshPro）
    public LocalizedString[] storyContent;            // 故事内容
    public Image backgroundImage1;         // 背景图片1
    
    public Image backgroundImage2;         // 背景图片2（需要第二张张图片时用，例如法杖）
    public AudioSource audioSource;        // 用于播放音效的AudioSource
    public AudioClip storyStartAudio;      // 故事开始的配音
    public float audioDelay = 2f;           // 配音延迟时间（日记的，不是心里对话）
    public AudioClip storyEndAudio;  
    // 故事淡入相关
    public float textDelay = 0f;           // 文字延迟显示时间，背景淡入结束才开始算，所以调成0

    // 心里对话相关
    public Image innerThoughtImage;        // 心里对话背景图片
    public Image innerThoughtCharacterImage;        // 心里对话背景图片
    public TextMeshProUGUI innerThoughtText; // 心里对话文字
    public LocalizedString[] innerThoughtContent;     // 心里对话内容
    public float innerThoughtDuration = 3f; // 心里对话持续时间

    private bool isInTrigger = false;      // 玩家是否在触发区域内
    private bool isStoryActive = false;    // 故事是否处于活动状态
    private bool hasTriggeredStory = false; // 确保故事只触发一次
    private bool hasTriggeredInnerThought = false; // 确保心里对话只触发一次


    void Start()
    {


        // 背景图片初始状态
        if (backgroundImage1 != null)
        {
            backgroundImage1.enabled = false;
        }

        if (backgroundImage2 != null)
        {
            backgroundImage2.enabled = false;
        }

        // 心里对话初始状态
        if (innerThoughtImage != null)
        {
            innerThoughtImage.enabled = false;
        }
        if (innerThoughtCharacterImage != null)
        {
            innerThoughtCharacterImage.enabled = false;
        }

        if (innerThoughtText != null)
        {
            innerThoughtText.text = "";
        }
    }

    void Update()
    {
        // 玩家按下E键且位于触发区域内，并且故事未触发过
        if (isInTrigger && Input.GetKeyDown(KeyCode.E) && !isStoryActive && !hasTriggeredStory)
        {
            DisplayStory();
        }

        // 玩家按下鼠标左键结束故事，且故事已经触发
        if (isStoryActive && Input.GetMouseButtonDown(0)) // 0为鼠标左键
        {
            HideStory();
        }
    }

    void DisplayStory()
{
    if (storyCanvasObject != null)
    {
        storyCanvasObject.SetActive(true); // 激活Canvas
    }

    // 显示背景图片
    if (backgroundImage1 != null) backgroundImage1.enabled = true;
    if (backgroundImage2 != null) backgroundImage2.enabled = true;

    // 直接显示文本内容
    if (storyText != null && storyContent.Length > 0)
    {
        storyText.text = storyContent[0].GetLocalizedString(); // 直接显示第一个故事内容
    }

    isStoryActive = true; // 标记故事为活动状态
    hasTriggeredStory = true; // 设置为已触发过故事

    // 延迟播放配音
    if (storyStartAudio != null)
    {
        StartCoroutine(PlayStoryAudioWithDelay(audioDelay));
    }
}

    public void HideStory()
    {
         // 停止播放故事开始的配音
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        // 隐藏Canvas和背景图片
        if (storyCanvasObject != null)
        {
            storyCanvasObject.SetActive(false);
        }

        if (backgroundImage1 != null)
        {
            backgroundImage1.enabled = false;
        }

        if (backgroundImage2 != null)
        {
            backgroundImage2.enabled = false;
        }

 

        isStoryActive = false; // 标记故事为不活动状态

        // 显示心里对话
        if (!hasTriggeredInnerThought)
        {
            ShowInnerThought();
        }
         // 播放故事结束的音效
        if (audioSource != null && storyEndAudio != null)
        {
            audioSource.PlayOneShot(storyEndAudio);
        }

    }

        void ShowInnerThought()
    {
        // 激活心里对话背景和文本
        if (innerThoughtImage != null)
        {
            innerThoughtImage.enabled = true;
        }

        if (innerThoughtCharacterImage != null)
        {
            innerThoughtCharacterImage.enabled = true;
        }

        if (innerThoughtText != null)
        {
            innerThoughtText.text = "";
        }

        StartCoroutine(DisplayInnerThoughtText());
    }

    IEnumerator DisplayInnerThoughtText()
    {
        // 延迟显示心里对话
        float elapsedTime = 0f;
        while (elapsedTime < innerThoughtDuration)
        {
            if (innerThoughtText != null)
            {
                innerThoughtText.text = innerThoughtContent[0].GetLocalizedString();
            }
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        HideInnerThought();
    }

    void HideInnerThought()
    {
        if (innerThoughtImage != null)
        {
            innerThoughtImage.enabled = false;
        }

        if (innerThoughtCharacterImage != null)
        {
            innerThoughtCharacterImage.enabled = false;
        }

        if (innerThoughtText != null)
        {
            innerThoughtText.text = "";
        }

        hasTriggeredInnerThought = true;
    }

    


    IEnumerator PlayStoryAudioWithDelay(float delay)
    {
        // 延迟播放配音
        yield return new WaitForSecondsRealtime(delay);
        if (audioSource != null && storyStartAudio != null)
        {
            audioSource.PlayOneShot(storyStartAudio);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;
        }
    }
}
