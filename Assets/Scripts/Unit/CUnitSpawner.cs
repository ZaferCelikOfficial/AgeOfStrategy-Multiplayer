using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

public class CUnitSpawner : NetworkBehaviour,IPointerClickHandler
{
    [SerializeField] private CHealth Health = null;
    [SerializeField] private CUnit Unit = null;
    [SerializeField] private Transform UnitSpawnPoint = null;
    [SerializeField] private TMP_Text RemainingUnitsText = null;
    [SerializeField] private Image UnitProgressImage = null;
    [SerializeField] private int MaxUnitQueue = 5;
    [SerializeField] private float SpawnMoveRange = 7f;
    [SerializeField] private float UnitSpawnDuration = 5f;

    [SyncVar(hook =nameof(ClientHandleQueuedUnitsUpdated))]
    private int QueuedUnits;

    [SyncVar]
    private float UnitTimer;

    private float ProgressImageVelocity;

    private void Update()
    {
        if(isServer)
        {
            ProduceUnits();
        }

        if(isClient)
        {
            UpdateTimerDisplay();
        }
    }

    #region Server
    [Server]
    private void ProduceUnits()
    {
        if(QueuedUnits == 0)
        {
            return;
        }

        UnitTimer += Time.deltaTime;

        if(UnitTimer < UnitSpawnDuration)
        {
            return;
        }

        GameObject unit_instance = Instantiate(Unit.gameObject, UnitSpawnPoint.position, UnitSpawnPoint.rotation);
        NetworkServer.Spawn(unit_instance, connectionToClient);

        Vector3 spawn_offset = UnityEngine.Random.insideUnitSphere * SpawnMoveRange;

        spawn_offset.y = UnitSpawnPoint.position.y;

        CUnitMovement unit_movement = unit_instance.GetComponent<CUnitMovement>();
        unit_movement.ServerMove(UnitSpawnPoint.position + spawn_offset);

        QueuedUnits--;
        UnitTimer = 0f;
    }
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
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if(QueuedUnits == MaxUnitQueue)
        {
            return;
        }

        CRTSPlayer player = connectionToClient.identity.GetComponent<CRTSPlayer>();
        if(player.GetResources() < Unit.GetResourceCost())
        {
            return;
        }

        QueuedUnits++;

        player.SetResources(player.GetResources() - Unit.GetResourceCost());
    }
    #endregion



    #region Client

    private void UpdateTimerDisplay()
    {
        float new_progress = UnitTimer / UnitSpawnDuration;

        if(new_progress < UnitProgressImage.fillAmount)
        {
            UnitProgressImage.fillAmount = new_progress;
        }
        else
        {
            UnitProgressImage.fillAmount = Mathf.SmoothDamp(UnitProgressImage.fillAmount, new_progress, ref ProgressImageVelocity, 0.1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        if(!isOwned) { return; }

        CmdSpawnUnit();
    }    
    private void ClientHandleQueuedUnitsUpdated(int old_queued_units , int new_queued_units)
    {
        RemainingUnitsText.text = new_queued_units.ToString();
    }
    #endregion
}
