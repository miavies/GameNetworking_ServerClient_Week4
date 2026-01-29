using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

namespace Network
{
    public class NetworkedGameManager : NetworkBehaviour
    {
        #region Public Variables
        [SerializeField] private NetworkPrefabRef playerPrefab;
        [SerializeField] private TextMeshProUGUI _playerCountText;
        [SerializeField] private TextMeshProUGUI _timerCountText;
        #endregion

        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();

        private NetworkSessionManager _networkSessionManager;

        private int maxPlayers = 2;
        private int timerBeforeStart = 3;

        public override void Spawned()
        {
            base.Spawned();
            NetworkSessionManager.Instance.OnPlayerJoinedEvent += OnPlayerJoined;
            NetworkSessionManager.Instance.OnPlayerLeftEvent += OnPlayerLeft;
        }

        //On destroy
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            NetworkSessionManager.Instance.OnPlayerJoinedEvent -= OnPlayerJoined;
            NetworkSessionManager.Instance.OnPlayerLeftEvent -= OnPlayerLeft;
        }

        public override void FixedUpdateNetwork()
        {
            _playerCountText.text = $"Players: {Object.Runner.ActivePlayers.Count()}/{maxPlayers}";
        }

        private void OnPlayerJoined(PlayerRef player)
        {
            if (!HasStateAuthority) return;
            if (NetworkSessionManager.Instance.JoinedPlayers.Count >= maxPlayers)
            {
                //start game count down nad then spawn
                OnGameStarted();
            }
            Debug.Log($"Player {player.PlayerId} Joined");
        }

        private void OnPlayerLeft(PlayerRef player) 
        {
            if (!HasStateAuthority) return;
            if (!_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject)) return;

            Object.Runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }

        private void OnGameStarted()
        {
            Debug.Log($"Game Started");
            foreach (var playerSpawn in NetworkSessionManager.Instance.JoinedPlayers)
            {
                var networkObject = Object.Runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, playerSpawn);
                _spawnedCharacters.Add(playerSpawn, networkObject);
            }
        }

        
    }

}

