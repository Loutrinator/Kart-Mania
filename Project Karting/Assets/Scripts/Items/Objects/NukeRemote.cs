using DG.Tweening;
using Handlers;
using Items;
using UnityEngine;

public class NukeRemote : ItemObject
{
    [SerializeField] Nuke nukePrefab;
    private bool used = false;
    public override void ResetItem()
    {
    }

    public override void OnKeyHold(PlayerRaceInfo info) {
            
    }

    public override void OnKeyDown(PlayerRaceInfo info)
    {
        if (used) return;
        Nuke nuke = Instantiate(nukePrefab);
        nuke.target = info.kart.transform;
        Use(info);
        transform.DOScale(Vector3.zero, 0.2f).SetDelay(1f).OnComplete(() => Destroy(gameObject));
        used = true;
    }

    public override void OnKeyUp(PlayerRaceInfo info) {
            
    }
    
}
