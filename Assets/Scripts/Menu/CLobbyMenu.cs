using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject LobbyUI = null;
    [SerializeField] private Button StartGameButton;
    [SerializeField] TMP_Text[] PlayerNameTexts = new TMP_Text[4];

    private void Start()
    {
        CRTSNetworkManager.ClientOnConnected += HandleClientConnected;
        CRTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        CRTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        StartGameButton.gameObject.SetActive(state);
    }

    private void HandleClientConnected()
    {
        LobbyUI.SetActive(true);
    }

    private void OnDestroy()
    {
        CRTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        CRTSPlayer.AuthorityOnPartyOwnerStateUpdated-= AuthorityHandlePartyOwnerStateUpdated;
        CRTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    private void ClientHandleInfoUpdated()
    {
        List<CRTSPlayer> players = ((CRTSNetworkManager)NetworkManager.singleton).Players;

        for (int i = 0; i < players.Count; i++)
        {
            PlayerNameTexts[i].text = players[i].GetDisplayName().ToString(); 
        }

        for (int i = players.Count; i < PlayerNameTexts.Length; i++)
        {
            PlayerNameTexts[i].text = "Waiting For Player ...";
        }

        StartGameButton.interactable = players.Count >= 2;
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<CRTSPlayer>().CmdStartGame();
    }
    public void LeaveLobby()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
