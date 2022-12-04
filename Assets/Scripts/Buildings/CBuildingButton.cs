using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CBuildingButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private CBuilding Building = null;
    [SerializeField] private Image IconImage = null;
    [SerializeField] private TMP_Text PriceText = null;
    [SerializeField] private LayerMask FloorMask = new LayerMask();

    private Camera MainCamera;
    private CRTSPlayer Player;
    private GameObject BuildingPreviewInstance;
    private Renderer BuildingRendererInstance;



    private void Start()
    {
        MainCamera = Camera.main;

        IconImage.sprite = Building.GetIcon();
        PriceText.text = Building.GetPrice().ToString();
    }

    private void Update()
    {
        if(Player == null)
        {
            Player = NetworkClient.connection.identity.GetComponent<CRTSPlayer>();
        }

        if (BuildingPreviewInstance == null) return;

        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        BuildingPreviewInstance = Instantiate(Building.GetBuildingPreview());

        BuildingRendererInstance = BuildingPreviewInstance.GetComponentInChildren<Renderer>();

        BuildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (BuildingPreviewInstance == null) return;

        Ray ray = MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(ray , out RaycastHit hit , Mathf.Infinity , FloorMask))
        {
            Player.CmdTryPlaceBuilding( (int)Building.GetBuildingType() , hit.point);
        }

        Destroy(BuildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, FloorMask)) return;

        BuildingPreviewInstance.transform.position = hit.point;

        if(!BuildingPreviewInstance.activeSelf)
        {
            BuildingPreviewInstance.SetActive(true);
        }
    }

}
