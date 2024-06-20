using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    private string _connectIP => addres.text;
    private ushort _port => ushort.Parse(port.text);

    [SerializeField] private Button conbutton;
    [SerializeField] private TMP_Dropdown contype;
    [SerializeField] private TMP_InputField port;
    [SerializeField] private TMP_InputField addres;

    private void Awake()
    {
        Application.runInBackground = true;
    }

    private void Start()
    {
        port.text = "7979";
        addres.text = "127.0.0.1";
    }


    public void OnButtonConnectClick()
    {
        //Debug.Log("buttonClicked");
        DestoryLocalSimulationWorld();
        SceneManager.LoadScene(1);

        switch (contype.value)
        {
            case 0:
                StartClientServer();
                break;
            case 1:
                StartClient();
                break;
            case 2:
                StartServer();
                break;
            default:
                Debug.Log("Error invalid connection type");
                break;
        }
    }

    private static void DestoryLocalSimulationWorld()
    {
        foreach (var world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }
    }

    private void StartServer()
    {
        var serverWorld = ClientServerBootstrap.CreateServerWorld("server");

        var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(_port);
        {
            using var query =
                serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                    query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
        }
        
    }

    private void StartClient()
    {
        var clientWorld = ClientServerBootstrap.CreateClientWorld("client");
        var connectionEndpoint = NetworkEndpoint.Parse(_connectIP, _port);
        {
            using var query =
                clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager,connectionEndpoint);
        }
        World.DefaultGameObjectInjectionWorld = clientWorld;
    }

    private void StartClientServer()
    {
        StartServer();
        StartClient(); 
    }
    
    public static World ServerWorld
    {
        get
        {
            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.GameServer)
                {
                    return world;
                }
            }
    
            return null;
        }
    
    }
    
    public static World ClientWorld
    {
        get
        {
            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.GameClient)
                {
                    return world;
                }
            }
    
            return null;
        }
    
    }
    //
    // private void Start()
    // {
    //
    // }
    //
    // public void Connect()
    // {
    //     foreach (var world in World.All)
    //     {
    //         if (world.Flags == WorldFlags.Game)
    //         {
    //             world.Dispose();
    //             break;
    //         }
    //     }
    //     if (_connecting)
    //     {
    //         return;
    //     }
    //
    //     _connecting = true;
    //     StartCoroutine(IntializeConnection());
    // }
    //
    // private IEnumerator IntializeConnection()
    // {
    //     bool isServer = ClientServerBootstrap.RequestedPlayType == ClientServerBootstrap.PlayType.ClientAndServer ||
    //                     ClientServerBootstrap.RequestedPlayType == ClientServerBootstrap.PlayType.Server;
    //     bool isClient = ClientServerBootstrap.RequestedPlayType == ClientServerBootstrap.PlayType.ClientAndServer ||
    //                     ClientServerBootstrap.RequestedPlayType == ClientServerBootstrap.PlayType.Client;
    //     while ((isServer && !ClientServerBootstrap.HasServerWorld) || (isClient && !ClientServerBootstrap.HasClientWorlds))
    //     {
    //         yield return null;
    //     }
    //
    //     if (isServer)
    //     {
    //         using var query =
    //             ServerWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
    //         query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(NetworkEndpoint.Parse(_listenIP, _port));
    //
    //     }
    //     
    //     if (isClient)
    //     {
    //         using var query =
    //             ClientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
    //         query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(ClientWorld.EntityManager,NetworkEndpoint.Parse(_connectIP, _port));
    //
    //     }
    //
    //     _connecting = false;
    // }
    //


}