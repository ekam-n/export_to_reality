using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoverSounds : MonoBehaviour
{
    [SerializeField] private AudioClip footstepSFX;
    private float footstepVol = 0.3f;
    private float footstepDelay = 0.3f;
    
    private MoverController2D moverController2D;

    void Start()
    {
        moverController2D = GetComponent<MoverController2D>();
        StartCoroutine(PlayFootsteps());
        // StartCoroutine(PlayDeath());
    }

    IEnumerator PlayFootsteps()
    {
        while (true)
        {
            if (
                moverController2D.animator.GetBool("isRunning") == true
                && moverController2D.IsGrounded()
            )
            {
                AudioManager.instance.PlaySFX(footstepSFX, footstepVol);
            }

            yield return new WaitForSeconds(footstepDelay);
        }
    }
}
