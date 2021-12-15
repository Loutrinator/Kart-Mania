using System;
using LevelEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class CircuitUIPreview : MonoBehaviour {
        public Image icon;
        public TextMeshProUGUI textMeshPro;
        public GameObject parent;

        [NonSerialized] public CraftedCircuit selectedCircuit;

        public void UpdatePreview(CraftedCircuit circuitP) {
            selectedCircuit = circuitP;

            if (selectedCircuit != null) {
                parent.SetActive(true);
                
                icon.sprite = selectedCircuit.circuitData.image;
                textMeshPro.text = selectedCircuit.circuitData.circuitName;
            }
        }
    }
}
