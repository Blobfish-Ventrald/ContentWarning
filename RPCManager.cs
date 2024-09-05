using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class RPCManager : MonoBehaviourPunCallbacks
{
    private static List<RPCInfo> rpcCalls = new List<RPCInfo>();

    public struct RPCInfo
    {
        public string methodName;
        public object[] parameters;
        public Player targetPlayer;

        public RPCInfo(string methodName, object[] parameters, Player targetPlayer)
        {
            this.methodName = methodName;
            this.parameters = parameters;
            this.targetPlayer = targetPlayer;
        }
    }
    public static List<RPCInfo> GetRPCs() => new List<RPCInfo>(rpcCalls);

    public static void ClearRPCs()
    {
        if (rpcCalls.Count > 0)
        {
            Debug.Log("Clearing RPCs:");

            foreach (var rpcInfo in rpcCalls)
            {
                Debug.Log("RPC Name: " + rpcInfo.methodName);
            }
            rpcCalls.Clear();
        }
        else
        {
            Debug.Log("No RPCs to clear.");
        }
    }
}
