using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyNetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{

    #region VARIABLES
    [SerializeField] private LobbyUI lobbyUI;
    private NetworkRunner _runner;
    public static LobbyNetworkManager instance;
    [SerializeField] private TMP_InputField roomName;
    private string nameOfRoom;
    public PlayerRef currentPlayer;

    [SerializeField, Space(20)] private Transform playerOnePosition;
    [SerializeField] private Transform playerTwoPosition;

    [SerializeField] private GameObject mainPrefab;

    [Networked] public bool isGameStarted { get; set; }
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
        roomName.text = "NewRoom" + UnityEngine.Random.Range(0, 100);
    }
    void Update()
    {
    }
    #endregion

    #region LOBBY
   /* private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 200, 200, 40), "Host"))
            {
                StartGame(GameMode.AutoHostOrClient);
            }
            if (GUI.Button(new Rect(0, 250, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
        else
        {
            if (_runner.IsServer && !isGameStarted && GUI.Button(new Rect(0, 200, 200, 40), "Start Game"))
            {
                StartGame();
            }
        }
    }*/

    public void JoinRandomGame()
    {
        nameOfRoom = "NewRoom" + UnityEngine.Random.Range(0, 100);

        lobbyUI.ShowMessagePanel("Joining Random Room.");

        StartGame(GameMode.AutoHostOrClient);
    }
    public void CreateRoom()
    {
        nameOfRoom = roomName.text;
        if (nameOfRoom == "")
            nameOfRoom = "NewRoom" + UnityEngine.Random.Range(0, 100);

        lobbyUI.ShowMessagePanel("Creating Custom Room.");

        StartGame(GameMode.Host);
    }
    public void JoinRoom()
    {
        nameOfRoom = roomName.text;
        if (nameOfRoom == "")
        {
            JoinRandomGame();
            return;
        }

        lobbyUI.ShowMessagePanel("Joining Custom Room.");

        StartGame(GameMode.Client);
    }
    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = nameOfRoom,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2
        });

        if (result.Ok)
        {
            Debug.Log("Connected");
            if (lobbyUI)
                lobbyUI.ShowMessagePanel("Joined Room! \n Waiting for Second player.\n Room Name : " + _runner.SessionInfo.Name);
        }
        else
        {
            Debug.Log(result.ShutdownReason);
            if (lobbyUI)
            {
                lobbyUI.ShowMessagePanel("Error! \n " + result.ErrorMessage);
                lobbyUI.ShowBackButton();
            }

        }
    }


    #endregion

    #region CALLBACKS
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // throw new NotImplementedException();
        if (runner.SessionInfo.PlayerCount == 2)
            StartGame();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // throw new NotImplementedException();
        if (!Gamemanager.instance.isGameOver)
            Gamemanager.instance.PlayerExit();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        // throw new NotImplementedException();
        if (!Gamemanager.instance.isGameOver)
            Gamemanager.instance.PlayerExit();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        //throw new NotImplementedException();
        //Gamemanager.instance.PlayerExit();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        //throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        // throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        // Debug.Log("Connected");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        //throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        // throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        // throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //  throw new NotImplementedException();
    }
    #endregion

    #region IN GAME
    [SimpleButton]
    public void StartGame()
    {
        if (_runner.IsServer)
        {
            isGameStarted = true;

            int i = 0;
            foreach (var item in _runner.ActivePlayers)
            {
                Transform p = i == 0 ? playerOnePosition : playerTwoPosition;
                GameObject gm = _runner.Spawn(mainPrefab, p.position, Quaternion.Euler(0, 0, 0), item).gameObject;

                if (gm.TryGetComponent<PlayerTower>(out PlayerTower playerTower))
                {
                    if (i == 0)
                        playerTower.isLeft = true;

                    if (p.parent.gameObject.TryGetComponent<HexagonTile>(out HexagonTile tile))
                    {
                        tile.PlaceItem(gm);
                    }

                    playerTower.life = 7;

                    gm.transform.parent = HexagonManager.instance.transform;
                }
                i++;
            }
            Gamemanager.instance.StartGame();

            Invoke("ChangeRotation", 1f);

        }

        LobbyUI.instance.GameStart();
    }
    void ChangeRotation()
    {
        HexagonManager.instance.SetRotationNow();
    }
    #endregion
}
