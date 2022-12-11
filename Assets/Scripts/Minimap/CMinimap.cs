using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CMinimap : MonoBehaviour,IPointerDownHandler,IDragHandler
{
    [SerializeField] private RectTransform MinimapRect = null;
    [SerializeField] private float MapScale = 20f;
    [SerializeField] private float Offset = -6f;
    private Transform PlayerCameraTransform;

    private void Update()
    {
        if(PlayerCameraTransform != null)
        {
            return;
        }

        if(NetworkClient.connection?.identity == null)
        {
            return;
        }

        PlayerCameraTransform = NetworkClient.connection.identity.GetComponent<CRTSPlayer>().GetCameraTransform();
        
    }

    private void MoveCamera()
    {
        Vector2 mouse_pos = Mouse.current.position.ReadValue();

        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(MinimapRect , mouse_pos , null , out Vector2 local_point))
        {
            return;
        }

        Vector2 lerp = new Vector2((local_point.x - MinimapRect.rect.x) / MinimapRect.rect.width, (local_point.y - MinimapRect.rect.y) / MinimapRect.rect.height);

        Vector3 new_camera_pos = new Vector3(Mathf.Lerp(-MapScale, MapScale, lerp.x), PlayerCameraTransform.position.y, Mathf.Lerp(-MapScale, MapScale, lerp.y));

        PlayerCameraTransform.position = new_camera_pos + new Vector3(0, 0, Offset);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
}
