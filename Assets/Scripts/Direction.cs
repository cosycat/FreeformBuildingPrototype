using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct Direction {
    
    [SerializeField] private float angle;

    public float Angle {
        get => angle;
        set => angle = value;
    }

    public Vector2 Vector => Quaternion.Euler(0, 0, Angle) * Vector2.right;
    
    public Direction(float angle) {
        this.angle = angle;
    }

    public Direction(Vector2 vector) {
        angle = Vector2.SignedAngle(Vector2.right, vector);
    }
}