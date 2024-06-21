using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


public  class Prefabs : MonoBehaviour
{
    public GameObject player = null;
    
}

public struct PrefabsData : IComponentData
{
    public Entity player;
}



public class PrefabsBaker : Baker<Prefabs>
{
    
    public override void Bake(Prefabs authoring)
    {
        Entity playerPrefab = default;

        if (authoring.player != null)
        {
            playerPrefab = GetEntity(authoring.player, TransformUsageFlags.Dynamic);
        }
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PrefabsData
        {
            player = playerPrefab
        });
    }
}