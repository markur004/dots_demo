using Unity.Entities;
using UnityEngine;

public struct MainCameraTag : IComponentData {}
public class MainCamera : IComponentData
{
    public Camera cam;
}
