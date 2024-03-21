using System.Collections;
using UnityEngine;

public class Base : MonoBehaviour
{
    public Teams baseType;
    bool has_completed = false;

    private void OnTriggerEnter(Collider other) {
        if (has_completed) {
            Debug.Log("has_completed");
            return;
        }
        if (!other.CompareTag("Player")) {
            Debug.Log("CompareTag");
            return; 
        }

        if (!other.attachedRigidbody.TryGetComponent(out PlayerController player))
        {
            Debug.Log("AtchedRigid");
            return;
        }
        if (!player.hasFlag.Value)
        {
            Debug.Log("HasFlag.value");
            return;
        }
        if (player.teams.Value != baseType)
        {
            Debug.Log("Se o time e diferente da bandeira");
            return;
        }

        player.LooseFlagServerRpc(player.OwnerClientId);
        GameManager.instance.AddPoint(baseType);
        has_completed = true;
        StartCoroutine(ResetHasCompleted(player));
    }

    IEnumerator ResetHasCompleted(PlayerController playerController){
        yield return new WaitUntil(() => !playerController.hasFlag.Value);
        has_completed = false;
    }
}
