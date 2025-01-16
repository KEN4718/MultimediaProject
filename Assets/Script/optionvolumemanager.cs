using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionMenuManager : MonoBehaviour
{
    public AudioMixer audioMixer;      // 引用Audio Mixer
    public Slider bgmSlider;           // 背景音乐音量滑动条
    public Slider ambientSlider;       // 环境音量滑动条
    public Slider voiceSlider;         // 语音音量滑动条

    void Start()
    {
        // 初始化滑动条数值
        float bgmVolume, ambientVolume, voiceVolume;
        audioMixer.GetFloat("BGMVolume", out bgmVolume);
        audioMixer.GetFloat("AmbientVolume", out ambientVolume);
        audioMixer.GetFloat("VoiceVolume", out voiceVolume);

        bgmSlider.value = Mathf.Pow(10, bgmVolume / 20);       // 将分贝(dB)转换为线性值
        ambientSlider.value = Mathf.Pow(10, ambientVolume / 20);
        voiceSlider.value = Mathf.Pow(10, voiceVolume / 20);

        // 为滑动条添加监听器
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        ambientSlider.onValueChanged.AddListener(SetAmbientVolume);
        voiceSlider.onValueChanged.AddListener(SetVoiceVolume);
    }

    // 调整背景音乐音量
    public void SetBGMVolume(float sliderValue)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(sliderValue) * 20); // 线性到分贝转换
    }

    // 调整环境音量
    public void SetAmbientVolume(float sliderValue)
    {
        audioMixer.SetFloat("AmbientVolume", Mathf.Log10(sliderValue) * 20);
    }

    // 调整语音音量
    public void SetVoiceVolume(float sliderValue)
    {
        audioMixer.SetFloat("VoiceVolume", Mathf.Log10(sliderValue) * 20);
    }
}