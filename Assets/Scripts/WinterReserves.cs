using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinterReserves : MonoBehaviour
{
    public float maxHunger = 100f;
    public float currentHunger;

    public Image hungerBar;
    // Start is called before the first frame update
    void Start()
    {
        currentHunger = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHungerUI();
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
            // win
            currentHunger = 100f;
            SceneManager.LoadScene("Victory");
        }
    }
}
