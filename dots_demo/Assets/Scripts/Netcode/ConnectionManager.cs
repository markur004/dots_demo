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
        DontDestroyOnLoad(this.gameObject);
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

    public void StartRelayHost(int port, string conIP)
    {
        Debug.Log("server" + port + " " + conIP);
        DestoryLocalSimulationWorld();
        SceneManager.LoadScene(1);
        
        var serverWorld = ClientServerBootstrap.CreateServerWorld("server");
        ushort Port = (ushort )port;
        // var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(Port);
        // {
        //     using var query =
        //         serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
        //     query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
        // }

        var listenReq = ServerWorld.EntityManager.CreateEntity(typeof(NetworkStreamRequestListen));
        ServerWorld.EntityManager.SetComponentData(listenReq,new NetworkStreamRequestListen {Endpoint = ClientServerBootstrap.DefaultListenAddress.WithPort(Port)});
        StartRelayClient(port, conIP);
    }

    private void StartRelayClient(int port, string conIP)
    {
        Debug.Log("server" + port + " " + conIP);
        DestoryLocalSimulationWorld();
        SceneManager.LoadScene(1);
        Debug.Log("client " + port + " " + conIP);
        var clientWorld = ClientServerBootstrap.CreateClientWorld("client");
        ushort  Port = (ushort )port;
        var connectionEndpoint = NetworkEndpoint.Parse(conIP, Port);
        {
            using var query =
                clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager,connectionEndpoint);
        }
        World.DefaultGameObjectInjectionWorld = clientWorld;
    }


}