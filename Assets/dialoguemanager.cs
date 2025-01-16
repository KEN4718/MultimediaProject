using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;


public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;      // 对话文本
    public TextMeshProUGUI characterName;    // 角色名字
    public GameObject dialogueBox;           // 对话框
    public GameObject backgroundPanel;       // 背景面板
    public Image characterImage;             // 角色图片
    public float textSpeed = 0.05f;          // 打字速度
    public AudioSource audioSource; // 用于播放台词音频
    public GameObject triggerObject; // 触发器对象

    public GameObject innerThoughtPanel;     // 心里对话背景面板
    public TextMeshProUGUI innerThoughtText; // 心里对话的文本
    

    private Queue<DialogueData> dialogueQueue;
    private bool isTyping;
    private System.Action onDialogueEnd; // 回调函数，用于触发心里对话

    private ZeraMovement playerMovement; // 玩家控制脚本的引用

    [System.Serializable]
    public class DialogueData
    {
        public string characterName;
        public Sprite characterSprite;
        public LocalizedString dialogue;  // 使用 LocalizedString 替代 string 类型
        public AudioClip audioClip; // 每句台词的音频
    }

     public bool isInDialogue = false; // 标记是否正在进行对话

    void Start()
    {
        dialogueQueue = new Queue<DialogueData>();
        dialogueBox.SetActive(false);
        backgroundPanel.SetActive(false);
        innerThoughtPanel.SetActive(false); // 默认隐藏心里对话
        isTyping = false;

        // 获取玩家移动控制脚本
        playerMovement = FindObjectOfType<ZeraMovement>();
    }

    public void StartDialogue(List<DialogueData> dialogues, System.Action dialogueEndCallback = null)
    {
        GameStateController.Instance.SetScriptActiveState(false);
         if (isInDialogue) return; // 如果已经在对话中，不允许启动新的对话

        isInDialogue = true; // 开始对话
        dialogueQueue.Clear();

        foreach (var dialogue in dialogues)
        {
            dialogueQueue.Enqueue(dialogue);
        }

        onDialogueEnd = dialogueEndCallback;

        playerMovement.canClick = false;



        dialogueBox.SetActive(true);
        backgroundPanel.SetActive(true);
        LockPlayerControls(); // 禁用玩家操作
        isTyping = false;
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

    public void DisplayInnerThought(Sprite innerThoughtSprite, LocalizedString[] innerThoughtText, float duration)
    {
         if (innerThoughtSprite == null)
    {
        // 如果图片为空，隐藏内心对话面板
        innerThoughtPanel.SetActive(false);
        return;
    }
        // 设置心里对话背景和文本
        innerThoughtPanel.GetComponent<Image>().sprite = innerThoughtSprite;

        StartCoroutine(SetInnerThoughtText(innerThoughtText));

        // 显示心里对话
        innerThoughtPanel.SetActive(true);

        // 启动协程，延迟清除
        StartCoroutine(ClearInnerThoughtAfterDelay(duration));
    }

    private IEnumerator ClearInnerThoughtAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 隐藏心里对话
        innerThoughtPanel.SetActive(false);
        innerThoughtText.text = ""; // 清空文字内容
    }

    public void EndDialogue()
    {
        isInDialogue = false; // 结束对话
        dialogueBox.SetActive(false);
        backgroundPanel.SetActive(false);    
        onDialogueEnd?.Invoke();
        SetCharacterName("");
        onDialogueEnd = null;
        SetCharacterImage(null);
        playerMovement.canClick = true;
        GameStateController.Instance.SetScriptActiveState(true);
        
        UnlockPlayerControls(); // 解除玩家操作禁用

        ActivateTrigger();
    
    }

public void ActivateTrigger()
    {
        // 激活触发器
        if (triggerObject != null)
        {
            triggerObject.SetActive(true);
        }
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
    void Update()
    {
        // 按下鼠标左键继续对话
        if (Input.GetMouseButtonDown(0) && dialogueBox.activeSelf)
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
                DisplayNextDialogue();
            }
        }
    }
}

