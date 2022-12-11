using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject LandingPanel = null;

    public void HostLobby()
    {
        LandingPanel.SetActive(false);

        NetworkManager.singleton.StartHost();

    }
}
