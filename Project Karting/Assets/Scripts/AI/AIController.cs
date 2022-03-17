using AI.UtilityAI;
using Kart;
using UnityEngine;
namespace AI
{
    public abstract class AIController : MonoBehaviour

    {
        [HideInInspector] public KartBase kart;
        public abstract AIAction tick();
    }
}