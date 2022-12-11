using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject LandingPanel = null;

    [SerializeField] private bool UseSteam = false;

    [SerializeField] TMPro.TMP_Text ErrorMessage;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;

    private void Start()
    {
        if(!UseSteam)
        {
            return;
        }

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        try
        {
            LandingPanel.SetActive(false);

            if (UseSteam)
            {
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
                return;
            }

            NetworkManager.singleton.StartHost();
        }
        catch(Exception ex)
        {
            ErrorMessage.text = ex.ToString();
        }
    }

    private void OnLobbyCreated (LobbyCreated_t callback)
    {
        try
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                LandingPanel.SetActive(true);
                return;
            }

            NetworkManager.singleton.StartHost();

            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                "HostAdress",
                SteamUser.GetSteamID().ToString());
        }
        catch (Exception ex)
        {
            ErrorMessage.text = ex.ToString();
        }
    }
    private void OnGameLobbyJoinRequested (GameLobbyJoinRequested_t callback)
    {
        try
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }
        catch (Exception ex)
        {
            ErrorMessage.text = ex.ToString();
        }
        
    }
    private void OnLobbyEntered (LobbyEnter_t callback)
    {
        try
        {
            if (NetworkServer.active)
            {
                return;
            }

            string host_adress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                "HostAdress");

            NetworkManager.singleton.networkAddress = host_adress;
            NetworkManager.singleton.StartClient();

            LandingPanel.SetActive(false);
        }
        catch (Exception ex)
        {
            ErrorMessage.text = ex.ToString();
        }

    }

}
