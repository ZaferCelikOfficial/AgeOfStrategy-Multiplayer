using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

public class CCameraController : NetworkBehaviour
{
    [SerializeField] private Transform PlayerCameraTransform;
    [SerializeField] private float Speed = 20f;
    [SerializeField] private float ScreenBorderThickness = 10f;
    [SerializeField] private Vector2 ScreenXLimits = Vector2.zero;
    [SerializeField] private Vector2 ScreenZLimits = Vector2.zero;

    private Vector2 PreviousInput;

    private CControls Controls;

    public override void OnStartAuthority()
    {
        PlayerCameraTransform.gameObject.SetActive(true);

        Controls = new CControls();

        Controls.Player.MoveCamera.performed += SetPreviousInput;
        Controls.Player.MoveCamera.canceled += SetPreviousInput;

        Controls.Enable();
    }
    [ClientCallback]
    private void Update()
    {
        if(!isOwned || !Application.isFocused)
        {
            return;
        }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = PlayerCameraTransform.position;

        if(PreviousInput == Vector2.zero)
        {
            Vector3 cursor_movement = Vector3.zero;

            Vector2 cursor_position = Mouse.current.position.ReadValue();

            if(cursor_position.y >= Screen.height - ScreenBorderThickness)
            {
                cursor_movement.z += 1;
            }
            else if(cursor_position.y <= ScreenBorderThickness)
            {
                cursor_movement.z -= 1;
            }

            if (cursor_position.x >= Screen.width - ScreenBorderThickness)
            {
                cursor_movement.x += 1;
            }
            else if (cursor_position.x <= ScreenBorderThickness)
            {
                cursor_movement.x -= 1;
            }

            pos += cursor_movement.normalized * Speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(PreviousInput.x, 0f, PreviousInput.y) * Speed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, ScreenXLimits.x, ScreenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, ScreenZLimits.x, ScreenZLimits.y);

        PlayerCameraTransform.position = pos;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        PreviousInput = ctx.ReadValue<Vector2>();
    }
}
