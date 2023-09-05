using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour {
    
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 _currMovement = Vector2.zero;

    private void OnMove(InputValue input) {
        _currMovement = input.Get<Vector2>();
    }

    private void Update() {
        transform.Translate(_currMovement * (moveSpeed * Time.deltaTime));
    }
}
