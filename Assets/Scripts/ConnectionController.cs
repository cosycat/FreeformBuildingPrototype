using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class ConnectionController : MonoBehaviour {
    
    public static ConnectionController Instance { get; private set; }

    private readonly List<ConnectionPoint> _connectionPoints = new();
    private readonly List<Connection> _connections = new();

    private GameObject _connectionGameObject;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
        _connectionGameObject = new GameObject("Connections");
    }

    public void AddConnectionPoint(ConnectionPoint connectionPoint) {
        _connectionPoints.Add(connectionPoint);
    }
    
    public void RemoveConnectionPoint(ConnectionPoint connectionPoint) {
        _connectionPoints.Remove(connectionPoint);
    }
    
    public bool GetNearestConnectionPoint(Vector2 position, out ConnectionPoint connectionPoint, out float distance) {
        if (_connectionPoints.Count == 0) {
            distance = 0;
            connectionPoint = null;
            return false;
        }
        var nearestConnection = _connectionPoints[0];
        var nearestDistance = Vector2.Distance(position, nearestConnection.Location);
        foreach (var possibleConnectionPoint in _connectionPoints) {
            var currDist = Vector2.Distance(position, possibleConnectionPoint.Location);
            if (currDist < nearestDistance) {
                nearestConnection = possibleConnectionPoint;
                nearestDistance = currDist;
            }
        }
        distance = nearestDistance;
        connectionPoint = nearestConnection;
        return true;
    }
    
    
    public bool GetNearestConnectionPointOfType(Vector2 position, ConnectionPoint.ConnectionType type, out ConnectionPoint connectionPoint,  out float distance) {
        if (_connectionPoints.Count == 0) {
            distance = 0;
            connectionPoint = null;
            return false;
        }
        var nearestConnection = _connectionPoints[0];
        var nearestDistance = Vector2.Distance(position, nearestConnection.Location);
        foreach (var possibleConnectionPoint in _connectionPoints) {
            if (possibleConnectionPoint.Type != type) {
                continue;
            }
            
            var currDist = Vector2.Distance(position, possibleConnectionPoint.Location);
            if (currDist < nearestDistance) {
                nearestConnection = possibleConnectionPoint;
                nearestDistance = currDist;
            }
        }
        distance = nearestDistance;
        connectionPoint = nearestConnection;
        return true;
    }
    
    public bool GetNearestConnectionPointCompatibleWith(Vector2 position, ConnectionPoint compatibleConnectionPoint, out ConnectionPoint connectionPoint, out float distance) {
        if (_connectionPoints.Count == 0) {
            distance = 0;
            connectionPoint = null;
            return false;
        }
        var nearestConnection = _connectionPoints[0];
        var nearestDistance = Vector2.Distance(position, nearestConnection.Location);
        foreach (var otherConnectionPoint in _connectionPoints) {
            if (!compatibleConnectionPoint.IsCompatibleWith(otherConnectionPoint)) {
                continue;
            }
            
            var currDist = Vector2.Distance(position, otherConnectionPoint.Location);
            if (currDist < nearestDistance) {
                nearestConnection = otherConnectionPoint;
                nearestDistance = currDist;
            }
        }
        distance = nearestDistance;
        connectionPoint = nearestConnection;
        return true;
    }

    public void CreateConnection(ConnectionPoint startConnectionPoint, ConnectionPoint endConnectionPoint) {
        if (!startConnectionPoint.IsCompatibleWith(endConnectionPoint)) {
            Debug.LogError($"Cannot create connection between incompatible connection points {startConnectionPoint} and {endConnectionPoint}");
            return;
        }
        var connection = new Connection(startConnectionPoint, endConnectionPoint);
        // TODO display connection
        ConnectionVisualizer.Create(connection); // FIXME: when connection is destroyed, visualizer needs to be destroyed too.
                                                              // Currently there is no link between them.
        _connections.Add(connection);
    }
}