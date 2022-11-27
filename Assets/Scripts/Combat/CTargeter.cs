using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CTargeter : NetworkBehaviour
{
    private CTargetable Target;

    public CTargetable GetTarget()
    {
        return Target;
    }



    #region Server

    public override void OnStartServer()
    {
        CGameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        CGameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Command]
    public void CmdSetTarget(GameObject target_gameobject)
    {
        if(!target_gameobject.TryGetComponent<CTargetable>(out CTargetable target)) { return; }

        this.Target = target;
    }

    [Server]
    public void ClearTarget()
    {
        Target = null;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }
    #endregion

}
