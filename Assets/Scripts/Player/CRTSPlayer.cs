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

    [SerializeField] private Transform CameraTransform;
    [SerializeField] private LayerMask BuildingBlockLayer= new LayerMask();
    [SerializeField] private CBuildingEntry[] BuildingEntry;
    [SerializeField] private float BuildingRangeLimit = 5f;


    private Color TeamColor = new Color();
    private List<CUnit> MyUnits = new List<CUnit>();
    private List<CBuilding> MyBuildings= new List<CBuilding>();


    [SyncVar(hook =nameof(ClientHandleResourcesUpdated))]
    private int Resources = 500;

    public event Action<int> ClientOnResourcesUpdated;
    public Color GetTeamColor()
    {
        return TeamColor;
    }

    public int GetResources()
    {
        return Resources;
    }
    public List<CUnit> GetMyUnits()
    {
        return MyUnits;
    }
    public Transform GetCameraTransform()
    {
        return CameraTransform;
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


    public bool CanPlaceBuilding(BoxCollider building_collider , Vector3 point)
    {
        if (Physics.CheckBox(point + building_collider.center, building_collider.size / 2, Quaternion.identity, BuildingBlockLayer))
        {
            return false;
        }

        foreach (CBuilding building in MyBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= BuildingRangeLimit * BuildingRangeLimit)
            {                
                return true;
            }
        }

        return false;
    }
    #region Server

    [Command]
    public void CmdTryPlaceBuilding(int building_id , Vector3 position)
    {
        CBuilding building_to_place = GetBuildingPrefab(building_id).GetComponent<CBuilding>();

        if(building_to_place == null)
        {
            return;
        }

        if(Resources < building_to_place.GetPrice())
        {
            return;
        }

        BoxCollider building_collider = building_to_place.GetComponent<BoxCollider>();

        if(!CanPlaceBuilding(building_collider , position))
        {
            return;
        }
        
        GameObject building_instance = Instantiate(building_to_place.gameObject, position, Quaternion.identity);

        NetworkServer.Spawn(building_instance , connectionToClient);

        SetResources(Resources - building_to_place.GetPrice());
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
    public void SetTeamColor(Color new_team_color)
    {
        TeamColor = new_team_color;
    }

    [Server]
    public void SetResources(int new_resources)
    {
        Resources = new_resources;
    }

    [Server]
    private void ServerHandleBuildingDespawned(CBuilding building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        MyBuildings.Remove(building);
    }
    [Server]
    private void ServerHandleBuildingSpawned(CBuilding building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        MyBuildings.Add(building);
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
