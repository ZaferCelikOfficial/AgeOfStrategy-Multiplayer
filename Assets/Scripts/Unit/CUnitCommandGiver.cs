using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CUnitCommandGiver : MonoBehaviour
{
    [SerializeField] private CUnitSelectionHandler UnitSelectionHandler = null;
    [SerializeField] private LayerMask UnitSelectionLayerMask = new LayerMask();

    private Camera MainCamera;
    private void Start()
    {
        MainCamera = Camera.main;

        CGameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }
    private void OnDestroy()
    {
        CGameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string obj)
    {
        enabled = false;
    }

    private void Update()
    {
        if(!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit , 300 , UnitSelectionLayerMask)) { return; }

        if(hit.collider.TryGetComponent<CTargetable>(out CTargetable target))
        {
            if(target.isOwned)
            {
                TryMove(hit.point);
                return;
            }
            TryTarget(target);
            return;
        }

        TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        foreach(CUnit unit in UnitSelectionHandler.GetSelectedUnits())
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
    private void TryTarget(CTargetable target)
    {
        foreach (CUnit unit in UnitSelectionHandler.GetSelectedUnits())
        {
            unit.TryTarget(target);
        }
    }
}
