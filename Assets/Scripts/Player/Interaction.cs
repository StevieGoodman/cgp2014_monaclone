using Unity.Mathematics;
using UnityEngine;

public class Interaction : MonoBehaviour
{

    [Header("Prefab References")] 
    [SerializeField] private GameObject interactionPromptAsset;

    [Header("GameObject References")] [SerializeField]
    public InteractionPrompt interactionPrompt;
    
    public void Begin(Vector3 position, float duration)
    {
        Debug.Log("HellO!");
        GameObject prompt = Instantiate(interactionPromptAsset, position, quaternion.identity);
        prompt.GetComponent<InteractionPrompt>()?.OnInteractionStart(duration);
        interactionPrompt = prompt.GetComponent<InteractionPrompt>();
    }

    public void Begin(GameObject parent, float duration)
    {

        GameObject prompt = Instantiate(interactionPromptAsset, parent.transform.position, quaternion.identity, parent.transform);
        prompt.GetComponent<InteractionPrompt>()?.OnInteractionStart(duration);
        Debug.Log(prompt);
        interactionPrompt = prompt.GetComponent<InteractionPrompt>();
    }
}