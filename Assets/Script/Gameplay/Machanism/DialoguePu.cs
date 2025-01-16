using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class DialoguePu : MonoBehaviour
{
    public LocalizedString[] firstTimeDialogues1; // Ken 的对话数组
    public LocalizedString[] firstTimeDialogues2; // Noah 的对话数组
    public LocalizedString[] secondTimeDialogues1; // 第二次对话 Ken
    public LocalizedString[] secondTimeDialogues2; // 第二次对话 Noah
    public LocalizedString[] thirdTimeDialogues; // 第三次对话

    public AudioClip firstTimeAudio1;
    public AudioClip firstTimeAudio2;
    public AudioClip firstTimeAudio3;
    public AudioClip secondTimeAudio1;
    public AudioClip secondTimeAudio2;
    public AudioClip secondTimeAudio3;
    public AudioClip secondTimeAudio4;
    public AudioClip secondTimeAudio5;
    public AudioClip thirdTimeAudio1;

    public AudioSource currentBGM;           // 当前的背景音乐（现有的BGM）
    public AudioSource newBGM;

    public GameObject unactivategem6;    // gem6模型消失


    public Sprite character1Image;       // 角色1的图片（Ken）
    public Sprite character2Image;       // 角色2的图片（Noah）

    public string character1Name;        // 角色1的名字（Ken）
    public string character2Name;        // 角色2的名字（Noah）

    public float fadeDuration = 1f;      // 黑屏和白屏淡入淡出的时间
    public Image fadeImage;              // 用于黑屏白屏的 UI Image

    public GameObject modelToActivate;   // 需要激活消失的建模（Pu）
    public GameObject modelToRotate;     // 需要旋转的建模（Noah）
    public Vector3 targetRotation;       // 旋转的目标角度

    public float whiteScreenDuration = 2f; // 白屏的持续时间
    public float postFadeDelay = 1f; // 白屏淡出后的延迟时间

    public Image appearImage;            // Inspector 设置的逐渐显现的图像(To be contunued那张）
    public float appearImageDuration = 2f; // 图像渐显的时间

    [SerializeField] private Image backgroundImage;   // 黑色背景，主要是衔接to be contunued那张消失后到影片播放的时间
    [SerializeField] private float backgroundImageFadeInDuration = 0f;
    public AudioSource creditRollBGM;           // Credit Roll 的背景音乐
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoRawImage; // 关联的 影片 Image


    public string targetSceneName;       // 目标场景名称

    private bool hasTriggered = false;   // 标记是否已经触发过对话
    private ManagerPu dialogueManager; // 引用 ManagerPu

    void Start()
    {
        dialogueManager = FindObjectOfType<ManagerPu>();

        if (newBGM != null)
        {
            newBGM.gameObject.SetActive(false);
        }

        if (creditRollBGM != null)
        {
            creditRollBGM.gameObject.SetActive(false);
        }

        if (videoRawImage != null)
        {
            videoRawImage.gameObject.SetActive(false);
        }

        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(StartDialogueSequence(other));
        }
    }

    private IEnumerator StartDialogueSequence(Collider player)
    {
        // 黑屏效果
        yield return StartCoroutine(FadeToBlack());

        if (unactivategem6 != null)
        {
            unactivategem6.SetActive(false);
        }

        // 激活建模
        if (modelToActivate != null)
        {
            modelToActivate.SetActive(true);
        }

        if (modelToRotate != null)
        {
            Vector3 newRotation = modelToRotate.transform.eulerAngles;
            newRotation.y = 180f;
            modelToRotate.transform.eulerAngles = newRotation;
        }

        // 黑屏淡出
        yield return StartCoroutine(FadeFromBlack());

        // 触发时停止现有的BGM
        if (currentBGM != null)
        {
            currentBGM.Stop();
        }

        // 开始第一次对话
        var dialogues = new List<ManagerPu.DialogueData>
    {
        new ManagerPu.DialogueData() { characterName = character1Name, characterSprite = character1Image, dialogue = firstTimeDialogues1[0], audioClip = firstTimeAudio1 },
        new ManagerPu.DialogueData() { characterName = character1Name, characterSprite = character1Image, dialogue = firstTimeDialogues1[1], audioClip = firstTimeAudio2 },
        new ManagerPu.DialogueData() { characterName = character2Name, characterSprite = character2Image, dialogue = firstTimeDialogues2[0], audioClip = firstTimeAudio3 },
    };

        dialogueManager.StartDialogue(dialogues);

        // 等待第一次对话结束
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished() && Input.GetMouseButtonDown(0));

        // 第一次对话结束后播放新的BGM
        if (newBGM != null)
        {
            newBGM.gameObject.SetActive(true);
            newBGM.Play();
        }

        // 旋转建模
        yield return StartCoroutine(RotateModelToTarget(modelToRotate, targetRotation));

        // 开始第二次对话
        var secondDialogues = new List<ManagerPu.DialogueData>
    {
        new ManagerPu.DialogueData() { characterName = character1Name, characterSprite = character1Image, dialogue = secondTimeDialogues1[0], audioClip = secondTimeAudio1 },
        new ManagerPu.DialogueData() { characterName = character2Name, characterSprite = character2Image, dialogue = secondTimeDialogues2[0], audioClip = secondTimeAudio2 },
        new ManagerPu.DialogueData() { characterName = character1Name, characterSprite = character1Image, dialogue = secondTimeDialogues1[1], audioClip = secondTimeAudio3 },
        new ManagerPu.DialogueData() { characterName = character2Name, characterSprite = character2Image, dialogue = secondTimeDialogues2[1], audioClip = secondTimeAudio4 },
        new ManagerPu.DialogueData() { characterName = character2Name, characterSprite = character2Image, dialogue = secondTimeDialogues2[2], audioClip = secondTimeAudio5 },
    };

        dialogueManager.StartDialogue(secondDialogues);

        // 等待第二次对话结束
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        if (modelToActivate != null)
        {
            Animator modelAnimator = modelToActivate.GetComponent<Animator>();
            if (modelAnimator != null)
            {
                modelAnimator.SetTrigger("StartAction");
            }
        }

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));



        // 在第二次对话完全结束后，再开始白屏效果
        yield return StartCoroutine(FadeToWhite());

        // 等待白屏持续时间
        yield return new WaitForSeconds(whiteScreenDuration);

        // 禁用建模
        if (modelToActivate != null)
        {
            modelToActivate.SetActive(false); // 禁用建模
        }

        // 白屏淡出
        yield return StartCoroutine(FadeFromWhite());

        // 延迟 1 秒后开始对话
        yield return new WaitForSeconds(postFadeDelay);

        // 开始第三次对话
        var thirdDialogues = new List<ManagerPu.DialogueData>
    {
        new ManagerPu.DialogueData() { characterName = character1Name, characterSprite = character1Image, dialogue = thirdTimeDialogues[0], audioClip = thirdTimeAudio1 },
    };

        dialogueManager.StartDialogue(thirdDialogues);

        // 等待第三次对话结束
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished() && Input.GetMouseButtonDown(0));


        // 渐显 Image
        if (appearImage != null)
        {
            appearImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeInImage(appearImage, appearImageDuration));

            yield return new WaitForSeconds(2f);
        }

        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(true);
            backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 0f); // 初始透明度为 0

            // 渐显背景图片
            yield return StartCoroutine(FadeInImage(backgroundImage, backgroundImageFadeInDuration));
        }


        // 停止当前播放的 BGM 和隐藏 appearImage
        if (newBGM != null)
        {
            newBGM.Stop();
        }
        appearImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        if (creditRollBGM != null)
        {
            creditRollBGM.gameObject.SetActive(true);
            creditRollBGM.Play();
        }

        yield return new WaitForSeconds(3f);


        if (videoPlayer != null)
        {
            if (videoRawImage != null)
            {
                videoRawImage.gameObject.SetActive(true); // Ensure Raw Image is active
            }

            videoPlayer.gameObject.SetActive(true);
            videoPlayer.Play();

            // Wait for video playback to complete
            // 等待影片播放结束
            videoPlayer.loopPointReached += OnVideoEnd; // 注册事件
        
    }


    }


    private IEnumerator RotateModelToTarget(GameObject model, Vector3 targetRotation)
    {
        if (model == null) yield break;

        Quaternion initialRotation = model.transform.rotation;
        Quaternion finalRotation = Quaternion.Euler(targetRotation);
        float elapsedTime = 0f;
        float duration = 1.5f; // 旋转的持续时间

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            model.transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, elapsedTime / duration);
            yield return null;
        }

        model.transform.rotation = finalRotation;
    }

    private IEnumerator FadeToBlack()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(true);
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator FadeFromBlack()
    {
        fadeImage.color = new Color(0, 0, 0, 1);
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadeImage.color = new Color(0, 0, 0, 1 - t / fadeDuration);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(false);
    }
    private IEnumerator FadeToWhite()
    {
        fadeImage.color = new Color(1, 1, 1, 0);
        fadeImage.gameObject.SetActive(true);

        // 在白屏开始时使角色图像变得完全透明
        dialogueManager.characterImage.color = new Color(1f, 1f, 1f, 0f); // 使角色图像不可见

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadeImage.color = new Color(1, 1, 1, t / fadeDuration);
            yield return null;
        }
        fadeImage.color = new Color(1, 1, 1, 1);
    }

    private IEnumerator FadeFromWhite()
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadeImage.color = new Color(1, 1, 1, 1 - t / fadeDuration);
            yield return null;
        }

        fadeImage.color = new Color(1, 1, 1, 0); // 完全透明
        fadeImage.gameObject.SetActive(false);  // 关闭白屏对象
        dialogueManager.characterImage.color = new Color(1f, 1f, 1f, 1f); // 使角色图像不可见
    }
    private IEnumerator FadeInImage(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color initialColor = image.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            image.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }
        image.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);
    }

    private IEnumerator FadeOutImage(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color initialColor = image.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsedTime / duration);
            image.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }
        image.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
    }

    // 视频播放结束后，再切换场景
    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(WaitBeforeSceneChange());
    }
    private IEnumerator WaitBeforeSceneChange()
    {
        yield return new WaitForSeconds(0f); // 等待额外的几秒
        SceneManager.LoadScene(targetSceneName);
    }
}
