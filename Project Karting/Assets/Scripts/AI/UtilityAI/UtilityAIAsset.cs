using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "UtilityAIAsset", menuName = "ScriptableObjects/Utility AI/UtilityAI Asset")]
    public class UtilityAIAsset : ScriptableObject
    {
        [SerializeField] public List<UtilityAIAction> actions;
    }
}
