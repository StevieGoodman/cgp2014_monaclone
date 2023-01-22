using System.Collections;
using UnityEngine;

public class ChaseBehaviour : EnemyBehaviour
{
    // The distance required between the player and this agent to consider the player caught.
    private const float PlayerCatchDistance = 0.75f;

    public override void StartBehaviour() => StartCoroutine(nameof(Chase));

    public IEnumerator Chase()
    {
        // Update our destination to the player
        Agent.SetDestination(GameManager.Instance.GetPlayerTransform().position);
        yield return new WaitForSeconds(Time.fixedDeltaTime);

        if (Vector2.Distance(Agent.transform.position, GameManager.Instance.GetPlayerTransform().position) <= PlayerCatchDistance)
            GameManager.Instance.GameOver("Caught!");
        
        StartBehaviour();
    }
}
