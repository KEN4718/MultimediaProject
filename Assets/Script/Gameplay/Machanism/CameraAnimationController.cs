using UnityEngine;
using DG.Tweening;

public class CameraAnimationController : MonoBehaviour
{
    public Camera mainCamera;                   // Assign your Main Camera in the Inspector
    public Camera subCamera;                    // Assign your Sub Camera in the Inspector
    public float transitionDuration = 1.5f;     // Duration of the animation

    private Vector3 defaultRotation = new Vector3(35.237f, 45.207f, 0f);

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Set initial rotation to default
        mainCamera.transform.rotation = Quaternion.Euler(defaultRotation);

        // Sync initial size of sub-camera
        SyncSubCameraSize();
    }

    public void ZoomTo(Vector3 targetPosition, float targetSize)
    {
        // Kill any active tweens to prevent interference
        DOTween.Kill(mainCamera.transform);

        // Animate position and size to target
        mainCamera.transform.DOMove(targetPosition, transitionDuration).SetEase(Ease.InOutQuad);
        DOTween.To(() => mainCamera.orthographicSize, x =>
        {
            mainCamera.orthographicSize = x;
            SyncSubCameraSize(); // Sync sub-camera size
        }, targetSize, transitionDuration).SetEase(Ease.InOutQuad);
    }

    private void SyncSubCameraSize()
    {
        if (subCamera != null)
        {
            subCamera.orthographicSize = mainCamera.orthographicSize;
        }
    }
}
