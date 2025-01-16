using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

public class ManagerPu : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;      // 对话文本
    public TextMeshProUGUI characterName;    // 角色名字
    public GameObject dialogueBox;           // 对话框
    public GameObject backgroundPanel;       // 背景面板
    public Image characterImage;             // 角色图片
    public float textSpeed = 0.05f;          // 打字速度

    private Queue<DialogueData> dialogueQueue;
    private bool isTyping;
    private System.Action onDialogueEnd; // 回调函数

    private ZeraMovement playerMovement; // 玩家控制脚本的引用
    private bool isDialogueComplete; // 新增变量，用于标记当前对话是否完成

    public AudioSource audioSource; // 用于播放台词音频

    [System.Serializable]
    public class DialogueData
    {
        public string characterName;
        public Sprite characterSprite;
        public LocalizedString dialogue;
        public AudioClip audioClip; // 每句台词的音频
    }

    void Start()
    {
        dialogueQueue = new Queue<DialogueData>();
        dialogueBox.SetActive(false);
        backgroundPanel.SetActive(false);
        isTyping = false;
        isDialogueComplete = false; // 初始化标记为未完成

        // 获取玩家移动控制脚本
        playerMovement = FindObjectOfType<ZeraMovement>();
    }

    public void StartDialogue(List<DialogueData> dialogues, System.Action dialogueEndCallback = null)
    {
        dialogueQueue.Clear();

        foreach (var dialogue in dialogues)
        {
            dialogueQueue.Enqueue(dialogue);
        }

        onDialogueEnd = dialogueEndCallback;

        dialogueBox.SetActive(true);
        backgroundPanel.SetActive(true);
        LockPlayerControls(); // 禁用玩家操作
        DisplayNextDialogue();
    }

    public void DisplayNextDialogue()
    {
        if (isTyping) return;

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        var nextDialogue = dialogueQueue.Dequeue();
        SetCharacterName(nextDialogue.characterName);
        SetCharacterImage(nextDialogue.characterSprite);
        if (nextDialogue.audioClip != null)
        {
            audioSource.clip = nextDialogue.audioClip;
            audioSource.Play();
        }
        StartCoroutine(TypeText(nextDialogue.dialogue));
    }

    private IEnumerator TypeText(LocalizedString localizedText)
    {
        isTyping = true;
        isDialogueComplete = false; // 打字未完成，标记为 false
        dialogueText.text = "";

      // 异步加载本地化字符串
    var localizedResult = localizedText.GetLocalizedStringAsync();
    yield return localizedResult;

    // 获取本地化后的字符串
    string text = localizedResult.Result;

    foreach (char letter in text.ToCharArray())
    {
        dialogueText.text += letter;
        yield return new WaitForSeconds(textSpeed);
    }

        isTyping = false;
        isDialogueComplete = true; // 打字完成，等待用户确认
    }

    private void SetCharacterName(string name)
    {
        characterName.text = name;
        characterName.gameObject.SetActive(!string.IsNullOrEmpty(name));
    }

    private void SetCharacterImage(Sprite sprite)
    {
        characterImage.sprite = sprite;
        characterImage.gameObject.SetActive(sprite != null);
    }

    public void EndDialogue()
    {
        dialogueBox.SetActive(false);
        backgroundPanel.SetActive(false);
        onDialogueEnd?.Invoke();
        dialogueText.text = "";
        SetCharacterName("");
        onDialogueEnd = null;
        SetCharacterImage(null);
        isDialogueComplete = false; // 重置对话完成状
        UnlockPlayerControls(); // 解除玩家操作禁用
    }

    private void LockPlayerControls()
    {
        if (playerMovement != null)
        {
            playerMovement.canClick = false; // 停止玩家点击操作
        }
    }

    private void UnlockPlayerControls()
    {
        if (playerMovement != null)
        {
            playerMovement.canClick = true; // 恢复玩家点击操作
        }
    }

    // Method to check if the dialogue is finished
    public bool IsDialogueFinished()
    {
        return dialogueQueue.Count == 0 && !isTyping;
    }
    void Update()
    {
        // 按下鼠标左键继续对话
        if (Input.GetMouseButtonDown(0) && dialogueBox.activeSelf)
        {
            if (isTyping && audioSource.isPlaying)
            {
                return;
            }
            else if (isDialogueComplete)
            {
                // 当前文字展示完毕后，用户点击才进入下一句
                DisplayNextDialogue();
            }
        }
    }
}
