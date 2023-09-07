using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Construction {
    /// <summary>
    /// Base class for all constructors of different construction modes.
    /// </summary>
    public abstract class Constructor : MonoBehaviour {
        
        public ConstructionController ConstructionController { get; private set; }

        public bool IsConstructing { get; internal set; } = false;


        public abstract void OnPointerChanged(Vector2 currentPointerPosition, Direction currentPointerRotation);
        
        public abstract ConstructionMode ConstructionMode { get; }
        public abstract void OnPlace(Vector2 currentPointerPosition, Direction currentPointerRotation);

        public abstract void End();
    
        public abstract void Begin(int subMode);

        public virtual void OnStart() { }

        private void Start() { ConstructionController = GetComponent<ConstructionController>(); }
    }
}