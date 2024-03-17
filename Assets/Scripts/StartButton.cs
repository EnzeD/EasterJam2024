using UnityEngine;
using UnityEngine.SceneManagement; // Include the SceneManagement namespace

public class StartButton : MonoBehaviour
{
    public SceneFader Fader;
    public void LoadGameplayScene()
    {
        StartCoroutine(Fader.FadeAndLoadScene(SceneFader.FadeDirection.In, "Gameplay"));
    }
}