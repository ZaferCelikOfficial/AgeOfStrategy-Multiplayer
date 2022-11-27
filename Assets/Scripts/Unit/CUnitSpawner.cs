using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
public class CUnitSpawner : NetworkBehaviour,IPointerClickHandler
{
    [SerializeField] private CHealth Health = null;
    [SerializeField] private GameObject UnitPrefab = null;
    [SerializeField] private Transform UnitSpawnPoint = null;



    #region Server
    public override void OnStartServer()
    {
        Health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        Health.ServerOnDie -= ServerHandleDie;
    }
    [Server]
    private void ServerHandleDie()
    {
        //NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unit_instance = Instantiate(UnitPrefab, UnitSpawnPoint.position, UnitSpawnPoint.rotation);
        NetworkServer.Spawn(unit_instance, connectionToClient); 
    }
    #endregion



    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        if(!isOwned) { return; }

        CmdSpawnUnit();
    }


    #endregion
}
