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

    private WinterReserves hungerScript;

    private float hungerReductionAmount;

    public GameObject sfxPrefab;

    // Start is called before the first frame update
    void Start()
    {
        hungerScript = FindObjectOfType<WinterReserves>();
        if(hungerScript == null)
        {
            //Debug.LogError("Hunger script not found!");
        }

        switch (objectType)
        {
            case InteractiveObjectType.Berry:
            hungerReductionAmount = 1f;
                break;
            case InteractiveObjectType.Honey:
            hungerReductionAmount = 2f;
                break;
            case InteractiveObjectType.RabbitMeat:
            hungerReductionAmount = 5f;
                break;
            case InteractiveObjectType.DeerMeat:
            hungerReductionAmount = 10f;
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
                Instantiate(sfxPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
