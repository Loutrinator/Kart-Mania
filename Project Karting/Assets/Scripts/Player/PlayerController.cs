using Kart;
using UnityEngine;

namespace Player
{
    public class PlayerController 
    {
        [HideInInspector] private PlayerRaceInfo _info;
        private IActions _actionsOutputs;

        public PlayerController(PlayerRaceInfo raceInfo, IActions actions)
        {
            _actionsOutputs = actions; 
            _info = raceInfo;
        }

        public void Update()
        {
            //TODO : inputs kart selection menu
            if (!GameManager.Instance.raceHasBegan()) return;
            _info.kart.forwardMove = _actionsOutputs.Accelerate();
            _info.kart.hMove = _actionsOutputs.Steer();
            _info.kart.drift = _actionsOutputs.Drift();
            if (_actionsOutputs.ItemKeyHold())
            {
                _info.item?.onKeyHold();
            }
            else if (_actionsOutputs.ItemKeyDown())
            {
                _info.item?.onKeyDown();
            }
            else if (_actionsOutputs.ItemKeyUp())
            {
                _info.item?.onKeyUp();
            }
        }
    }
}