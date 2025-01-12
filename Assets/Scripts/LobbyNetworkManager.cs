using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyNetworkManager : MonoBehaviour,INetworkRunnerCallbacks
{

    #region VARIABLES
    private NetworkRunner _networkRunner;
    public FusionBootstrap _fusionBootstrap;
    public static LobbyNetworkManager instance;
    [SerializeField] private TMP_InputField roomName;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    void Start()
    {
    }
    void Update()
    {
    }
    #endregion

    #region FUNCTIONS
    public void CreateRoom()
    {
        _fusionBootstrap.DefaultRoomName = roomName.text;
        _fusionBootstrap.StartHost();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Connected " + runner.IsConnectedToServer);
    }
    #endregion

    #region CALLBACKS
    #endregion
}
