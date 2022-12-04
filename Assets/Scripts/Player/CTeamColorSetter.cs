using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] ColorRenderers = new Renderer[0];

    [SyncVar(hook = nameof(HandleTeamColorUpdated))]
    private Color TeamColor = new Color();

    #region Server

    public override void OnStartServer()
    {
        CRTSPlayer player = connectionToClient.identity.GetComponent<CRTSPlayer>();

        TeamColor = player.GetTeamColor();
    }

    #endregion

    #region Client

    private void HandleTeamColorUpdated(Color old_color , Color new_color)
    {
        foreach(Renderer renderer in ColorRenderers)
        {
            renderer.material.SetColor("_BaseColor",new_color);
        }
    }

    #endregion
}
