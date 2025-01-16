using UnityEngine;
using UnityEngine.Localization;

public class PlayerTrigger : MonoBehaviour
{
    public LocalizedString dialogueKey;  // 绑定的本地化文本
    public Sprite imageToShow;           // 绑定的图片

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 当玩家触碰到触发区域时，显示相应的内容
            FindObjectOfType<FadeInTextAndImage>().ShowContent(dialogueKey, imageToShow);
        }
    }
}
