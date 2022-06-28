﻿using Items;
using UnityEngine;

[CreateAssetMenu(fileName="WoodboxData",menuName="ScriptableObjects/Items/WoodboxData",order=0)]
public class WoodboxData : ItemData
{
    [SerializeField] private WoodBox prefab;
    [SerializeField] private float distanceFromKartBack = 5f;
    [SerializeField] private Sprite icon;

    public override Sprite GetIcon()
    {
        return icon;
    }
    public override ItemObject GiveItem(Transform parent)
    {
        WoodBox woodbox = Instantiate(prefab,parent.position - parent.forward * distanceFromKartBack, parent.rotation,parent);
        woodbox.ResetItem();
        return woodbox;
    }
}
