using UnityEngine;
using Fusion;
public struct NetworkInputData : INetworkInput
{
    public Vector2 InputVector;
    public NetworkBool JumpInput;
    public NetworkBool SprintInput;
}
