using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ConnectionVisualizer : MonoBehaviour {
    
    private const float Resolution = 10; // points per 1 unit
    private const float Width = 0.1f;

    private Connection _connection;
    private SplineContainer _splineContainer;
    private Spline _spline;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    public bool IsInitialized { get; private set; } = false;
    public bool IsPreviewOnly => _connection == null;

    #region Creation
    
    /// <summary>
    /// Creates a connection visualizer between two points, and sets the connection.
    /// </summary>
    /// <see cref="Create(Vector2, Vector2)"/>
    /// <param name="connection"> The connection to visualize. </param>
    /// <returns> The created connection visualizer. </returns>
    public static ConnectionVisualizer Create(Connection connection) {
        var connectionVisualizer = Create(connection.StartConnectionPoint.Location, connection.StartConnectionPoint.Direction, connection.EndConnectionPoint.Location, connection.EndConnectionPoint.Direction);
        connectionVisualizer._connection = connection;
        connection.OnConnectionRemoved += () => Destroy(connectionVisualizer.gameObject);
        return connectionVisualizer;
    }

    /// <summary>
    /// Creates a connection visualizer between two points, without setting a connection.
    /// Use this for temporary connections, like for previewing a connection when building one.
    /// Destroying the visualizer is up to the caller.
    /// </summary>
    /// <see cref="Create(Connection)"/>
    /// <param name="startPoint"> The start point of the connection. </param>
    /// <param name="startDirection"> The direction of the start point. </param>
    /// <param name="endPoint"> The end point of the connection. </param>
    /// <param name="endDirectionFrom"> The direction of the spline coming into the end point. </param>
    /// <returns> The created connection visualizer. </returns>
    public static ConnectionVisualizer Create(Vector2 startPoint, Direction startDirection, Vector2 endPoint, Direction endDirectionFrom) {
        var connectionVisualizer = new GameObject("Connection").AddComponent<ConnectionVisualizer>();
        connectionVisualizer.Visualize(startPoint, startDirection, endPoint, endDirectionFrom);
        connectionVisualizer.IsInitialized = true;
        return connectionVisualizer;
    }

    private void Visualize(Vector2 startPoint, Direction startDirection, Vector2 endPoint, Direction endDirection) {
        _splineContainer ??= gameObject.AddComponent<SplineContainer>(); // if we update the visualization, we don't want to add all these components again
        _spline = _splineContainer.Spline ?? _splineContainer.AddSpline();
        _meshFilter ??= gameObject.AddComponent<MeshFilter>();
        _meshRenderer ??= gameObject.AddComponent<MeshRenderer>();

        // TODO Rotation of Connection
        var startDirectionVector = startDirection.Vector;
        var endDirectionVector = endDirection.Vector;
        var connectionKnot = new BezierKnot(new Vector3(startPoint.x,startPoint.y, 0), -new float3(startDirectionVector.x, startDirectionVector.y, 0), new float3(startDirectionVector.x, startDirectionVector.y, 0));
        var pointerKnot = new BezierKnot(new Vector3(endPoint.x, endPoint.y, 0), new float3(endDirectionVector.x, endDirectionVector.y, 0), -new float3(endDirectionVector.x, endDirectionVector.y, 0));
        switch (_spline.Count) {
            case 0:
                _spline.Add(connectionKnot);
                _spline.Add(pointerKnot);
                break;
            case 2:
                _spline[0] = connectionKnot;
                _spline[1] = pointerKnot;
                break;
            default:
                Debug.LogWarning($"ConnectionVisualizer has {_spline.Count} knots, but should have 0 or 2. This should never happen Clearing the spline and adding the new knots.");
                _spline.Clear();
                _spline.Add(connectionKnot);
                _spline.Add(pointerKnot);
                break;
        }

        // Get all points on the spline
        var vertsP0 = new List<Vector3>();
        var vertsP1 = new List<Vector3>();
        var step = 1f / Resolution;
        for (var i = 0; i < Resolution; i++) {
            var t = step * i;
            SampleSplineWidth(t, out var p0, out var p1);
            vertsP0.Add(p0);
            vertsP1.Add(p1);
        }
        
        // BUILD MESH
        var mesh = new Mesh();
        var verts = new List<Vector3>();
        var tris = new List<int>();
        var length = vertsP1.Count; //Iterate verts and build a face

        for (int i = 0; i < length; i++) {
            var p0 = vertsP0[i];
            var p1 = vertsP1[i];
            var p2 = vertsP0[(i + 1) % length];
            var p3 = vertsP1[(i + 1) % length];
            // var p2 = (i == length - 1) ? m_vertsP1[0] : m_vertsP1[i + 1]; // same code as above,
            // var p3 = (i == length - 1) ? m_vertsP2[0] : m_vertsP2[i + 1]; // different logic

            var offset = 4 * i;
            
            var t1 = offset + 0;
            var t2 = offset + 2;
            var t3 = offset + 3;
            
            var t4 = offset + 3;
            var t5 = offset + 1;
            var t6 = offset + 0;
            
            verts.AddRange(new List<Vector3>{p0, p1, p2, p3});
            tris.AddRange(new List<int>{t1, t2, t3, t4, t5, t6});
        }
        
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        _meshFilter.mesh = mesh;
    }

    private void SampleSplineWidth(float t, out Vector3 left, out Vector3 right) {
        _spline.Evaluate(t, out var position, out var forward, out var up);
        var rightDir = Vector3.Cross(forward, up).normalized;
        right = (Vector3)position + (rightDir * Width);
        left = (Vector3)position + (-rightDir * Width);
    }
    
    #endregion


    #region Update

    /// <summary>
    /// Updates the Visualizer to match the connection.
    /// Only used, when there is no corresponding connection.
    /// </summary>
    /// <param name="startPoint"> The start point of the connection. </param>
    /// <param name="startDirection"> The direction of the start point. </param>
    /// <param name="endPoint"> The end point of the connection. </param>
    /// <param name="endDirection"> The direction of the spline coming into the end point. </param>
    public void UpdatePositions(Vector2 startPoint, Direction startDirection, Vector2 endPoint, Direction endDirection) {
        if (!IsPreviewOnly) {
            Debug.LogWarning("Updating positions of a connection visualizer that has a connection. This is not allowed. The ConnectionVisualizer will update itself upon Connection update.");
            return;
        }
        Visualize(startPoint, startDirection, endPoint, endDirection);
    }

    #endregion

    
}