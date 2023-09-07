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
        
        
        private bool IsDragging => _connectionVisualizer != null;
        // [CanBeNull] private SplineContainer _currDraggingSplineContainer;
        // [CanBeNull] private Spline DraggingSpline => _currDraggingSplineContainer == null ? null : _currDraggingSplineContainer.Spline;
        [CanBeNull] private ConnectionPoint _draggingStartConnectionPoint;
        [CanBeNull] private ConnectionVisualizer _connectionVisualizer;


        [CanBeNull] private ConnectionPoint _connectionPointInRangeOfPointer;
        /// <summary>
        /// The connection point nearest to where the pointer is currently at.
        /// </summary>
        [CanBeNull]
        public ConnectionPoint ConnectionPointInRangeOfPointer {
            get => _connectionPointInRangeOfPointer;
            private set {
                if (_connectionPointInRangeOfPointer != null) _connectionPointInRangeOfPointer.StopHighlight();
                _connectionPointInRangeOfPointer = value;
                if (_connectionPointInRangeOfPointer != null) _connectionPointInRangeOfPointer.StartHighlight();
            }
        }

        /// <summary>
        /// Cancels the current dragging operation, if any.
        /// </summary>
        public void CancelDragging() {
            if (!IsDragging) return;
            StopDragging(Vector2.zero, true);
        }

        /// <summary>
        /// Updates which connection point is currently nearest to the pointer (if any).
        /// Will highlight the connection point when CurrConnectionPointInRange is set.
        /// </summary>
        /// <param name="currentPointerPosition"></param>
        private void UpdateConnectionInRange(Vector2 currentPointerPosition) {
            if (!connectionController.GetNearestConnectionPoint(currentPointerPosition, out var nearestConnection,
                    out var distance)) return;
            // Debug.Log($"Distance to nearest connection: {distance}; connection: {nearestConnection}");
            if (distance < snapDistance) {
                if (ConnectionPointInRangeOfPointer == nearestConnection) return;
                ConnectionPointInRangeOfPointer = nearestConnection;
            }
            else {
                ConnectionPointInRangeOfPointer = null;
            }
        }

        private void StartDragging(Vector2 currentPointerPosition) {
            if (ConnectionPointInRangeOfPointer == null) return;
            // Create a spline from the current connection to the pointer
            _draggingStartConnectionPoint = ConnectionPointInRangeOfPointer;
            // var go = new GameObject();
            // _currDraggingSplineContainer = go.AddComponent<SplineContainer>();
            // if (_currDraggingSplineContainer!.Spline == null) _currDraggingSplineContainer.AddSpline();
            // // TODO Rotation of Connection
            // var connectionKnot = new BezierKnot(new Vector3(CurrConnectionPointInRange.Location.x, CurrConnectionPointInRange.Location.y, 0));
            // var pointerKnot = new BezierKnot(new Vector3(currentPointerPosition.x, currentPointerPosition.y, 0));
            // DraggingSpline.Add(connectionKnot);
            // DraggingSpline.Add(pointerKnot);
            _connectionVisualizer = ConnectionVisualizer.Create(ConnectionPointInRangeOfPointer.Location, currentPointerPosition);
            // connectionVisualizer.transform.SetParent(go.transform); // so it gets destroyed when the dragging spline is destroyed
            // go.AddComponent<ConnectionLineDisplayer>();
        }

        private void UpdateDragging(Vector2 currentPointerPosition) {
            if (!AssertCorrectnessWhileDragging()) return;
            _connectionVisualizer.UpdatePositions(_draggingStartConnectionPoint.Location, currentPointerPosition);
            
            // var lastKnot = DraggingSpline[^1];
            // lastKnot.Position = new Vector3(currentPointerPosition.x, currentPointerPosition.y, 0);
            // DraggingSpline[^1] = lastKnot;
        }

        private void StopDragging(Vector2 currentPointerPosition, bool cancel = false) {
            if (!AssertCorrectnessWhileDragging()) return;
            
            // Check if the pointer is close enough to a connection point
            if (cancel || !connectionController.GetNearestConnectionPointCompatibleWith(currentPointerPosition,
                    _draggingStartConnectionPoint, out var nearestConnectionPoint,
                    out var distance)) {
                // If not, destroy the spline and return
                DestroyVisualization();
                return;
            }

            if (distance > snapDistance) {
                return;
            }
            // If so, create a connection between the two connection points
            ConnectionController.Instance.CreateConnection(_draggingStartConnectionPoint, nearestConnectionPoint);
            DestroyVisualization();
            return;

            void DestroyVisualization() {
                // if (_currDraggingSplineContainer != null) Destroy(_currDraggingSplineContainer.gameObject);
                // _currDraggingSplineContainer = null;
                if (_connectionVisualizer != null) Destroy(_connectionVisualizer.gameObject);
                _connectionVisualizer = null;
                _draggingStartConnectionPoint = null;
            }
        }

        private bool AssertCorrectnessWhileDragging() {
            if (_draggingStartConnectionPoint == null || _connectionVisualizer == null) {
                Debug.LogError($"ConnectionConstructor is dragging but has no start connection point or connection visualizer: {_draggingStartConnectionPoint}, {_connectionVisualizer}");
                return false;
            }
            return true;
        }

        #region Constructor Methods

        public override void OnMovePointerTo(Vector2 currentPointerPosition) {
            if (IsDragging) {
                UpdateDragging(currentPointerPosition);
            }
            
            UpdateConnectionInRange(currentPointerPosition);
        }

        public override void OnRotate(float value) {
            Debug.Log("TODO: Rotate connection");
        }

        public override void OnPlace(Vector2 currentPointerPosition) {
            if (IsDragging) {
                StopDragging(currentPointerPosition);
            }
            else {
                StartDragging(currentPointerPosition);
            }
        }

        public override void End() {
            ConnectionPointInRangeOfPointer = null;
            if (IsDragging) {
                StopDragging(Vector2.zero, false);
            }
        }

        public override void Begin(int subMode) {
            
        }

        public override ConstructionMode ConstructionMode => ConstructionMode.Connection;
        
        #endregion

    }
}