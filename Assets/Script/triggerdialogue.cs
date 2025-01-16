using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class TriggerDialogue : MonoBehaviour
{
    public LocalizedString[] firstTimeDialogues1; // Ken 的对话数组（第一次）
    public LocalizedString[] firstTimeDialogues2; // Noah 的对话数组（第一次）
    public LocalizedString[] repeatDialogues1;    // Ken 的对话数组（重复）
    public LocalizedString[] repeatDialogues2;    // Noah 的对话数组（重复）
    public LocalizedString[] newDialogues1;
    public LocalizedString[] newDialogues2;

    public bool isUpdated = false;

    public GameObject questionMark;

    public AudioSource audioSource;        // 用于播放音效的AudioSource
    public AudioClip[] firstPersonAudio;  // Ken 的音频剪辑数组
    public AudioClip[] repeatFirstPersonAudio;  // Ken 的音频剪辑数组
    public AudioClip[] secondPersonAudio; // Noah 的音频剪辑数组
    public AudioClip[] newRepeatFirstPersonAudio;  // Ken 的音频剪辑数组
    public AudioClip innerthoughtAudio;
    public AudioClip innerthoughtAudio2;

    public Sprite character1Image;       // 角色1的图片（Ken）
    public Sprite character2Image;       // 角色2的图片（Noah）
    public Sprite newCharacter1Image;   // 新的角色1图片


    public string character1Name;        // 角色1的名字（Ken）
    public string character2Name;        // 角色2的名字（Noah）

    public Sprite innerThoughtImageFirstTime; // 第一次心里对话图片
    public LocalizedString[] innerThoughtDialogueFirstTime; // 第一次心里对话文本
    public LocalizedString[] newInnerThoughtDialogueFirstTime;

    public Sprite innerThoughtImageRepeat; // 重复心里对话图片（放跟innerThoughtImageFirstTime一样的背景就行）
    public LocalizedString[] innerThoughtDialogueRepeat; // 重复心里对话文本

    public float innerThoughtDuration = 3f; // 心里对话持续时间（秒）

    private bool hasTriggered = false;   // 标记是否已经触发过对话
    private DialogueManager dialogueManager; // 引用 DialogueManager
    int b = 2;


    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            var dialogues = new List<DialogueManager.DialogueData>();
            if (!hasTriggered)
            {
                dialogues = new List<DialogueManager.DialogueData>();

                if (firstTimeDialogues1.Length <= 4)
                {
                    // 添加正常的首次对话逻辑
                    dialogues.Add(new DialogueManager.DialogueData()
                    {
                        characterName = character1Name,
                        characterSprite = character1Image,
                        dialogue = firstTimeDialogues1[0],
                        audioClip = firstPersonAudio[0]
                    });
                    if (secondPersonAudio[0] != null && firstTimeDialogues2[0] != null)
                    {
                        dialogues.Add(new DialogueManager.DialogueData()
                        {
                            characterName = character2Name,
                            characterSprite = character2Image,
                            dialogue = firstTimeDialogues2[0],
                            audioClip = secondPersonAudio[0]

                        });
                    }

                    dialogues.Add(new DialogueManager.DialogueData()
                    {
                        characterName = character1Name,
                        characterSprite = character1Image,
                        dialogue = firstTimeDialogues1[1],
                        audioClip = firstPersonAudio[1]
                    });
                    if (secondPersonAudio[1] != null && firstTimeDialogues2[1] != null)
                    {
                        dialogues.Add(new DialogueManager.DialogueData()
                        {
                            characterName = character2Name,
                            characterSprite = character2Image,
                            dialogue = firstTimeDialogues2[1],
                            audioClip = secondPersonAudio[1]
                        });
                    }

                    dialogues.Add(new DialogueManager.DialogueData()
                    {
                        characterName = character1Name,
                        characterSprite = character1Image,
                        dialogue = firstTimeDialogues1[2],
                        audioClip = firstPersonAudio[2]
                    });
                    if (firstTimeDialogues1.Length > 3)
                    {
                        dialogues.Add(new DialogueManager.DialogueData()
                        {
                            characterName = character1Name,
                            characterSprite = character1Image,
                            dialogue = firstTimeDialogues1[3],
                            audioClip = firstPersonAudio[3]
                        });
                    }
                    if (questionMark != null)
                    {
                        questionMark.SetActive(false); // 隐藏对象
                    }
                    dialogueManager.StartDialogue(dialogues, () => StartInnerThought(innerThoughtImageFirstTime, innerThoughtDialogueFirstTime));
                }



                hasTriggered = true; // 标记为已触发
            }

            else
            {
                // 检查 repeatDialogues 是否为空
                if (isUpdated)
                {
                    // 玩家已经更新了对话内容，允许触发 repeatDialogue
                    StartRepeatDialogue();
                }
                else
                {
                    Debug.Log("Dialogue not updated yet. Skipping repeatDialogue.");
                    return; // 未更新对话内容，直接跳过
                }
            }


        }

    }
    private void StartRepeatDialogue()
    {
        // 重复触发对话
        var repeatDialogues = new List<DialogueManager.DialogueData>();

        for (int a = 0; a < b; a++)
        {
            repeatDialogues.Add(new DialogueManager.DialogueData()
            {
                characterName = character1Name,
                characterSprite = character1Image,
                dialogue = repeatDialogues1[a],
                audioClip = repeatFirstPersonAudio[a]
            });
        }


        dialogueManager.StartDialogue(repeatDialogues, () => StartInnerThought(innerThoughtImageRepeat, new LocalizedString[] { innerThoughtDialogueFirstTime[0] }));
        GameStateController.Instance.SetScriptActiveState(true); // 恢复其他脚本
        if (questionMark != null)
        {
            questionMark.SetActive(false); // 隐藏对象
        }
    }
    private void StartInnerThought(Sprite innerThoughtSprite, LocalizedString[] innerThoughtText)
    {

        dialogueManager.DisplayInnerThought(innerThoughtSprite, innerThoughtText, innerThoughtDuration);
        if (innerthoughtAudio != null)
        {
            audioSource.PlayOneShot(innerthoughtAudio);
        }

    }

    public void ReplaceCharacter1Image()
    {

        character1Image = newCharacter1Image; // 将 character1Image 替换为新的图片
    }
    public void UpdateFirstTimeDialogues()
    {
        isUpdated = true;
        if (questionMark != null)
        {
            questionMark.SetActive(true); // 隐藏对象
        }
        repeatDialogues1[0] = newDialogues1[0];
        repeatDialogues1[1] = newDialogues1[1];
        repeatFirstPersonAudio[0] = newRepeatFirstPersonAudio[0];
        repeatFirstPersonAudio[1] = newRepeatFirstPersonAudio[1];
        innerThoughtDialogueFirstTime[0] = newInnerThoughtDialogueFirstTime[0];
        innerthoughtAudio = innerthoughtAudio2;

    }
}