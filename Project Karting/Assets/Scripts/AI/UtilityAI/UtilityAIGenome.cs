using System.Collections.Generic;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "UtilityAIGenome", menuName = "ScriptableObjects/UtilityAI/UtilityAI Genome")]
    public class UtilityAIGenome : ScriptableObject
    {
        [SerializeField] List<List<List<float>>> genes = new List<List<List<float>>>();
    }
}