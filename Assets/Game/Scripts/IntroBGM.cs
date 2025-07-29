using UnityEngine;

public class IntroBGM : MonoBehaviour
{
    public AudioClip introMusic;
    private AudioSource bgmSource;

    void Awake()
    {
        // Make this object persist across scenes if needed
        DontDestroyOnLoad(gameObject);

        // Add and configure AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = introMusic;
        bgmSource.loop = true;
        bgmSource.playOnAwake = true;
        bgmSource.volume = 0.6f;
        bgmSource.Play();
    }
}
