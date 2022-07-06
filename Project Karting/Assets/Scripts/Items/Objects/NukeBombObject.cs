using Handlers;
using Items;
using UnityEngine;

public class NukeBombObject : ItemObject
{
    [SerializeField] Nuke nukePrefab;
    public override void ResetItem()
    {
    }

    public override void OnKeyHold(PlayerRaceInfo info) {
            
    }

    public override void OnKeyDown(PlayerRaceInfo info)
    {
        Instantiate(nukePrefab);
        Use(info); 
    }

    public override void OnKeyUp(PlayerRaceInfo info) {
            
    }
}
