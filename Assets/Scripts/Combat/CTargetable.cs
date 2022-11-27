using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTargetable : NetworkBehaviour
{
    [SerializeField] private Transform AimAtPoint = null;

    public Transform GetAimAtPoint()
    {
        return AimAtPoint;
    }

}
