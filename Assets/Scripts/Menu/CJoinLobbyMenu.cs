using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CJoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject LandingPagePanel = null;
    [SerializeField] private TMP_InputField AdressInput = null;
    [SerializeField] private Button JoinButton = null;


    private void OnEnable()
    {
        CRTSNetworkManager.ClientOnConnected += HandleClientConnected;
        CRTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        CRTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        CRTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected; 
    }

    public void Join()
    {
        string adress = AdressInput.text;

        NetworkManager.singleton.networkAddress = adress;
        NetworkManager.singleton.StartClient();

        JoinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        JoinButton.interactable = true;

        gameObject.SetActive(false);
        LandingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        JoinButton.interactable = false;
    }
}
