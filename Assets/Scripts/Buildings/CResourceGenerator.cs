using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CResourceGenerator : NetworkBehaviour
{
    [SerializeField] private CHealth Health = null;
    [SerializeField] private int ResourcesPerInterval = 10;
    [SerializeField] private float Interval = 2f;

    private float Timer;
    private CRTSPlayer Player;

    public override void OnStartServer()
    {
        Timer = Interval;
        Player = connectionToClient.identity.GetComponent<CRTSPlayer>();

        Health.ServerOnDie += ServerHandleDie;
        CGameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    public override void OnStopServer()
    {
        Health.ServerOnDie -= ServerHandleDie;
        CGameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }
    [ServerCallback]
    private void Update()
    {
        Timer -= Time.deltaTime;

        if(Timer <= 0)
        {
            Player.SetResources(Player.GetResources() + ResourcesPerInterval);

            Timer += Interval;
        }
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
}
