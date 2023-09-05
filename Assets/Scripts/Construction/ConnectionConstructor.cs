using JetBrains.Annotations;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Construction {
    
    /// <summary>
    /// Constructor for connections.
    /// </summary>
    public class ConnectionConstructor : Constructor {
        
        [SerializeField] private float snapDistance = .5f;
        
        private ConnectionController connectionController => ConnectionController.Instance;
        
        private bool IsDragging => DraggingSpline != null;
        [CanBeNull] private SplineContainer _currDraggingSplineContainer;
        [CanBeNull] private Spline DraggingSpline => _currDraggingSplineContainer == null ? null : _currDraggingSplineContainer.Spline;

        [CanBeNull] private ConnectionPoint _currConnectionPointInRange;

        [CanBeNull]
        public ConnectionPoint CurrConnectionPointInRange {
            get => _currConnectionPointInRange;
            private set {
                if (_currConnectionPointInRange != null) _currConnectionPointInRange.StopHighlight();
                _currConnectionPointInRange = value;
                if (_currConnectionPointInRange != null) _currConnectionPointInRange.StartHighlight();
            }
        }

        private void UpdateConnectionInRange(Vector2 currentPointerPosition) {
            if (!connectionController.GetNearestConnectionPoint(currentPointerPosition, out var nearestConnection,
                    out var distance)) return;
            Debug.Log($"Distance to nearest connection: {distance}; connection: {nearestConnection}");
            if (distance < snapDistance) {
                if (CurrConnectionPointInRange == nearestConnection) return;
                CurrConnectionPointInRange = nearestConnection;
            }
            else {
                CurrConnectionPointInRange = null;
            }
        }

        private void StartDragging(Vector2 currentPointerPosition) {
            if (CurrConnectionPointInRange == null) return;
            // Create a spline from the current connection to the pointer
            var go = new GameObject();
            _currDraggingSplineContainer = go.AddComponent<SplineContainer>();
            _currDraggingSplineContainer.AddSpline();
            // TODO Rotation of Connection
            var connectionKnot = new BezierKnot(new Vector3(CurrConnectionPointInRange.Location.x, CurrConnectionPointInRange.Location.y, 0));
            var pointerKnot = new BezierKnot(new Vector3(currentPointerPosition.x, currentPointerPosition.y, 0));
            DraggingSpline.Add(connectionKnot);
            DraggingSpline.Add(pointerKnot);
            go.AddComponent<ConnectionLineDisplayer>();
        }

        private void UpdateDragging(Vector2 currentPointerPosition) {
            if (DraggingSpline == null) {
                Debug.LogError("ConnectionConstructor is dragging but has no spline");
                return;
            }

            if (DraggingSpline.Count < 2) {
                Debug.LogError("ConnectionConstructor is dragging but spline has less than 2 knots");
                return;
            }

            var lastKnot = DraggingSpline[^1];
            lastKnot.Position = new Vector3(currentPointerPosition.x, currentPointerPosition.y, 0);
            DraggingSpline[^1] = lastKnot;
        }

        #region Constructor Methods

        public override void OnMovePointerTo(Vector2 currentPointerPosition) {
            if (IsDragging) {
                UpdateDragging(currentPointerPosition);
            }
            else {
                UpdateConnectionInRange(currentPointerPosition);
            }
        }

        public override void OnRotate(float value) {
            Debug.Log("TODO: Rotate connection");
        }

        public override void OnPlace(Vector2 currentPointerPosition) {
            if (IsDragging) {
                
            }
            else {
                StartDragging(currentPointerPosition);
            }
        }

        public override void End() {
            CurrConnectionPointInRange = null;
            if (IsDragging) {
                if (_currDraggingSplineContainer != null) Destroy(_currDraggingSplineContainer.gameObject);
                _currDraggingSplineContainer = null;
            }
        }

        public override void Begin(int subMode) {
            
        }

        public override ConstructionMode ConstructionMode => ConstructionMode.Connection;
        
        #endregion

    }
}