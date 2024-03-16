using UnityEngine;
using UnityEngine.UI;

public class Hunger : MonoBehaviour
{
    public float maxHunger = 100f;
    public float hungerDecayRate = 1f; // Rate of decay per second
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
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
    }

    void UpdateHungerUI()
    {
        hungerBar.fillAmount = currentHunger / maxHunger;
    }

    public void ReduceHunger(float amount)
    {        
        currentHunger += amount;
        if (currentHunger >= 100f)
        {
            currentHunger = 100f;
        }
    }
}
