using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    #region Public Variables
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private TMP_InputField nameTxt;
    [SerializeField] private TMP_Dropdown colorDropdown;
    [SerializeField] private Button playBtn;
    #endregion

    #region Private Variables
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = null;
    private NetworkRunner _networkRunner;

    public Color playerColor;
    public string playerName;
    #endregion

    async void StartGame(GameMode game)
    {
        _networkRunner = this.gameObject.AddComponent<NetworkRunner>();
        _networkRunner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        
        await _networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = game,
            SessionName = "TestSession",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });//Task<StartGameResult>
    }

    #region Unity Callbacks
    private void Awake()
    {
        _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
       
    }

    public void PlayButton()
    {
        SetColor();
        SetName();

        #if SERVER
        StartGame(GameMode.Host);
        #elif CLIENT
        StartGame(GameMode.Client);
        #endif
    }
    #endregion

    #region Used Fusion Callbacks
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        data.InputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Set(data);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            var position = Vector3.zero;
             var networkObject = runner.Spawn(playerPrefab, position, Quaternion.identity, player);
            _spawnedCharacters.Add(player, networkObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!_spawnedCharacters.TryGetValue(player, out var playerObject)) return;

        runner.Despawn(playerObject);
        _spawnedCharacters.Remove(player);
    }
    #endregion

    private void SetColor()
    {
        switch (colorDropdown.value)
        {
            case 0: 
                playerColor = Color.white;
                break;
            case 1: 
                playerColor = Color.black;
                break;
            case 2: 
                playerColor = Color.red;
                break;
            case 3: 
                playerColor = Color.orange;
                break;
            case 4: 
                playerColor = Color.yellow;
                break;
            case 5: 
                playerColor = Color.green;
                break;
            case 6: 
                playerColor = Color.blue;
                break;
            case 7: 
                playerColor = Color.purple;
                break;
            default: 
                playerColor = Color.cyan;
                break;
        }
    }

    private void SetName()
    {
         playerName = nameTxt.text;
    }

    #region
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }
    #endregion
}