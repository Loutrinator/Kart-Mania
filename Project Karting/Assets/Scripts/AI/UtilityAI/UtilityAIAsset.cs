using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "UtilityAIAsset", menuName = "ScriptableObjects/UtilityAI/UtilityAI Asset")]
    public class UtilityAIAsset : ScriptableObject
    {
        [SerializeField] public List<UtilityAIActionGroup> actionGroups;

        [SerializeField, HideInInspector]
        public int currentTab = 0;

        public List<List<List<float>>> GetGenome() {
            var genome = new List<List<List<float>>>();
            Debug.Log(actionGroups.Count);
            foreach (var actionGroup in actionGroups) {
                var actionGroupData = new List<List<float>>();
                foreach (var action in actionGroup.actions) {
                    var actionData = new List<float>();
                    foreach (var evaluationFunction in action.evaluationFunctions) {
                        actionData.Add(evaluationFunction.coefficient);
                    }
                    actionGroupData.Add(actionData);
                }
                genome.Add(actionGroupData);
            }
            return genome;
        }
    }

    [Serializable]
    public class UtilityAIActionGroup
    {
        public string groupName = "GROUP";
        public List<UtilityAIAction> actions;

        public UtilityAIActionGroup()
        {
            groupName = "New Group";
            actions = new List<UtilityAIAction>();
        }
    }
}
