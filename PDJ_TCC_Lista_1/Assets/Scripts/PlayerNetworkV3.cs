using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkV3 : NetworkBehaviour
{
    private NetworkVariable<int> numeroAleatorio = new NetworkVariable<int>(1,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    Rigidbody m_Rigidbody;
    public float m_Speed = 5f;

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
    }



    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        numeroAleatorio.OnValueChanged += (int valorAnterior, int novoValor) =>
        {
            Debug.Log(OwnerClientId + "\tNumero Aleatorio: " + novoValor);
            //GameManagerV3.instance.textField.text = novoValor.ToString();
        };
    }



    private void FixedUpdate()
    {
        if (IsOwner == true)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                numeroAleatorio.Value = Random.Range(1, 50);
            }

            Vector3 m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            m_Rigidbody.MovePosition(transform.position + m_Input * Time.deltaTime * m_Speed);
        }
    }
}

