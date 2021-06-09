namespace Player
{
    public class PlayerController
    {
        private PlayerRaceInfo _info;
        private IActions _actionsOutputs;

        public PlayerController(PlayerRaceInfo raceInfo, IActions actions)
        {
            _actionsOutputs = actions;
            _info = raceInfo;
        }

        public void Update()
        {
            //TODO : inputs kart selection menu
            _info.kart.forwardMove = _actionsOutputs.Accelerate();
            _info.kart.hMove = _actionsOutputs.Steer();
            _info.kart.drift = _actionsOutputs.Drift();
            if (_info.hasItem) {
                if (_actionsOutputs.ItemKeyHold()) _info.Item.OnKeyHold(_info);
                if (_actionsOutputs.ItemKeyDown()) _info.Item.OnKeyDown(_info);
                if (_actionsOutputs.ItemKeyUp()) _info.Item.OnKeyUp(_info);
            }
        }
    }
}