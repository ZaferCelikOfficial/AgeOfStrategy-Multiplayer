using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CUnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform UnitSelectionArea = null;
    [SerializeField] private LayerMask UnitSelectionLayerMask = new LayerMask();

    private Vector2 StartPosition;

    private CRTSPlayer Player;
    private Camera MainCamera;

    private List<CUnit> SelectedUnits = new List<CUnit>();

    private event Action TryGetRTSPlayer;

    private void Start()
    {
        MainCamera = Camera.main;

        CUnit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        CGameOverHandler.ClientOnGameOver += ClientHandleGameOver;

        Player = NetworkClient.connection.identity.GetComponent<CRTSPlayer>();
    }

    private void ClientHandleGameOver(string obj)
    {
        enabled = false;
    }

    private void OnDestroy()
    {
        CUnit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

        CGameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
    private void Update()
    {

        if( Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }

    }

    private void ClearSelectionArea()
    {
        UnitSelectionArea.gameObject.SetActive(false);

        if(UnitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, 300, UnitSelectionLayerMask)) { return; }

            if (!hit.collider.TryGetComponent<CUnit>(out CUnit hit_unit)) { return; }

            if (!hit_unit.isOwned) return;

            SelectedUnits.Add(hit_unit);

            foreach (CUnit selected_unit in SelectedUnits)
            {
                selected_unit.Select();
            }
        }

        Vector2 min = UnitSelectionArea.anchoredPosition - UnitSelectionArea.sizeDelta / 2;
        Vector2 max = UnitSelectionArea.anchoredPosition + UnitSelectionArea.sizeDelta / 2;

        foreach(CUnit unit in Player.GetMyUnits())
        {
            if (SelectedUnits.Contains(unit)) continue;

            Vector3 screen_position = MainCamera.WorldToScreenPoint(unit.transform.position);

            if(screen_position.x > min.x  &&  screen_position.y > min.y && screen_position.x < max.x && screen_position.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    public List<CUnit> GetSelectedUnits()
    {
        return SelectedUnits;
    }

    private void StartSelectionArea()
    {
        if(!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (CUnit unit in SelectedUnits)
            {
                unit.Deselect();
            }
        }

        SelectedUnits.Clear();

        UnitSelectionArea.gameObject.SetActive(true);

        StartPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }
    private void UpdateSelectionArea()
    {
        Vector2 mouse_position = Mouse.current.position.ReadValue();

        float area_width = mouse_position.x - StartPosition.x;
        float area_height = mouse_position.y - StartPosition.y;

        UnitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(area_width) , Mathf.Abs(area_height));
        UnitSelectionArea.anchoredPosition = StartPosition + new Vector2(area_width / 2 , area_height / 2);
    }


    private void AuthorityHandleUnitDespawned(CUnit obj)
    {
        SelectedUnits.Remove(obj);
    }
}
