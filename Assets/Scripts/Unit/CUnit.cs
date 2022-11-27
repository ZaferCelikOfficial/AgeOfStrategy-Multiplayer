using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class CUnit : NetworkBehaviour
{
    [SerializeField] private CHealth Health = null;
    [SerializeField] private CUnitMovement UnitMovement;
    [SerializeField] private CTargeter Targeter;
    [SerializeField] private UnityEvent OnSelectedEvent;
    [SerializeField] private UnityEvent OnDeselectedEvent;

    public static event Action<CUnit> ServerOnUnitSpawned;
    public static event Action<CUnit> ServerOnUnitDespawned;

    public static event Action<CUnit> AuthorityOnUnitSpawned;
    public static event Action<CUnit> AuthorityOnUnitDespawned;

    #region Server
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        Health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        Health.ServerOnDie -= ServerHandleDie;
        ServerOnUnitDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {        
        AuthorityOnUnitSpawned?.Invoke(this);
    }
    public override void OnStopClient()
    {
        if (!isOwned) return;

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if(!isOwned) { return; }

        OnSelectedEvent?.Invoke();

    }

    [Client]
    public void Deselect()
    {
        if (!isOwned) { return; }

        OnDeselectedEvent?.Invoke();
    }

    #endregion

    public CUnitMovement GetUnitMovement()
    {
        return UnitMovement;
    }
    public void TryTarget(CTargetable target)
    {
        Targeter.CmdSetTarget(target.gameObject);
    }
}
