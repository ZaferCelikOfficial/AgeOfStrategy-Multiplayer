using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUnitBase : NetworkBehaviour
{
    [SerializeField] CHealth Health = null;

    public static event Action<int> ServerOnPlayerDie;
    public static event Action<CUnitBase> ServerOnBaseSpawned;
    public static event Action<CUnitBase> ServerOnBaseDespawned;

    #region Server

    public override void OnStartServer()
    {
        Health.ServerOnDie += ServerHandleBaseDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);

        Health.ServerOnDie -= ServerHandleBaseDie;
    }

    [Server]
    private void ServerHandleBaseDie()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }

    #endregion


    #region Client



    #endregion
}
