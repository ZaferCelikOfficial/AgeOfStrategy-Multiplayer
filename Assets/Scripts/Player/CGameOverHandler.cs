using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGameOverHandler : NetworkBehaviour
{    
    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

    private List<CUnitBase> Bases = new List<CUnitBase>();


    #region Server

    public override void OnStartServer()
    {
        CUnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        CUnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        CUnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        CUnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(CUnitBase unit_base)
    {
        Bases.Add(unit_base);
    }

    [Server]
    private void ServerHandleBaseDespawned(CUnitBase unit_base)
    {
        Bases.Remove(unit_base);

        if (Bases.Count != 1) return;

        int player_id = Bases[0].connectionToClient.connectionId;

        RpcGameOver($"Player {player_id}");

        ServerOnGameOver?.Invoke();
    }

    #endregion


    #region Client
    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
