using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private Image progressIndicator;

    public void OnInteractionStart(float duration)
    {
        StartCoroutine(FillIndicator());

        IEnumerator FillIndicator()
        {
            while (progressIndicator.fillAmount < 1)
            {
                progressIndicator.fillAmount += Time.deltaTime / duration;
                yield return null;
            }
            OnInteractionComplete();
        }
    }

    public void OnInteractionInterrupt()
    {
        Destroy(gameObject);
    }

    private void OnInteractionComplete()
    {
        OnInteractionInterrupt();
    }
}
