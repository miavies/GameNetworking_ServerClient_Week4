using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class NetworkPlayer : NetworkBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Jump = Animator.StringToHash("Jump");

    [Header("Player")]
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private Transform cameraPos;

    [Header("Network Properties")]
    [Networked] public Vector3 NetworkedPosition { get; set; }
    [Networked] public Color PlayerColor { get; set; }
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [Networked] public int PlayerTeam { get; set; }
    [Networked] public NetworkAnimatorData AnimatorData { get; set; }

    #region Interpolation
    private Vector3 _lastknownPosition;
    [SerializeField] private float _lerpSpeed = 3f; //smaller value => smoother
    #endregion

    #region Fusion Callbacks
    //relevant to the network, do it in spawned (initialization)

    public override void Spawned()
    {
        if (HasInputAuthority) //client
        {
            GameObject camera = GameObject.Find("Main Camera");
            camera.transform.SetParent(cameraPos.transform);
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;

            string name = NetworkSessionManager.Instance.playerName;
            Color color = NetworkSessionManager.Instance.playerColor;
            int team = NetworkSessionManager.Instance.playerTeam;

            RPC_SetPlayerData(name, color, team);
        }

        if (HasStateAuthority) //server
        {
            AnimatorData = new NetworkAnimatorData
            {
                Speed = 0,
                Jump = false
            };
        }
    }

    //On destroy
    public override void Despawned(NetworkRunner runner, bool hasState)
    {

    }

    //update function
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        if (GetInput(out NetworkInputData input))
        {
            this.transform.position +=
                new Vector3(input.InputVector.normalized.x, input.InputVector.normalized.y)
                * Runner.DeltaTime;
           
            if(input.JumpInput)
            _animator.SetTrigger(Jump);

        _animator.SetFloat(Speed, input.SprintInput ? 1f : 0f);

            NetworkedPosition = this.transform.position;

            AnimatorData = new NetworkAnimatorData()
            {
                Speed = input.SprintInput ? 1f : 0f,
                Jump = input.JumpInput,
            };

        }
    }

    //happens after fixedupdatenetwork, for nonserver objects
    public override void Render()
    {

        if (playerNameTxt != null)
            playerNameTxt.text = PlayerName.ToString();

        if (_meshRenderer != null)
        {
            _meshRenderer.material.color = PlayerColor;
        }

        if (AnimatorData.Jump)
            _animator.SetTrigger(Jump);

        _animator.SetFloat(Speed, AnimatorData.Speed);


}

    public void LateUpdate()
    {
        this.transform.position = Vector3.Lerp(_lastknownPosition, NetworkedPosition, Runner.DeltaTime * _lerpSpeed);
        _lastknownPosition = NetworkedPosition;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerData(string name, Color color, int team)
    {
        if (HasStateAuthority)
        {
            PlayerName = name;
            PlayerColor = color;
            PlayerTeam = team;

            Transform spawnPoint = AssignPlayerPosition(PlayerTeam);
            Transform pos = spawnPoint;

            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
    }

    private Transform AssignPlayerPosition(int teamNum)
    {
        
        List<Transform> list;

        if (teamNum == 1)
        {
            list = NetworkSessionManager.Instance.team1Pos;
        }
        else
        {
            list = NetworkSessionManager.Instance.team2Pos;
        }

        if (list.Count == 0)
        {
            Debug.LogError($"No spawn points left for team {teamNum}");
            return null;
        }

        Transform pos = list[0];
        list.RemoveAt(0);
        return pos;
    }

    #endregion

    

    #region Unity Callbacks

    #endregion
}
