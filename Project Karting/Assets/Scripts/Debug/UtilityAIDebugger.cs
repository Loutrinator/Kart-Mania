using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAIDebugger : MonoBehaviour
{
    [SerializeField] private PlayerAI AI;

    [SerializeField] private ActionDebugger ActionDebuggerPrefab;
    [SerializeField] private RectTransform container;

    private List<ActionDebugger> debuggers = new List<ActionDebugger>();
    private void Start()
    {
        SetupForAI();
    }

    private void SetupForAI()
    {
        Vector3 position = new Vector3(20,Screen.height,0);
        RectTransform prefabTransform = ActionDebuggerPrefab.GetComponent<RectTransform>();
        float offset = prefabTransform.rect.height;

        List<string> names = AI.aiController.getActionNames();
        Debug.Log("NAMES COUNT " + names.Count);
        for (int i = 0; i < names.Count; ++i)
        {
            string actionName = names[i];
            Debug.Log("ACTION NAME : " + actionName);
            position += Vector3.down * offset;
            ActionDebugger actionDebugger = Instantiate(ActionDebuggerPrefab, position, Quaternion.identity, container);
            actionDebugger.name = actionName + " Debugger";
            actionDebugger.setName(actionName);
            debuggers.Add(actionDebugger);
        }
    }

    private void FixedUpdate()
    {
        float[] values = AI.aiController.getActionValues();
        int selectedId = AI.aiController.getSelectedActionId();
        
        for (int i = 0; i < values.Length; ++i)
        {
            float value = values[i];
            if (i < debuggers.Count)
            {
                ActionDebugger debugger = debuggers[i];
                debugger.setValues(value,i == selectedId);
            }
        }
    }
}
