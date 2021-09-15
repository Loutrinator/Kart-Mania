using System.Collections.Generic;

namespace AI.UtilityAI
{
    public class UtilityAIController : AIController
    {
        private List<UtilityAIAction> actions;
        private UtilityAIAction defaultAction;
        public void tick()
        {
            UtilityAIAction selectedAction = actions[0];
            float utilityMax = selectedAction.getUtility();
            for (int i = 1; i < actions.Count; i++)
            {
                UtilityAIAction action = actions[i];
                float actionUtility = action.getUtility();
                if(actionUtility > utilityMax){
                    selectedAction = action;
                    utilityMax = actionUtility;
                }
            }
            if(utilityMax > 0f){
                selectedAction.performAction();
            } else if (defaultAction != null)
            {
                defaultAction.performAction();
            }
        }

        public void debug()
        {
            
        }
        
    }
}