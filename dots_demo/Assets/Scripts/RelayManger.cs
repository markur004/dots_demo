using Unity.Entities.UniversalDelegates;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManger : MonoBehaviour
{
    public string joincode;
    [SerializeField] private ConnectionManager _connectionManager;
    async void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
           Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
           
           joincode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
           
           _connectionManager.StartRelayHost(allocation , joincode);
           
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinRelay(string joinCode)
    {
        try
        {
            await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
