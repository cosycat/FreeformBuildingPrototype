using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Building : MonoBehaviour {

    private List<ConnectionPoint> _connections;

    private void Start() {
        _connections = new List<ConnectionPoint>(GetComponentsInChildren<ConnectionPoint>());
    }

    public bool IsPlaced { get; private set; } = false;
    public float Rotation {
        get => transform.rotation.eulerAngles.z;
        set => transform.rotation = Quaternion.Euler(0, 0, value);
    }

    public Vector2 Position {
        get => transform.position;
        set => transform.position = value;
    }

    public void OnPlaced() {
        IsPlaced = true;
    }
}