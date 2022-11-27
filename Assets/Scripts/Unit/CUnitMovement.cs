using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class CUnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent Agent = null;
    [SerializeField] private CTargeter Targeter = null;
    [SerializeField] private float ChaseRange = 10f;

    private Camera MainCamera;

    #region Server
    public override void OnStartServer()
    {
        CGameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        CGameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        Agent.ResetPath();
    }

    [ServerCallback]
    private void Update()
    {
        CTargetable target = Targeter.GetTarget();

        if(target != null)
        {
            if( (target.transform.position - transform.position).sqrMagnitude > ChaseRange * ChaseRange )
            {
                Agent.SetDestination(target.transform.position);
            }
            else if(Agent.hasPath)
            {
                Agent.ResetPath();
            }

            return;
        }

        if (!Agent.hasPath) return;

        if(Agent.remainingDistance > Agent.stoppingDistance)  return; 

        Agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        Targeter.ClearTarget();

        if(!NavMesh.SamplePosition(position , out NavMeshHit hit , 1f , NavMesh.AllAreas)) { return; }

        Agent.SetDestination(hit.position);
    }


    #endregion



}
