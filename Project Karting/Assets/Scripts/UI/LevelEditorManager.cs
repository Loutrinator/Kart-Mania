using System;
using Game;
using UnityEngine;

namespace UI {
    public class LevelEditorManager : MonoBehaviour {
        public GameObject firstPanel;   // new circuit or load circuit
        public GameObject circuitSelectionPanel;    // load circuit by selecting from a list
        public GameObject roadNavigationPanel;
        public GameObject roadSectionPanel;

        private void Awake() {
            circuitSelectionPanel.SetActive(false);
            roadNavigationPanel.SetActive(false);
            roadSectionPanel.SetActive(false);
            
            firstPanel.SetActive(true);
        }

        public void Init(Race circuit) {
            
        }

        public void OnCreateNewCircuitOptionClicked() {
            firstPanel.SetActive(false);
            // todo create circuit, loading screen, then road navigation panel
        }

        public void OnLoadCircuitOptionClicked() {
            firstPanel.SetActive(false);
            circuitSelectionPanel.SetActive(true);
        }
    }
}
