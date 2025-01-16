using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuPanel; // 拖入你的 MenuPanel
    public GameObject[] otherPanels;
    private bool isMenuActive = false;

    private AudioSource[] allAudioSources;

    void Start()
    {
        // 获取所有音频源
        allAudioSources = FindObjectsOfType<AudioSource>();
    }

    void Update()
    {
        // 检测 ESC 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        isMenuActive = !isMenuActive; // 切换菜单状态
        menuPanel.SetActive(isMenuActive);

        if (isMenuActive)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // 暂停游戏

        // 静音所有音频源
        foreach (var audioSource in allAudioSources)
        {
            audioSource.Pause();
        }
    }

    public void ResumeGame()
    {
        isMenuActive = isMenuActive;
        Time.timeScale = 1f;  // 恢复游戏时间
        menuPanel.SetActive(false); // 隐藏暂停菜单

        // 遍历所有其他面板并关闭
        foreach (GameObject panel in otherPanels)
        {
            if (panel.activeSelf)
            {
                panel.SetActive(false);
            }
        }
    }
}
