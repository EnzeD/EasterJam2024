using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimeBeforeWinter : MonoBehaviour
{
    public float maxHunger = 100f;
    public float hungerDecayRate = 0.5f; // Rate of decay per second
    private float currentHunger;

    // Blinking

    public float blinkThreshold = 25f; // Blink bellow 25
    public float blinkSpeed = 2f;
    public bool isBlinking = false;

    public Image hungerBar;
    // Start is called before the first frame update
    void Start()
    {
        currentHunger = maxHunger;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHunger();
        UpdateHungerUI();
    }

    void UpdateHunger()
    {
        currentHunger -= hungerDecayRate * Time.deltaTime;
        if (currentHunger <= 0f) 
        {
            SceneManager.LoadScene("Gameover");
        }
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
    }

    void UpdateHungerUI()
    {
        hungerBar.fillAmount = currentHunger / maxHunger;

        if (currentHunger < blinkThreshold && !isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkHungerBar());
        }
        else if (currentHunger >= blinkThreshold && isBlinking)
        {
            isBlinking = false;
            StopCoroutine(BlinkHungerBar());
            SetHungerBarAlpha(1f); // Fully visible if not blinking
        }
    }

    IEnumerator BlinkHungerBar()
    {
        while (isBlinking)
        {
            // Lerp the alpha value
            hungerBar.color = new Color(hungerBar.color.r, hungerBar.color.g, hungerBar.color.b, Mathf.PingPong(Time.time * blinkSpeed, 1.0f));
            yield return null;
        }

        // Reset if stop blink but probably not necessary
        SetHungerBarAlpha(1f);
    }

    void SetHungerBarAlpha(float alpha)
    {
        hungerBar.color = new Color(hungerBar.color.r, hungerBar.color.g, hungerBar.color.b, alpha);
    }
}
