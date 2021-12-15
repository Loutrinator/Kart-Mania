using LevelEditor;
using TMPro;
using UnityEngine;

namespace UI {
    public class LevelEditorManager : MonoBehaviour {
        public GameObject firstPanel;   // new circuit or load circuit
        public GameObject newCircuitPanel;  // ask for name of new circuit
        public GameObject circuitSelectionPanel;    // load circuit by selecting from a list
        public CircuitUIPreview selectedCircuitPreview;
        public GameObject roadNavigationPanel;
        public GameObject roadSectionPanel;

        private CraftedCircuit _currentCircuit;

        private void Awake() {
            circuitSelectionPanel.SetActive(false);
            roadNavigationPanel.SetActive(false);
            roadSectionPanel.SetActive(false);
            
            firstPanel.SetActive(true);
        }

        public void Init(CraftedCircuit circuit) {
            
        }

        public void OnCreateNewCircuitOptionClicked() {
            firstPanel.SetActive(false);
            newCircuitPanel.SetActive(true);
        }

        public void OnCreateNewCircuitValidated(TMP_InputField levelNameField) {
            // todo create circuit, loading screen, then road navigation panel
            if (string.IsNullOrEmpty(levelNameField.text)) return;
            
            newCircuitPanel.SetActive(false);
            GameObject circuit = new GameObject("Circuit") {
                transform = {
                    position = Vector3.zero
                }
            };
            _currentCircuit = circuit.AddComponent<CraftedCircuit>();
            _currentCircuit.circuitData = new CircuitData {
                circuitName = levelNameField.text
            };
            roadNavigationPanel.gameObject.SetActive(true);
        }

        public void OnLoadCircuitOptionClicked() {
            firstPanel.SetActive(false);
            circuitSelectionPanel.SetActive(true);
        }

        public void OnLoadSelectedCircuitClicked() {
            circuitSelectionPanel.SetActive(false);
            Init(selectedCircuitPreview.selectedCircuit);
        }
    }
}
