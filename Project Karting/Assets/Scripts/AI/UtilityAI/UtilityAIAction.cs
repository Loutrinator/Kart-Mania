using UnityEngine;

namespace AI.UtilityAI
{
    public class UtilityAIAction : ScriptableObject
    {
        public Event action;

        public void performAction()
        {
            action.Use();
        }
    }
}
