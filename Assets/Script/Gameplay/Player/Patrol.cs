using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Patrol : MonoBehaviour
{
    public Vector3 destination1Offset, destination2Offset;
    public Vector3 forwardDirection = new Vector3(1, 0, 0); // 主角触发后前进的方向（可调整）
    public float forwardDistance = 1f; // 主角前进的距离
    public float forwardDuration = 1f; // 前进动画的持续时间
    public Animator anim; // 主角的Animator
    public Image fadeImage; // 黑屏的Image组件
    public float fadeDuration = 1f; // 渐变黑屏的时间


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(destination1Offset + transform.position, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(destination2Offset + transform.position, 0.2f);
        Gizmos.DrawLine(destination1Offset + transform.position, destination2Offset + transform.position);
    }

    IEnumerator OnTriggerEnter(Collider other)
    {
        // 检查是否有 Animator 控制
        if (anim == null)
        {
            anim = other.GetComponent<Animator>();
        }

        if (anim != null)
        {
                        // 获取 DialogueManager 的引用
            DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();

            // 如果当前正在对话，则不修改 canClick
            if (dialogueManager != null && dialogueManager.isInDialogue)
            {
                // 如果正在对话，则直接跳出，不做任何修改
                yield break;
            }
            // 锁定用户点击
            ZeraMovement zera = other.GetComponent<ZeraMovement>();
            if (zera != null)
            {
                zera.canClick = false;
            }

            // 启动走路动画
            anim.SetBool("IsRunning", true);


            // 让角色先朝指定方向前进
            Vector3 forwardTarget = other.transform.position + forwardDirection.normalized * forwardDistance;
            yield return other.transform.DOMove(forwardTarget, forwardDuration).SetEase(Ease.Linear).WaitForCompletion();

            // 渐变黑屏
            if (fadeImage != null)
            {
                fadeImage.gameObject.SetActive(true); // 激活Image
                yield return FadeToBlack();

            }

            // 停止走路动画
            anim.SetBool("IsRunning", false);

            // 短暂等待后瞬移到第一个目标点
            yield return new WaitForSeconds(0.5f);
            other.transform.position = destination1Offset + transform.position;

            // 设置建模的旋转角度为 180 度
            other.transform.rotation = Quaternion.Euler(0, 180, 0);


            // 渐变恢复亮屏
            if (fadeImage != null)
            {
                yield return FadeToClear();
                fadeImage.gameObject.SetActive(false); // 禁用Image
            }

            anim.SetBool("IsRunning", true);

            // 平滑移动到第二个目标点
            yield return other.transform.DOMove(destination2Offset + transform.position, 0.5f).SetEase(Ease.Linear).WaitForCompletion();

            anim.SetBool("IsRunning", false);


            // 解锁用户点击
            if (zera != null)
            {
                zera.canClick = true;
            }
        }
    }
    // 渐变黑屏
    IEnumerator FadeToBlack()
    {
        fadeImage.raycastTarget = true; // 防止用户点击
        yield return fadeImage.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
    }

    // 渐变亮屏
    IEnumerator FadeToClear()
    {
        yield return fadeImage.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
        fadeImage.raycastTarget = false; // 恢复用户点击
    }
}