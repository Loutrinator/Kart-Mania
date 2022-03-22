using System;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "UtilityAIAsset", menuName = "ScriptableObjects/Utility AI/UtilityAI Asset")]
    public class UtilityAIAsset : ScriptableObject
    {
        [SerializeField] public List<UtilityAIActionGroup> actionGroups;

        [SerializeField, HideInInspector]
        public int currentTab = 0;
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
