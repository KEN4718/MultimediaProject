using UnityEngine;
using System.Collections.Generic;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private Dictionary<string, Dictionary<string, string>> localizedTexts;
    private string currentLanguage;

    void Awake()
    {
        // 单例模式，确保只创建一个 LocalizationManager 实例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadLocalizedText();
    }

    void LoadLocalizedText()
    {
        // 假设我们有一个文件名为 "Localization.json"
        TextAsset textAsset = Resources.Load<TextAsset>("Localization");
        localizedTexts = JsonUtility.FromJson<LocalizedTextWrapper>(textAsset.text).texts;
        currentLanguage = "en";  // 默认语言为英文
    }

    public void SetLanguage(string language)
    {
        currentLanguage = language;
    }

    public string GetLocalizedText(string key)
    {
        if (localizedTexts.ContainsKey(currentLanguage) && localizedTexts[currentLanguage].ContainsKey(key))
        {
            return localizedTexts[currentLanguage][key];
        }
        else
        {
            return key;  // 如果没有找到对应的键，返回键本身
        }
    }

    [System.Serializable]
    public class LocalizedTextWrapper
    {
        public Dictionary<string, Dictionary<string, string>> texts;
    }
}