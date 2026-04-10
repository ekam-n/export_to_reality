using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoverSounds : MonoBehaviour
{
    [SerializeField] private AudioClip footstepSFX;
    private float footstepVol = 0.3f;
    private float footstepDelay = 0.3f;
    private bool isAlive = true; // 1. The "Kill Switch" for the coroutine
    private MoverController2D moverController2D;

    void Start()
    {
        moverController2D = GetComponent<MoverController2D>();
        if (moverController2D != null)
        {
            StartCoroutine(PlayFootsteps());
        }
    }

    private void OnDisable()
    {
        isAlive = false;
        StopAllCoroutines(); 
    }

    IEnumerator PlayFootsteps()
    {
        while (isAlive)
        {
            if (moverController2D != null && moverController2D.animator != null)
            {
                if (moverController2D.animator.GetBool("isRunning") && moverController2D.IsGrounded())
                {
                    // 5. Check the Manager exists before calling it
                    if (AudioManager.instance != null)
                    {
                        AudioManager.instance.PlaySFX(footstepSFX, footstepVol);
                    }
                }
            }
            yield return new WaitForSeconds(footstepDelay);
        }
    }
}
