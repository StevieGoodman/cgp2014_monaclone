using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public bool returnToBar = true;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        
        if(returnToBar)
            LevelManager.ChangeScene("Bar Scene");
        else
            LevelManager.Instance.ChangeScene();
    }
}
