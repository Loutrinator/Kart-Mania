using Kart;
using UnityEngine;

namespace Player
{
    public class PlayerController 
    {
        [HideInInspector] public PlayerRaceInfo info;
        private IActions _actionsOutputs;
        private KartBase _kart;

        public PlayerController(PlayerRaceInfo raceInfo, IActions actions)
        {
            _actionsOutputs = actions; 
            info = raceInfo;
            _kart = info.kart;
            // automatically update when a player choose antoher kart
            info.onKartChange += () => { _kart = info.kart; };
        }

        public void Update()
        {
            //TODO : inputs kart selection menu
            if (!GameManager.Instance.raceHasBegan()) return;
            _kart.forwardMove = _actionsOutputs.Accelerate();
            _kart.hMove = _actionsOutputs.Steer();
            _kart.drift = _actionsOutputs.Drift();
            if (_actionsOutputs.ItemKeyHold())
            {
                info.item?.onKeyHold();
            }
            else if (_actionsOutputs.ItemKeyDown())
            {
                info.item?.onKeyDown();
            }
            else if (_actionsOutputs.ItemKeyUp())
            {
                info.item?.onKeyUp();
            }
        }
    }
}