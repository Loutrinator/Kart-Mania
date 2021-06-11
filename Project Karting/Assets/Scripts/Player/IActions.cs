using UnityEngine.EventSystems;

namespace Player
{
    public interface IActions
    {
        int  Accelerate();
        float Steer();
        bool Drift();
        bool ItemKeyHold();
        bool ItemKeyDown();
        bool ItemKeyUp();
        bool ShowRearCamera();
    }
}