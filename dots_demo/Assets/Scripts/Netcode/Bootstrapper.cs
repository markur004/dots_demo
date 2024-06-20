
using Unity.NetCode;
[UnityEngine.Scripting.Preserve]
public class Bootstrapper : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        AutoConnectPort = 0;
        CreateLocalWorld(defaultWorldName);
        return true;
    }
}

