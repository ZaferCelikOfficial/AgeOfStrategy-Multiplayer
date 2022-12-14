using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EBuildings
{
    Base = 0 ,
    BigSpawner = 1 ,
    SmallSpawner = 2 ,
    ResourceGenerator = 3,
}
public class CBuilding : NetworkBehaviour
{
    [SerializeField] private GameObject BuildingPreview = null;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int price = 100;
    //[SerializeField] private int id = -1;
    [SerializeField] EBuildings BuildingType;


    public static event Action<CBuilding> ServerOnBuildingSpawned;
    public static event Action<CBuilding> ServerOnBuildingDespawned;

    public static event Action<CBuilding> AuthorityOnUnitSpawned;
    public static event Action<CBuilding> AuthorityOnUnitDespawned;

    public GameObject GetBuildingPreview()
    {
        return BuildingPreview;
    }
    public Sprite GetIcon()
    {
        return icon;
    }

    //public int GetId()
    //{
    //    return id;
    //}

    public int GetPrice()
    { 
        return price; 
    }

    public EBuildings GetBuildingType()
    {
        return BuildingType;
    }


    #region Server


    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
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
    #endregion
}
