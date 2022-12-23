using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float movementSpeed;

    
    // PRIVATE VARIABLES
    private PlayerInput _playerInput;
    private Vector2 _movementInput;
    private Vector2 _mousePosition;
    private Rigidbody2D _rb2d;
    private Transform _entityTransform;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rb2d = GetComponentInChildren<Rigidbody2D>();
        _entityTransform = _rb2d.transform;
    }

    private void Update()
    {
        ReadInput();
    }

    // Read inputs given by the player to be used in other functions
    private void ReadInput()
    {
        _movementInput = _playerInput.actions["Move"].ReadValue<Vector2>();
        _mousePosition = _playerInput.actions["Look"].ReadValue<Vector2>();
    }
    // Since we are using a rigidbody to move the player. We want to make sure that movements are done consistently via FixedUpdate
    private void FixedUpdate()
    {
        MovePlayer();
        FaceMouse();
    }
    // Move the player with forces
    private void MovePlayer()
    {
        var movementVector = _movementInput * (movementSpeed * 1000);
        _rb2d.AddForce(movementVector * Time.fixedDeltaTime);
    }
    
    private void FaceMouse()
    {
        if (!_entityTransform) return;
        _mousePosition = Camera.main.ScreenToWorldPoint(_mousePosition);
        var direction = new Vector2(_mousePosition.x - _entityTransform.position.x, _mousePosition.y - _entityTransform.position.y);
        //Debug.DrawLine(_entityTransform.position, _mousePosition);
        _rb2d.MoveRotation(GetAngleFromVectorFloat(direction, 90f));
    }
    private static float GetAngleFromVectorFloat(Vector3 dir, float offset)
    {
        dir = dir.normalized;
        var n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return (n + offset);
    }
}
