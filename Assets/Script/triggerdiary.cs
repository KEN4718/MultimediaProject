using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;

public class TriggerDiaryContent : MonoBehaviour
{
    public LocalizedString diaryText;
    public AudioSource audioSource;
    public AudioClip diaryAudio;
    public AudioClip innerthoughtAudio;
    public GameObject diaryBox;
    public Sprite diaryImage;
    public Sprite innerThoughtImageFirstTime;
    public LocalizedString[] innerThoughtDialogueFirstTime;

    public GameObject eButtonUI;
    public GameObject diarymodel;

    public float innerThoughtDuration = 3f; // 心里对话持续时间（秒）
    private bool hasTriggered = false;
    private bool isDiaryActive = false;
    private bool isPlayerInRange = false;
    private ShowInteractableUI showInteractableUI; // Reference to ShowInteractableUI script

    private DiaryManager diaryManager;

    void Start()
    {
        diaryManager = FindObjectOfType<DiaryManager>();
        showInteractableUI = FindObjectOfType<ShowInteractableUI>(); // 获取 ShowInteractableUI 脚本的引用
    }

    void Update()
    {

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDiaryActive)
        {
            Debug.Log("E key pressed, showing diary.");
            ShowDiary();

            // Hide the "E" button UI from ShowInteractableUI
            if (eButtonUI != null)
            {
                eButtonUI.SetActive(false); // Hide the "E" button UI
            }
            if (isPlayerInRange && GameObject.FindGameObjectWithTag("Player").transform.parent != null)
            {
                GameObject.FindGameObjectWithTag("Player").transform.SetParent(null);
                Debug.Log("Fixed player's parent.");
            }

        }

        if (isDiaryActive && Input.GetMouseButtonDown(0)) // 左键点击关闭日记
        {
            Debug.Log("Left click pressed, hiding diary.");
            HideDiary();
            if (diarymodel != null)
            {
                diarymodel.SetActive(false); // Hide the "E" button UI
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            other.transform.SetParent(null); // 确保玩家没有被设置为其他物体的子物体
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

        }
    }

    void ShowDiary()
    {
        if (hasTriggered) return;

        hasTriggered = true;


        var dialogues = new List<DiaryManager.DialogueData>
        {
            new DiaryManager.DialogueData()
            {
                dialogue = diaryText,
                audioClip = diaryAudio,
                diarySprite = diaryImage
            }
        };

        diaryManager.StartDiary(dialogues, () => StartInnerThought(innerthoughtAudio));

        isDiaryActive = true;

    }

    private void HideDiary()
    {
        diaryManager.EndDiary();
        isDiaryActive = false;

    }

    private void StartInnerThought(AudioClip innerThoughtAudio)
    {
        diaryManager.DisplayInnerThought(innerThoughtImageFirstTime, innerThoughtDialogueFirstTime, innerThoughtAudio, innerThoughtDuration);
    }
}
