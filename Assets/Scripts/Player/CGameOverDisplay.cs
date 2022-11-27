using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class CGameOverDisplay : MonoBehaviour
{
    [SerializeField] GameObject GameOverDisplayParent = null;
    [SerializeField] private TMP_Text WinnerNameText = null;
    private void Start()
    {
        CGameOverHandler.ClientOnGameOver += ClientHandleGameOver;   
    }

    private void OnDestroy()
    {
        CGameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private void ClientHandleGameOver(string winner)
    {
        WinnerNameText.text = $"{winner} Has Won";

        GameOverDisplayParent.SetActive(true);
    }
}
