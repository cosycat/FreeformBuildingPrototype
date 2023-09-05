using System;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// For Debugging purposes only.
/// </summary>
public class ConnectionLineDisplayer : MonoBehaviour {
    
    private LineRenderer _lineRenderer;
    private Vector2 _start;
    private Vector2 _end;

    private void Start() {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.widthMultiplier = 0.1f;
    }

    private void Update() {
        var splineContainer = GetComponent<SplineContainer>();
        _start = new Vector2(splineContainer.Spline[0].Position.x, splineContainer.Spline[0].Position.y);
        _end = new Vector2(splineContainer.Spline[1].Position.x, splineContainer.Spline[1].Position.y);
        _lineRenderer.SetPosition(0, _start);
        _lineRenderer.SetPosition(1, _end);
    }

    private void OnDestroy() {
        Debug.Log("Destroying ConnectionLine");
        Destroy(_lineRenderer);
    }
}