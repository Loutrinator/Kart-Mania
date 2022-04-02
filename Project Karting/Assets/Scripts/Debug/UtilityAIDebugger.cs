using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.UtilityAI;
using Kart;
using UnityEngine;
using UnityEngine.UIElements;

public class UtilityAIDebugger : MonoBehaviour {
    [SerializeField] private PlayerAI AI;

    [SerializeField] private bool showControllerDebugger = false;
    [SerializeField] private ActionDebugger ActionDebuggerPrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private FunctionDebugger FunctionDebuggerPrefab;
    [SerializeField] private Mesh debugArrowMesh;
    [SerializeField] private ControllerDebugger controllerDebugger;

    private List<ActionDebugger> actionDebuggers = new List<ActionDebugger>();
    private List<FunctionDebugger> functionDebuggers = new List<FunctionDebugger>();

    private Vector3 funcDebugPos;
    private float offset = 0;

    private bool _initialized;

    public void Init(PlayerAI ai) {
        AI = ai;
        SetupForAI();
        SetupUtilityAI();
        _initialized = true;
    }

    private void SetupUtilityAI() {
        funcDebugPos = new Vector3(20, 25, 0);
        RectTransform prefabTransform = ActionDebuggerPrefab.GetComponent<RectTransform>();
        offset = prefabTransform.rect.height;
        AddFunctionDebugger("Vitesse");
        AddFunctionDebugger("Distance du centre de la route");
        AddFunctionDebugger("Courbure de la route");
        AddFunctionDebugger("Alignement avec la route");

        //speed, distanceToCenterOfRoad, curvatureOfTheRoad,  alignedToTheRoad, constant, sineNormalized
    }

    private void AddFunctionDebugger(string funcName) {
        funcDebugPos -= Vector3.down * offset;
        FunctionDebugger functionDebugger =
            Instantiate(FunctionDebuggerPrefab, funcDebugPos, Quaternion.identity, container);
        functionDebugger.name = funcName + " Debugger";
        functionDebugger.SetName(funcName);
        functionDebuggers.Add(functionDebugger);
    }

    private void SetupForAI() {
        Vector3 position = new Vector3(20, Screen.height, 0);
        RectTransform prefabTransform = ActionDebuggerPrefab.GetComponent<RectTransform>();
        float offset = prefabTransform.rect.height;

        List<string> names = AI.aiController.getActionNames();
        Debug.Log("NAME COUNT " + names.Count);
        for (int i = 0; i < names.Count; ++i) {
            string actionName = names[i];
            position += Vector3.down * offset;
            ActionDebugger actionDebugger = Instantiate(ActionDebuggerPrefab, position, Quaternion.identity, container);
            actionDebugger.name = actionName + " Debugger";
            actionDebugger.setName(actionName);
            actionDebuggers.Add(actionDebugger);
        }
    }

    private void FixedUpdate() {
        if (!_initialized) return;
        float[] values = AI.aiController.getActionValues();
        int selectedId = AI.aiController.getSelectedActionId();
        int i;
        for (i = 0; i < values.Length; ++i) {
            float value = values[i];
            if (i < actionDebuggers.Count) {
                ActionDebugger debugger = actionDebuggers[i];
                debugger.setValues(value, i == selectedId);
            }
        }

        KartBase kart = AI.kart;
        UpdateFuncDebugger(0, UtilityAIKartBehaviorManager.Instance.SpeedFunction(kart));
        UpdateFuncDebugger(1, UtilityAIKartBehaviorManager.Instance.DistanceToCenterOfRoadFunction(kart));
        UpdateFuncDebugger(2, UtilityAIKartBehaviorManager.Instance.CurvatureOfRoadFunction(kart));
        UpdateFuncDebugger(3, UtilityAIKartBehaviorManager.Instance.AlignedToRoad(kart));

        if (showControllerDebugger) {
            controllerDebugger.gameObject.SetActive(true);
            controllerDebugger.SetInputs(AI.movement);
        }
        else {
            controllerDebugger.gameObject.SetActive(false);
        }
    }

    private void UpdateFuncDebugger(int index, float value) {
        FunctionDebugger debugger = functionDebuggers[index];
        debugger.SetValue(value);
    }

    private void OnDrawGizmos() {
        if (!_initialized) return;
        UtilityAIKartBehaviorManager.Instance.OnDrawGizmos(AI.kart, debugArrowMesh);
    }
}