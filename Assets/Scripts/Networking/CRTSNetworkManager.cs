using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public class CRTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject UnitBase = null;
    [SerializeField] private CGameOverHandler GameOverHandlerPrefab = null;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public List<CRTSPlayer> Players = new List<CRTSPlayer>();

    private bool IsGameInProgress = false;

    #region Server
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(!IsGameInProgress)
        {
            return;
        }

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        CRTSPlayer player = conn.identity.GetComponent<CRTSPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        CRTSPlayer player = conn.identity.GetComponent<CRTSPlayer>();

        Players.Add(player);

        player.SetDisplayName("Player " + Players.Count);

        player.SetTeamColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1));

        player.SetPartyOwner(Players.Count == 1);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        IsGameInProgress = false;
    }

    public void StartGame()
    {
        if(Players.Count < 2 )
        {
            return;
        }

        IsGameInProgress = true;

        ServerChangeScene("Scene_Map_01");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            CGameOverHandler game_over_handler_instance = Instantiate(GameOverHandlerPrefab);

            NetworkServer.Spawn(game_over_handler_instance.gameObject);

            foreach(CRTSPlayer player in Players)
            {
                GameObject base_instance = Instantiate(UnitBase, GetStartPosition().position, Quaternion.identity);

                NetworkServer.Spawn(base_instance, player.connectionToClient);
            }
        }

    }

    #endregion

    #region Client 

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        ClientOnDisconnected?.Invoke();
    }


    public override void OnStopClient()
    {
        base.OnStopClient();
    }
    #endregion
}
 