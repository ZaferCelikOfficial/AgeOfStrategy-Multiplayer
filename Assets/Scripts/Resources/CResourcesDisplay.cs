using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text ResourcesText = null;

    private CRTSPlayer Player;

    private void Update()
    {
        if(Player == null)
        {
            Player = NetworkClient.connection.identity.GetComponent<CRTSPlayer>();

            if(Player != null)
            {
                ClientHandleResourcesUpdated(Player.GetResources());

                Player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
            }
        }
    }

    private void OnDestroy()
    {
        Player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        ResourcesText.text = $"Resources: {resources}";
    }
}
