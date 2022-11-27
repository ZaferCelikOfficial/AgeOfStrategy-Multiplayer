using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRTSPlayer : NetworkBehaviour
{
    private List<CUnit> MyUnits = new List<CUnit>();
    private List<CBuilding> MyBuildings= new List<CBuilding>();

    public List<CUnit> GetMyUnits()
    {
        return MyUnits;
    }

    public List<CBuilding> GetMyBuildings()
    {
        return MyBuildings;
    }

    #region Server

    public override void OnStartServer()
    {
        CUnit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        CUnit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

        CBuilding.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        CBuilding.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }



    public override void OnStopServer()
    {
        CUnit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        CUnit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;

        CBuilding.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        CBuilding.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Server]
    private void ServerHandleBuildingDespawned(CBuilding building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        MyBuildings.Add(building);
    }
    [Server]
    private void ServerHandleBuildingSpawned(CBuilding building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        MyBuildings.Remove(building);
    }

    private void ServerHandleUnitSpawned(CUnit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        MyUnits.Add(unit);
    }
    private void ServerHandleUnitDespawned(CUnit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        MyUnits.Remove(unit);
    }
    #endregion


    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) return;

        CUnit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        CUnit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        CBuilding.AuthorityOnUnitSpawned += AuthorityHandleBuildingSpawned;
        CBuilding.AuthorityOnUnitDespawned += AuthorityHandleBuildingDespawned;
    }



    public override void OnStopClient()
    {
        if (!isClientOnly || !isOwned) return;

        CUnit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        CUnit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

        CBuilding.AuthorityOnUnitSpawned += AuthorityHandleBuildingSpawned;
        CBuilding.AuthorityOnUnitDespawned += AuthorityHandleBuildingDespawned;
    }

    private void AuthorityHandleUnitSpawned(CUnit unit)
    {        
        MyUnits.Add(unit);
    }
    private void AuthorityHandleUnitDespawned(CUnit unit)
    {        
        MyUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingDespawned(CBuilding building)
    {
        MyBuildings.Add(building);
    }

    private void AuthorityHandleBuildingSpawned(CBuilding building)
    {
        MyBuildings.Remove(building);
    }
    #endregion
}
