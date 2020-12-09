using Items;
using UnityEngine;

[CreateAssetMenu(fileName="WoodBox",menuName="ScriptableObject/Items/Woodbox",order=0)]
public class ItemWoodbox : Item
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float distanceFromKartBack = 5f;
    private Vector3 _spawnPosition;
    public override void Use()
    {
        Instantiate(prefab, _spawnPosition, Quaternion.identity);
    }

    public override void OnKeyDown(PlayerRaceInfo info)
    {
        var transform = info.kart.transform;
        _spawnPosition = transform.position - transform.forward*distanceFromKartBack;  
        Use();
    }
}
