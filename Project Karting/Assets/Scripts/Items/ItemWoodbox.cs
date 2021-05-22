using Items;
using UnityEngine;

[CreateAssetMenu(fileName="WoodBox",menuName="ScriptableObject/Items/Woodbox",order=0)]
public class ItemWoodbox : Item
{
    [SerializeField] private WoodBox prefab;
    [SerializeField] private float distanceFromKartBack = 5f;

    public override void OnKeyDown(PlayerRaceInfo info) {
        
    }

    public override void OnKeyUp(PlayerRaceInfo info)
    {
        // Calling this will change info.ItemIsUsing and invoke info.onItemUsed
        // who call wb.Throw ( cf line 22 ). 
        Use(info); 
    }

    public override void OnKeyHold(PlayerRaceInfo info)
    {
        if(info.ItemIsInUse) return;
        Transform transform = info.kart.transform;
        WoodBox wb = Instantiate(prefab, transform.position - transform.forward*distanceFromKartBack, Quaternion.identity, transform);
        info.onItemUsed += wb.Throw;
        info.ItemIsInUse = true;
    }
}
