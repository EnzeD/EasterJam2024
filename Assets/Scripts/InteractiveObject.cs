using UnityEngine;

public enum InteractiveObjectType
{
    Berry,
    Honey,
    RabbitMeat,
    DeerMeat,
    HumanMeat
}

public class InteractiveObject : MonoBehaviour
{
    public InteractiveObjectType objectType;

    private Hunger hungerScript;

    private float hungerReductionAmount;
    // Start is called before the first frame update
    void Start()
    {
        hungerScript = FindObjectOfType<Hunger>();
        if(hungerScript == null)
        {
            Debug.LogError("Hunger script not found!");
        }

        switch (objectType)
        {
            case InteractiveObjectType.Berry:
            hungerReductionAmount = 10f;
                break;
            case InteractiveObjectType.Honey:
            hungerReductionAmount = 20f;
                break;
            case InteractiveObjectType.RabbitMeat:
            hungerReductionAmount = 30f;
                break;
            case InteractiveObjectType.DeerMeat:
            hungerReductionAmount = 50f;
                break;
            case InteractiveObjectType.HumanMeat:
            hungerReductionAmount = 100f;
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("collide!");
            
            if(hungerScript != null)
            {
                hungerScript.ReduceHunger(hungerReductionAmount);
                Destroy(gameObject);
            }
        }
    }
}
