using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CRTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject UnitBase = null;
    [SerializeField] private CGameOverHandler GameOverHandlerPrefab = null;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        CRTSPlayer player = conn.identity.GetComponent<CRTSPlayer>();

        player.SetTeamColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f),1));        

        GameObject unit_base = Instantiate(UnitBase, conn.identity.transform.position, conn.identity.transform.rotation);

        NetworkServer.Spawn(unit_base, conn);

    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            CGameOverHandler game_over_handler_instance = Instantiate(GameOverHandlerPrefab);

            NetworkServer.Spawn(game_over_handler_instance.gameObject);
        }

    }
}
 