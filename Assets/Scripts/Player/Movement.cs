using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    // How fast can the player move?
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

    public void UpdateMousePosition(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
    }
    
    public void UpdateMovementInput(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
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
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(_mousePosition);
        Vector2 direction = new Vector2(worldPoint.x - _entityTransform.position.x, worldPoint.y - _entityTransform.position.y);
        //Debug.DrawLine(_entityTransform.position, worldPoint);
        _rb2d.MoveRotation(GetAngleFromVectorFloat(direction, -90f));
    }
    private static float GetAngleFromVectorFloat(Vector3 dir, float offset)
    {
        dir = dir.normalized;
        var n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return (n + offset);
    }
}
