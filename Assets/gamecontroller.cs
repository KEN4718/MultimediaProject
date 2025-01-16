using UnityEngine;

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get; private set; }

    public MonoBehaviour[] scriptsToDisable; // 需要禁用的功能脚本

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 启用或禁用脚本功能
    public void SetScriptActiveState(bool isActive)
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = isActive; // 只禁用脚本功能
            }
        }
    }
}