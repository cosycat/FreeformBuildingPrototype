using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Construction {
    /// <summary>
    /// Constructor for buildings.
    /// </summary>
    public class BuildingConstructor : Constructor {

        private GameObject _buildingsParent;

        [CanBeNull] private Building _currentBuilding;
        private int _currentBuildingIndex = -1;

        [SerializeField] private List<Building> buildingBlueprints;
        
        public List<string> BuildingNames => buildingBlueprints.ConvertAll(building => building.name);

        public override void OnStart() {
            _buildingsParent = new GameObject("Buildings");
        }

        public override void OnPointerChanged(Vector2 currentPointerPosition, Direction currentPointerRotation) {
            if (_currentBuilding == null) return;
            _currentBuilding.Position = currentPointerPosition;
            _currentBuilding.Rotation = currentPointerRotation;
        }

        public override void OnPlace(Vector2 currentPointerPosition, Direction currentPointerRotation) {
            if (_currentBuilding == null) return;
            var currentRotation = _currentBuilding.Rotation; // make sure the next building to place keeps the same rotation
            _currentBuilding.transform.SetParent(_buildingsParent.transform);
            _currentBuilding.OnPlaced();
            _currentBuilding = null;
            ConstructionController.OnChangeConstructionMode(ConstructionMode.Building, _currentBuildingIndex);
            if (_currentBuilding != null) _currentBuilding.Rotation = currentRotation;
        }

        public override void End() {
            if (_currentBuilding != null) Destroy(_currentBuilding.gameObject);
            _currentBuilding = null;
        }

        public override void Begin(int subMode) {
            if (_currentBuilding != null) {
                Debug.LogWarning($"Building not cleared before beginning new building; current {_currentBuilding.name}, new {buildingBlueprints[subMode].name}");
                Destroy(_currentBuilding.gameObject);
            }
            if (subMode >= 0 && subMode < buildingBlueprints.Count)
                SetBuilding(subMode);
            else
                Debug.LogError($"Building index {subMode} is out of range (0-{buildingBlueprints.Count - 1})");
        }
        
        internal void SetBuilding(int index) {
            if (_currentBuilding != null) Destroy(_currentBuilding.gameObject);
            _currentBuilding = null;
            if (index >= 0 && index < buildingBlueprints.Count) {
                _currentBuilding = Instantiate(buildingBlueprints[index], transform);
                _currentBuildingIndex = index;
            }
            else
                Debug.LogWarning($"Building index {index} is out of range (0-{buildingBlueprints.Count - 1})");
        }

        public override ConstructionMode ConstructionMode => ConstructionMode.Building;
    }
}