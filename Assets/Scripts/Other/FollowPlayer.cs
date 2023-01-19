using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform _player;
    public Vector3 offset;
    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }

    // Follow the player with an offset if defined.
    void Update()
    {
        transform.position = _player.position + offset;
    }
}
