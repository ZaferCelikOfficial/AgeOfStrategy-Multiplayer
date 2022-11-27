using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUnitFiring : NetworkBehaviour
{
    [SerializeField] private CTargeter Targeter = null;
    [SerializeField] private GameObject ProjectilePrefab = null;
    [SerializeField] private Transform ProjectileSpawnPoint = null;

    [SerializeField] private float FireRange = 5;
    [SerializeField] private float FireRate = 1;
    [SerializeField] private float RotationSpeed = 20f;

    private float LastFireTime;

    [ServerCallback]
    private void Update()
    {
        CTargetable target = Targeter.GetTarget();

        if (target == null) return;

        if (!CanFireAtTarget(target)) return;

        Quaternion target_rotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, target_rotation, RotationSpeed * Time.deltaTime);

        if(Time.time > (1 / FireRate) + LastFireTime)
        {
            Quaternion projectile_rotation = Quaternion.LookRotation(target.GetAimAtPoint().position - ProjectileSpawnPoint.position);

            GameObject projectile_instance = Instantiate(ProjectilePrefab, ProjectileSpawnPoint.position, projectile_rotation);

            NetworkServer.Spawn(projectile_instance, connectionToClient);

            LastFireTime = Time.time;
        }
    }

    [Server]
    private bool CanFireAtTarget(CTargetable target)
    {
        return ((target.transform.position - transform.position).sqrMagnitude <= FireRange * FireRange);
    }
}
