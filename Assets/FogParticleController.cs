using UnityEngine;

public class FogParticleController : MonoBehaviour
{
    [Header("Fog Particle Settings")]
    public ParticleSystem fogParticleSystem; // 指定的粒子系统（雾效果）
    public float fadeDuration = 5f;          // 粒子淡出时间（秒）

    private ParticleSystem.MainModule fogMain; // 粒子系统主模块
    private float initialStartSize;           // 初始粒子大小
    private float initialStartAlpha;          // 初始粒子透明度
    private bool isFading = false;            // 标志是否开始淡化
    private float fadeTimer = 0f;             // 淡化计时器

    private void Start()
    {
        if (fogParticleSystem != null)
        {
            // 获取粒子系统主模块
            fogMain = fogParticleSystem.main;
            // 保存初始粒子的大小和透明度
            initialStartSize = fogMain.startSize.constant;
            initialStartAlpha = fogMain.startColor.color.a;
        }
        else
        {
            Debug.LogError("Fog Particle System is not assigned.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFading) // 检测触发对象是否是玩家
        {
            isFading = true; // 开始淡化粒子
        }
    }

    private void Update()
    {
        if (isFading && fogParticleSystem != null)
        {
            // 更新淡化计时器
            fadeTimer += Time.deltaTime;
            float fadeProgress = Mathf.Clamp01(fadeTimer / fadeDuration);

            // 逐渐减少粒子大小
            fogMain.startSize = Mathf.Lerp(initialStartSize, 0f, fadeProgress);

            // 逐渐减少粒子透明度
            Color startColor = fogMain.startColor.color;
            startColor.a = Mathf.Lerp(initialStartAlpha, 0f, fadeProgress);
            fogMain.startColor = new ParticleSystem.MinMaxGradient(startColor);

            // 淡化完成后禁用粒子系统
            if (fadeProgress >= 1f)
            {
                fogParticleSystem.Stop();
                fogParticleSystem.gameObject.SetActive(false);
                isFading = false;
            }
        }
    }
}