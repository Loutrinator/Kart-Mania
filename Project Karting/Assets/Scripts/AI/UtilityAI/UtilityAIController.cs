using System.Collections.Generic;
using UnityEngine;
using AIGenome = System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<float>>>;

namespace AI.UtilityAI
{
    public class UtilityAIController : AIController
    {
        public AIGenome genome;
        [SerializeField] public UtilityAIAsset utilityAIAsset;
        
        private float[] values;
        private int[] selectedIds;
        private int valuesUpdated = 0;
        
        private List<string> actionNames = new List<string>();

        public void Init(AIGenome genomeP) {
            selectedIds = new int[utilityAIAsset.actionGroups.Count];
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

            genome = genomeP;
        }

        public override List<AIAction> tick()
        {
            valuesUpdated = 0;
            List<AIAction> selectedActions = new List<AIAction>();
            for (var index = 0; index < utilityAIAsset.actionGroups.Count; ++index) {
                var actionGroup = utilityAIAsset.actionGroups[index];
                selectedActions.Add(BestActionForGroup(actionGroup, index, out selectedIds[index]));
            }

            return selectedActions;
        }

        private AIAction BestActionForGroup(UtilityAIActionGroup actionGroup, int actionGroupIndex, out int selectedId)
        {
            
            UtilityAIAction selectedAction = actionGroup.actions[0];
            float utilityMax = selectedAction.getUtility(kart, genome[actionGroupIndex][0]);
            values[valuesUpdated] = utilityMax;
            int selected = 0;
            valuesUpdated++;
            for (int i = 1; i < actionGroup.actions.Count; i++)
            {
                UtilityAIAction action = actionGroup.actions[i];
                float actionUtility = action.getUtility(kart, genome[actionGroupIndex][i]);
                values[valuesUpdated] = actionUtility;
                valuesUpdated++;
                if(actionUtility > utilityMax){
                    selectedAction = action;
                    utilityMax = actionUtility;
                    selected = i;
                }
            }
            selectedId = selected;
            //Debug.Log(actionGroup.actions[1].actionName);
            
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

        public override int[] getSelectedActionsId()
        {
            return selectedIds;
        }

        public void debug()
        {
            
        }
        
    }
}