using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private string targetScene;  // 要传送到的目标场景

    private void OnCollisionEnter(Collision collision)
    {
        // 检查是否是玩家接触了该方块
        if (collision.gameObject.CompareTag("Player"))
        {
            // 触发传送到目标场景
            SceneManager.LoadScene(targetScene);
        }
    }
}
