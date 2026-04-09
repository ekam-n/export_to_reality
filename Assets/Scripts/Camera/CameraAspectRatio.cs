using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    public float targetAspect = 16f / 9f;

    private Camera cam;
    private int lastScreenWidth;
    private int lastScreenHeight;

    void Start()
    {
        cam = GetComponent<Camera>();
        EnsureBackgroundCamera();
        ApplyAspect();
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            ApplyAspect();
    }

    void ApplyAspect()
    {
        lastScreenWidth  = Screen.width;
        lastScreenHeight = Screen.height;

        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight  = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            cam.rect = new Rect(0, (1f - scaleHeight) / 2f, 1f, scaleHeight);
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            cam.rect = new Rect((1f - scaleWidth) / 2f, 0, scaleWidth, 1f);
        }
    }

    // Creates a child camera that fills the full screen with black,
    // rendering behind the main camera to cover the letterbox/pillarbox areas.
    void EnsureBackgroundCamera()
    {
        // Don't create a duplicate if one already exists
        Transform existing = transform.Find("LetterboxCamera");
        if (existing != null) return;

        GameObject bg = new GameObject("LetterboxCamera");
        bg.transform.SetParent(transform, false);

        Camera bgCam = bg.AddComponent<Camera>();
        bgCam.clearFlags      = CameraClearFlags.SolidColor;
        bgCam.backgroundColor = Color.black;
        bgCam.cullingMask     = 0;           // render nothing — just clear to black
        bgCam.depth           = cam.depth - 1;
        bgCam.rect            = new Rect(0, 0, 1, 1);
        bgCam.orthographic    = true;
    }
}
