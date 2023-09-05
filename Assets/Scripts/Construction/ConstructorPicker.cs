using System;
using UnityEngine;

namespace Construction {
    
    /// <summary>
    /// Constructor GUI for selecting the construction mode and building type.
    /// </summary>
    public class ConstructorPicker : MonoBehaviour
    {
        private const int Width = 150;
        private const int ButtonHeight = 20;
        private const int PaddingBetween = 10;
        private const int PaddingTop = 30;
        private const int PaddingBottom = 10;
        
        private ConstructionController _constructionController;
        
        private void Start() {
            useGUILayout = true;
            _constructionController = FindObjectOfType<ConstructionController>();
        }

        private void OnGUI() {
            var constructionModeCount = Enum.GetValues(typeof(ConstructionMode)).Length;
            var buttonCount = constructionModeCount + _constructionController.GetConstructorOfType<BuildingConstructor>()!.BuildingNames.Count;
            var height = buttonCount * ButtonHeight + PaddingBetween * 2 + PaddingTop + PaddingBottom;
            GUI.Box(new Rect(PaddingBetween, PaddingBetween, Width, height), "Construction Menu");
            var currButton = 0;
            foreach (ConstructionMode constructionMode in Enum.GetValues(typeof(ConstructionMode))) {
                switch (constructionMode) {
                    case ConstructionMode.None:
                        break;
                    case ConstructionMode.Building:
                        OnBuildingConstructionGUI(ref currButton);
                        break;
                    case ConstructionMode.Connection:
                    default:
                        OnDefaultConstructionGUI(constructionMode, ref currButton);
                        break;
                }
                
            }
        }

        private void OnDefaultConstructionGUI(ConstructionMode constructionMode, ref int currButton) {
            if (GUI.Button(new Rect(PaddingBetween, PaddingTop + PaddingBetween + currButton * ButtonHeight, Width, ButtonHeight), constructionMode.ToString())) {
                Debug.Log($"Construction mode: {constructionMode}");
                _constructionController.OnChangeConstructionMode(constructionMode);
            }
            // if (GUI.Button(new Rect(20, 40 + (int)constructionMode * 30, 80, 20), constructionMode.ToString())) {
            //     Debug.Log($"Construction mode: {constructionMode}");
            //     _constructionController.OnChangeConstructionMode(constructionMode);
            // }
        }

        private void OnBuildingConstructionGUI(ref int currButton) {
            const ConstructionMode constructionMode = ConstructionMode.Building;
            var buildingConstructor = _constructionController.GetConstructorOfType<BuildingConstructor>();
            if (buildingConstructor != null)
                for (var i = 0; i < buildingConstructor.BuildingNames.Count; i++) {
                    var buildingName = buildingConstructor.BuildingNames[i];
                    if (GUI.Button(new Rect(PaddingBetween, PaddingTop + PaddingBetween + currButton * ButtonHeight, Width, ButtonHeight), buildingName)) {
                        Debug.Log($"Building: {buildingName} ({i})");
                        _constructionController.OnChangeConstructionMode(constructionMode, subMode: i);
                    }
                    currButton++;
                }
        }
    }
}
