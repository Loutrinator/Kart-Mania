using System.Collections.Generic;
using UnityEngine;

namespace AI.UtilityAI
{
    public class UtilityAIController : AIController
    {
        [SerializeField] private UtilityAIAsset utilityAIAsset;
        public override AIAction tick()
        {/*
            List<UAIAction> actions = utilityAIAsset.Actions;
            UAIAction selectedAction = actions[0];
            float utilityMax = selectedAction.getUtility();
            for (int i = 1; i < actions.Count; i++)
            {
                UAIAction action = actions[i];
                float actionUtility = action.getUtility();
                if(actionUtility > utilityMax){
                    selectedAction = action;
                    utilityMax = actionUtility;
                }
            }
            //performing the action
            /*if(utilityMax > 0f){
                selectedAction.performAction();
            } else if (defaultAction != null)
            {
                defaultAction.performAction();
            }*/
            return null;
        }

        public void debug()
        {
            
        }
        
    }
}