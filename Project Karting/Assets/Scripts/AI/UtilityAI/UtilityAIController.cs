using System;
using System.Collections.Generic;
using Kart;
using UnityEngine;

namespace AI.UtilityAI
{
    public class UtilityAIController : AIController
    {
        [SerializeField] private UtilityAIAsset utilityAIAsset;
        private float[] values;
        private int selectedId = 0;

        private void Start()
        {
            values = new float[utilityAIAsset.actions.Count];
        }

        public override AIAction tick()
        {
            
            UtilityAIAction selectedAction = utilityAIAsset.actions[0];
            float utilityMax = selectedAction.getUtility(kart);
            values[0] = utilityMax;
            int selected = 0;

            for (int i = 1; i < utilityAIAsset.actions.Count; i++)
            {
                UtilityAIAction action = utilityAIAsset.actions[i];
                float actionUtility = action.getUtility(kart);
                values[i] = actionUtility;
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
            Debug.Log("NB ACTIONS " + utilityAIAsset.actions.Count);
            List<string> names = new List<string>();
            foreach (var action in utilityAIAsset.actions)
            {
                
                names.Add(action.actionName);
            }

            foreach (var name in names)
            {
                
                Debug.Log("action name " + name);
            }

            return names;
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