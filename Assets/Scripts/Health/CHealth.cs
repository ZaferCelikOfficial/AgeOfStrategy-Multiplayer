using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.EventSystems;
public class CHealth : NetworkBehaviour
{    
    [SerializeField] private int MaxHealth = 100;

    public event Action ServerOnDie;

    public event Action<int,int> ClientOnHealthUpdated;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int CurrentHealth;

    #region Server
    public override void OnStartServer()
    {
        CurrentHealth = MaxHealth;

        CUnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }
    public override void OnStopServer()
    {
        CUnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    private void ServerHandlePlayerDie(int connection_id)
    {
        if (connectionToClient.connectionId != connection_id) return;

        DealDamage(CurrentHealth);
    }

    [Server]
    public void DealDamage(int damage_amount)
    {
        if(CurrentHealth == 0) { return; }        

        CurrentHealth = Mathf.Max(CurrentHealth - damage_amount, 0);

        if(CurrentHealth !=0 ) { return; }

        ServerOnDie?.Invoke();
        
    }


    #endregion

    #region Client

    private void HandleHealthUpdated(int old_health , int new_health)
    {
        ClientOnHealthUpdated?.Invoke(new_health, MaxHealth);
    }

    #endregion

}
