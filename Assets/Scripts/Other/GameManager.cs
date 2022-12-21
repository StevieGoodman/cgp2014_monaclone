using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{   // The purpose of the game manager is to control things like the player reference, timer and stuff

    public GameObject player;
    public float timeRemaining;
    [Space]
    public static GameManager Instance;
    
    private void Awake()
    {
        Instance = this;
        player = GameObject.FindWithTag("Player");
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public Transform GetPlayerTransform()
    {
        return player.transform;
    }
    
    
}
