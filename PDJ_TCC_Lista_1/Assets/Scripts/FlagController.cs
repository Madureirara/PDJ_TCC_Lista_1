using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FlagController : NetworkBehaviour {
    Rigidbody rb;
    public Teams team;
    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();

        if(IsServer){
            rb = GetComponent<Rigidbody>();
        }
    } 
}
