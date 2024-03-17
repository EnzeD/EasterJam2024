using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeBeforeWinter : MonoBehaviour
{
    public float maxHunger = 100f;
    public float hungerDecayRate = 0.5f; // Rate of decay per second
    private float currentHunger;

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
    }
}
