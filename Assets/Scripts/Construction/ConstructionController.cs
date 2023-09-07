using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Construction {
    /// <summary>
    /// This class is responsible for handling the construction of buildings and connections.
    /// It maintains a list of constructors, each of which is responsible for a specific construction mode.
    /// Upon changing the construction mode, it changes the current constructor.
    /// It mainly handles input and delegates it to the current constructor.
    /// </summary>
    [RequireComponent(typeof(BuildingConstructor), typeof(ConnectionConstructor))]
    public class ConstructionController : MonoBehaviour {
    
        [SerializeField] private float rotationSpeed = 5f;

        public ConstructionMode ConstructionMode => _currentConstructor != null ? _currentConstructor.ConstructionMode : ConstructionMode.None;

        [CanBeNull] private Constructor _currentConstructor;
        private Constructor[] _constructors;
        private Vector2 _currentPointerPosition;
        private Direction _currentPointerRotation;

        private void Start() {
            _constructors = GetComponents<Constructor>();
            foreach (var constructor in _constructors) {
                constructor.OnStart();
            }

            var playerInput = GetComponent<PlayerInput>();
            playerInput.actions["MovePointer"].performed += OnMovePointer;
            playerInput.actions["Rotate"].performed += OnRotate;
            playerInput.actions["Click"].performed += _ => OnClick();
        }

        public void OnChangeConstructionMode(ConstructionMode mode, int subMode = 0) {
            if (_currentConstructor != null) {
                _currentConstructor.End();
                _currentConstructor.IsConstructing = false;
            }
            _currentConstructor = GetConstructorOfType(mode);
            if (_currentConstructor != null) {
                _currentConstructor.IsConstructing = true;
                _currentConstructor.Begin(subMode);
                _currentConstructor.OnPointerChanged(_currentPointerPosition, _currentPointerRotation);
            }
        }
    
        public Constructor GetConstructorOfType(ConstructionMode mode) {
            foreach (var constructor in _constructors) {
                if (constructor.ConstructionMode == mode) return constructor;
            }
            return null;
        }
    
        [CanBeNull]
        public TConstructionMode GetConstructorOfType<TConstructionMode>() where TConstructionMode : Constructor {
            foreach (var constructor in _constructors) {
                if (constructor is TConstructionMode constructionMode) return constructionMode;
            }
            return null;
        }
    

        #region Input
    
        private void OnRotate(InputAction.CallbackContext callbackContext) {
            _currentPointerRotation.Angle += callbackContext.ReadValue<float>() * rotationSpeed;
            if (_currentConstructor != null)
                _currentConstructor.OnRotate(callbackContext.ReadValue<float>() * rotationSpeed);
        }

        private void OnMovePointer(InputAction.CallbackContext callbackContext) {
            if (Input.mousePresent) {
                var mousePos = Input.mousePosition;
                if (Camera.main == null) {
                    Debug.LogError("Main camera is null");
                    return;
                }
                var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                worldPos.z = 0;
                _currentPointerPosition = worldPos;
            }
            else {
                _currentPointerPosition += callbackContext.ReadValue<Vector2>();
            }
            if (_currentConstructor != null)
                _currentConstructor.OnPointerChanged(_currentPointerPosition, _currentPointerRotation);
        }

        private void OnClick() {
            if (_currentConstructor != null)
                _currentConstructor.OnPlace(_currentPointerPosition, _currentPointerRotation);
        }
    
        #endregion

    
    
    }

    public enum ConstructionMode {
        None,
        Building,
        Connection
    }
}