using UnityEngine;

public class TemporaryAudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.Play();
        Destroy(gameObject, audioSource.clip.length);
    }
}