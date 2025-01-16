using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LanguageManager : MonoBehaviour
{
    public TMP_Dropdown languageDropdown; // Dropdown 对象
    public string defaultLanguageCode = "zh-Hans"; // 默认语言代码

    private List<string> languageCodes = new List<string> { "zh-Hans", "en" }; // 支持的语言代码

    void Start()
    {
        // 初始化 Dropdown
        InitializeDropdown();

        // 设置默认语言
        SetLanguage(defaultLanguageCode);
    }

    // 初始化 Dropdown
    void InitializeDropdown()
    {
        // 清空 Dropdown 选项
        languageDropdown.ClearOptions();

        // 添加语言名称到 Dropdown
        List<string> options = new List<string> { "中文", "English" };
        languageDropdown.AddOptions(options);

        // 设置 Dropdown 的默认值
        int defaultIndex = languageCodes.IndexOf(defaultLanguageCode);
        if (defaultIndex >= 0)
        {
            languageDropdown.value = defaultIndex;
        }

        // 监听 Dropdown 的值改变事件
        languageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);
    }

    // Dropdown 改变事件
    void OnLanguageDropdownChanged(int index)
    {
        if (index >= 0 && index < languageCodes.Count)
        {
            SetLanguage(languageCodes[index]);
        }
    }

    // 设置语言的函数
    public void SetLanguage(string languageCode)
    {
        var selectedLocale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);

        if (selectedLocale != null)
        {
            LocalizationSettings.SelectedLocale = selectedLocale;
        }
}
}