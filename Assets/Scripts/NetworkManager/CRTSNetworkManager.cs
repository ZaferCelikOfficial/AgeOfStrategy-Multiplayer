using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CRTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject UnitSpawnerPrefab = null;
    [SerializeField] private CGameOverHandler GameOverHandlerPrefab = null;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unit_spawner_instance = Instantiate(UnitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

        NetworkServer.Spawn(unit_spawner_instance , conn);

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
 