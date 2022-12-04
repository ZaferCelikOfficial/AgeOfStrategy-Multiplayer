using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRTSPlayer : NetworkBehaviour
{
    [Serializable]
    public class CBuildingEntry
    {
        public EBuildings BuildingType;
        public GameObject BuildingPrefab;
    }

    [SerializeField] private CBuildingEntry[] BuildingEntry;

    private List<CUnit> MyUnits = new List<CUnit>();
    private List<CBuilding> MyBuildings= new List<CBuilding>();

    [SyncVar(hook =nameof(ClientHandleResourcesUpdated))]
    private int Resources = 500;

    public event Action<int> ClientOnResourcesUpdated;

    public int GetResources()
    {
        return Resources;
    }
    public List<CUnit> GetMyUnits()
    {
        return MyUnits;
    }

    public List<CBuilding> GetMyBuildings() 
    {
        return MyBuildings;
    }
    public GameObject GetBuildingPrefab(int building_index)
    {
        for (int i = 0; i < BuildingEntry.Length; i++)
        {
            if((int)BuildingEntry[i].BuildingType == building_index)
            {
                return BuildingEntry[i].BuildingPrefab;
            }
        }

        return null;
    }

    [Server]
    public void SetResources(int new_resources)
    {
        Resources = new_resources;
    }

    #region Server

    [Command]
    public void CmdTryPlaceBuilding(int building_id , Vector3 position)
    {
        GameObject building_instance = Instantiate(GetBuildingPrefab(building_id), position, Quaternion.identity);

        NetworkServer.Spawn(building_instance , connectionToClient);
    }

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

    private void ClientHandleResourcesUpdated(int old_resources, int new_resources)
    {
        ClientOnResourcesUpdated?.Invoke(new_resources);
    }
    #endregion
}
