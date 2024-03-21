using UnityEngine;
using Unity.Netcode;
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour {

    private Rigidbody rb;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 300f;

    [SerializeField] private Renderer[] renderers;

    public NetworkVariable<bool> hasFlag = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public GameObject flag;  
    public NetworkVariable<Teams> teams = new NetworkVariable<Teams>(Teams.NoTeam,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private readonly ulong[] targetClientsArray = new ulong[1];
    public bool hasInitialized = false;
    public override void OnNetworkSpawn(){
        //base.OnNetworkSpawn();

        if (IsServer) {
            GameManager.instance.lastPlayerTeam.Value = GameManager.instance.lastPlayerTeam.Value + 1 > 2 ? 1 : GameManager.instance.lastPlayerTeam.Value + 1;
        }

        if (IsOwner){
            rb = GetComponent<Rigidbody>();
        }
        ChangeColor(teams.Value);     

        hasFlag.OnValueChanged += delegate { OnHasFlagChanged(); };
        teams.OnValueChanged += delegate { ChangeColor(teams.Value); };

        
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();    
    }

    public void Update(){
        if (!IsOwner) { return; }
        if (!hasInitialized) {
            hasInitialized = true;
            Teams _team = (Teams)GameManager.instance.lastPlayerTeam.Value;
            teams.Value = _team;
            GameManager.instance.AddPlayer(this);
        }
        Move();
    }

    private void ChangeColor(Teams teams)
    {
        if (teams == Teams.Red)
        {
            transform.position = new Vector3(-1.48000002f, 2.49000001f, -5.59000015f);
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = Color.red;
            }
        }
        else
        {
            transform.position = new Vector3(12.7299995f, 2.49000001f, -5.59000015f);
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = Color.blue;
            }
        }
        
    }
    private void Move() {
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        if (moveDir != Vector3.zero) {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        rb.velocity +=  moveDir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsOwner) return;
        if (!other.CompareTag("Flag")) return;
        if (other.GetComponent<FlagController>().team == teams.Value) { hasFlag.Value = true;}
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void LooseFlagServerRpc(ulong player1) {
        targetClientsArray[0] = player1;
        ClientRpcParams clientRpcParams = new ClientRpcParams{
            Send = new ClientRpcSendParams
            {
                TargetClientIds = targetClientsArray
            }
        };
        ThrowFlagClientRpc(clientRpcParams);
    }

    [ClientRpc(RequireOwnership = false)]
    private void ThrowFlagClientRpc(ClientRpcParams clientRpcParams = default) {
        if (!IsOwner) return;
        hasFlag.Value = false; 
    }

    private void OnCollisionEnter(Collision other) {
        if (!IsOwner) return;
        if (!other.collider.CompareTag("Player")) return;

        if (other.gameObject.TryGetComponent(out PlayerController player)) {
            player.LooseFlagServerRpc(player.OwnerClientId);
        }
        if (hasFlag.Value) {
            hasFlag.Value = false;
        }     
    }

    public void OnHasFlagChanged() {
        if (hasFlag.Value) {
            flag.SetActive(true);
            if (teams.Value==Teams.Red)
            {
                GameManager.instance.flagController2.gameObject.SetActive(false);
            }
            else
            {
                GameManager.instance.flagController1.gameObject.SetActive(false);
            }          
        } else {
            flag.SetActive(false);
            if (teams.Value == Teams.Red)
            {
                GameManager.instance.flagController2.gameObject.SetActive(true);
                GameManager.instance.flagController2.transform.SetPositionAndRotation(new Vector3(19.9603634f, 4.80000019f, -5.54003906f), Quaternion.identity);
            }
            else
            {
                GameManager.instance.flagController1.gameObject.SetActive(true);
                GameManager.instance.flagController1.transform.SetPositionAndRotation(new Vector3(-8.22999954f, 4.92999983f, -5.30865192f), Quaternion.identity);
            }
            
        }
    }
}