using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody ProjectileRb = null;
    [SerializeField] private int DamageToDeal = 20;
    [SerializeField] private float DestroyAfterSeconds = 5f;
    [SerializeField] private float LaunchForce = 10f;

    private void Start()
    {
        ProjectileRb.velocity = transform.forward * LaunchForce; 
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), DestroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity network_identity))
        {
            if(network_identity.connectionToClient == connectionToClient) { return; }
        }

        if(other.TryGetComponent<CHealth>(out CHealth other_health))
        {
            other_health.DealDamage(DamageToDeal);
        }

        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
