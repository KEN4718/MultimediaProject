using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GlowingTransition : MonoBehaviour
{
    public CanvasGroup transitionCanvasGroup; // Attach the CanvasGroup of the white overlay
    public RectTransform transitionImage;     // Attach the RectTransform of the white overlay
    public float defaultDuration = 3f;       // Default duration for the transition

    public void StartGlowingTransition(Vector3 playerPosition, float duration = -1f)
    {
        if (duration <= 0) duration = defaultDuration;

        // Convert player world position to canvas screen position
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(playerPosition);
        transitionImage.position = screenPosition;

        // Reset scale and alpha
        transitionImage.localScale = Vector3.zero;
        transitionCanvasGroup.alpha = 0;

        // Animate the glow
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transitionCanvasGroup.DOFade(1, duration / 3)); // Fade in
        sequence.Join(transitionImage.DOScale(new Vector3(200, 180, 5), duration / 3)); // Scale up
        sequence.AppendInterval(duration / 3);                          // Hold the white screen
        sequence.Append(transitionCanvasGroup.DOFade(0, duration / 3)); // Fade out
        sequence.Join(transitionImage.DOScale(new Vector3(200, 180, 5), duration / 3)); // Scale further out
        sequence.Play();
    }
}
