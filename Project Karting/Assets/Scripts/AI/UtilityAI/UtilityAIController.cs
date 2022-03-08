using System.Collections.Generic;
using Kart;
using UnityEngine;

namespace AI.UtilityAI
{
    public class UtilityAIController : AIController
    {
        [SerializeField] private UtilityAIAsset utilityAIAsset;
        public override AIAction tick()
        {
            
            UtilityAIAction selectedAction = utilityAIAsset.actions[0];
            float utilityMax = selectedAction.getUtility(kart);

            for (int i = 1; i < utilityAIAsset.actions.Count; i++)
            {
                UtilityAIAction action = utilityAIAsset.actions[i];
                float actionUtility = action.getUtility(kart);
                if(actionUtility > utilityMax){
                    selectedAction = action;
                    utilityMax = actionUtility;
                }
            }

            return selectedAction;
        }

        public void debug()
        {
            
        }
        
    }
}