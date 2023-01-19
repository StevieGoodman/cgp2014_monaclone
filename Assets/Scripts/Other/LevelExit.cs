using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public bool returnToBar = true;
    public string nextLevel;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        saveRepValues();
        if(nextLevel != string.Empty)
            PlayerPrefs.SetString("CurrentLevel", nextLevel);

        if (returnToBar)
        {
            var completionUI = FindObjectOfType<CompletionUI>();
            completionUI.DisplayCompletion();
            Invoke(nameof(changeScene), 5f);
        }
        else
            changeScene();
    }

    private void changeScene()
    {
        if(returnToBar)
            LevelManager.ChangeScene("Bar Scene");
        else
            LevelManager.Instance.ChangeScene();
    }
    private void saveRepValues()
    {

        var player = GameManager.Instance.GetPlayerTransform().root.gameObject;
        
        PlayerPrefs.SetInt("HackRep", (int)player.GetComponent<HackAbility>().Reputation);
        PlayerPrefs.SetInt("DisguiseRep", (int)player.GetComponent<DisguiseAbility>().Reputation);
        PlayerPrefs.SetInt("KnockoutRep", (int)player.GetComponent<KnockoutAbility>().Reputation);
        PlayerPrefs.SetInt("LockPickRep", (int)player.GetComponent<LockPickingAbility>().Reputation);
    }
}
