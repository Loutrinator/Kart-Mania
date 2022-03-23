using System;
using System.Collections.Generic;
using Kart;
using UnityEngine;

namespace AI.UtilityAI
{
    public class UtilityAIController : AIController
    {
        [SerializeField] public UtilityAIAsset utilityAIAsset;
        private float[] values;
        private int selectedId = 0;
        private int valuesUpdated = 0;
        
        private List<string> actionNames = new List<string>();

        public void Init()
        {
            int size = 0;
            foreach (var actionGroup in utilityAIAsset.actionGroups)
            {
                size += actionGroup.actions.Count;
            }
            values = new float[size];
            
            actionNames = new List<string>();
            foreach (var actiongroup in utilityAIAsset.actionGroups)
            {
                
                foreach (var action in actiongroup.actions)
                {
                
                    actionNames.Add(action.actionName);
                }
            }
        }

        public override List<AIAction> tick()
        {
            valuesUpdated = 0;
            List<AIAction> selectedActions = new List<AIAction>();
            foreach (var actionGroup in utilityAIAsset.actionGroups)
            {
                selectedActions.Add(BestActionForGroup(actionGroup));
            }

            return selectedActions;
        }

        private AIAction BestActionForGroup(UtilityAIActionGroup actionGroup)
        {
            
            UtilityAIAction selectedAction = actionGroup.actions[0];
            float utilityMax = selectedAction.getUtility(kart);
            values[valuesUpdated] = utilityMax;
            int selected = 0;
            valuesUpdated++;
            for (int i = 1; i < actionGroup.actions.Count; i++)
            {
                UtilityAIAction action = actionGroup.actions[i];
                float actionUtility = action.getUtility(kart);
                values[valuesUpdated] = actionUtility;
                valuesUpdated++;
                if(actionUtility > utilityMax){
                    selectedAction = action;
                    utilityMax = actionUtility;
                    selected = i;
                }
            }
            selectedId = selected;
            
            return selectedAction;
        }

        public override List<string> getActionNames()
        {
            return actionNames;
        }

        public override float[] getActionValues()
        {
            return values;
        }

        public override int getSelectedActionId()
        {
            return selectedId;
        }

        public void debug()
        {
            
        }
        
    }
}