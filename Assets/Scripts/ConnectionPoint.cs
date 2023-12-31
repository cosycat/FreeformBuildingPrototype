using System;
using JetBrains.Annotations;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour {
    
    private const bool ShowConnections = true;
    
    public ConnectionController ConController => ConnectionController.Instance;
    private SpriteRenderer _spriteRenderer;

    
    [SerializeField] private ConnectionType type;
    [SerializeField] private Direction direction;

    public Vector2 Location => transform.position;
    public ConnectionType Type => type;
    public Direction Direction => direction;

    [CanBeNull] public Connection ConnectedConnection { get; set; }
    public bool IsConnected => ConnectedConnection != null;
    
    public enum ConnectionType {
        Input,
        Output,
        Bidirectional
    }

    private void Start() {
        ConController.AddConnectionPoint(this);

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (!ShowConnections) {
            _spriteRenderer.enabled = false;
        }
        else {
            _spriteRenderer.color = GetSpriteRendererColor();
        }
    }

    private void OnDestroy() {
        ConController.RemoveConnectionPoint(this); // NOT TESTED
    }
    
    public bool IsCompatibleWith(ConnectionPoint other) {
        if (other.Type == ConnectionType.Bidirectional) return true;
        return Type switch {
            ConnectionType.Bidirectional => true,
            ConnectionType.Input => other.Type == ConnectionType.Output,
            ConnectionType.Output => other.Type == ConnectionType.Input,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Color GetSpriteRendererColor() {
        return Type switch {
            ConnectionType.Input => Color.red,
            ConnectionType.Output => Color.green,
            ConnectionType.Bidirectional => Color.blue,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void OnDrawGizmos() {
        Gizmos.color = GetSpriteRendererColor();
        Gizmos.DrawSphere(Location, 0.1f);
        Gizmos.DrawLine(Location, Location + Direction.Vector * 0.3f);
    }
    
    public void StartHighlight() {
        _spriteRenderer.transform.localScale *= 1.5f;
    }

    public void StopHighlight() {
        _spriteRenderer.transform.localScale /= 1.5f;
    }
}