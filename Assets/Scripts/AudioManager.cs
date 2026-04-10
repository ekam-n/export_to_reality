using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource masterSource;

    private void Awake()
    {
        instance = this;
        masterSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip audioClip, float volume = 1f)
    {
        if (audioClip == null) return;

        masterSource.PlayOneShot(audioClip, volume);
    }
}
