
using System.Collections.Generic;
using AI.UtilityAI;
using Kart;
using UnityEngine;
namespace AI
{
    public abstract class AIController : MonoBehaviour

    {
        [HideInInspector] public KartBase kart;
        public abstract List<AIAction> tick();
        public abstract List<string> getActionNames();
        public abstract float[] getActionValues();
        public abstract int[] getSelectedActionsId();

    }
}