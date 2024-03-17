using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance = null;

    public static MusicManager Instance
    {
        get { return instance; }
    }

    [SerializeField]
    private float defaultVolume = 0.20f; // The default volume level

    void Awake()
    {
        // Check if instance already exists and assign if not
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // Destroy this instance because it is a duplicate
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // Prevent MusicManager from being destroyed on scene load
        GetComponent<AudioSource>().volume = defaultVolume; // Set the volume
    }
}