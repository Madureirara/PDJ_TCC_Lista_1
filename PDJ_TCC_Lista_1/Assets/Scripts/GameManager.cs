using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public enum Teams{
    NoTeam = 0,
    Red = 1,
    Blue = 2
}

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public FlagController flagController1;
    public FlagController flagController2;

    public NetworkVariable<int> redTeamPoints = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> blueTeamPoints = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> lastPlayerTeam = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public UIManager uIManager;


    private readonly ulong[] targetClientsArray = new ulong[1];

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();

        blueTeamPoints.OnValueChanged += delegate { OnPointsChanged(); };
        redTeamPoints.OnValueChanged += delegate { OnPointsChanged(); };
    }

    private void Awake() {
        if (instance)
            Destroy(instance.gameObject);
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void AddPlayer(NetworkBehaviour player){
        Teams team = (Teams)lastPlayerTeam.Value;
        if(team == Teams.Red)
            uIManager.SetCurrentTeamText("Red");
        else
            uIManager.SetCurrentTeamText("Blue");      
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddPointServerRpc(int team) {
        if (team == (int)Teams.Red) {
            redTeamPoints.Value++;
        }
        else {
            blueTeamPoints.Value++;
        }
    }
    public void OnPointsChanged() {
        uIManager.SetRedTeamPoints(redTeamPoints.Value);
        uIManager.SetBlueTeamPoints(blueTeamPoints.Value);
    }
    public void AddPoint(Teams team){
        AddPointServerRpc((int)team);
    }


}
