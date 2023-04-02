using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private Image progressIndicator;

    public void OnInteractionStart(float duration)
    {
        StartCoroutine(FillIndicator());
        IEnumerator FillIndicator()
        {
            while (progressIndicator.fillAmount < 1)
            {
                progressIndicator.fillAmount += Time.deltaTime / duration;
                yield return new WaitForEndOfFrame();
            }
            OnInteractionComplete();
            yield return null;
        }
    }

    public void OnInteractionInterrupt() => Destroy(gameObject);

    private void OnInteractionComplete() => OnInteractionInterrupt();
}
