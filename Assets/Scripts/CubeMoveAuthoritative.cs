using UnityEngine;
using System.Collections;

public class CubeMoveAuthoritative : MonoBehaviour
{

    public NetworkPlayer theOwner;
    float lastClientHInput = 0f;
    float lastClientVInput = 0f;
    float serverCurrentHInput = 0f;
    float serverCurrentVInput = 0f;

    void Awake()
    {
        if (Network.isClient)
        {
            enabled = false;
            Debug.Log("Turning off");
        }
    }

    [RPC]
    void SetPlayer(NetworkPlayer player)
    {
        theOwner = player;
        if (Network.isClient)
        {
            Debug.Log("Setting myself as owner of " + player.ToString());
        }
        if (player == Network.player)
        {
            enabled = true;
            Debug.Log("Turning on");
        }
    }

    void Update()
    {
        if (Network.player == theOwner)
        {
            float HInput = Input.GetAxis("Horizontal");
            float VInput = Input.GetAxis("Vertical");
            if (lastClientHInput != HInput || lastClientVInput != VInput)
            {
                Debug.Log("where we at");
                lastClientHInput = HInput;
                lastClientVInput = VInput;
                if (Network.isServer)
                {
                    SendMovementInput(HInput, VInput);
                }
                else if (Network.isClient)
                {
                    Debug.Log(string.Format("Sending info to server ({0},{1})", HInput, VInput));
                    networkView.RPC("SendMovementInput", RPCMode.Server, HInput, VInput);
                }
            }

        }
        //does the actual moving
        if (Network.isServer)
        {
            Vector3 moveDirection = new Vector3(serverCurrentHInput, 0, serverCurrentVInput);
            float speed = 5;
            transform.Translate(speed * moveDirection * Time.deltaTime);
        }
    }

    [RPC]
    void SendMovementInput(float HInput, float VInput)
    {
        Debug.Log(string.Format("calling this as {0}", Network.isClient ? "client" : "server"));
        serverCurrentHInput = HInput;
        serverCurrentVInput = VInput;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = transform.position;
            stream.Serialize(ref pos);
        }
        else
        {
            Vector3 posReceive = Vector3.zero;
            stream.Serialize(ref posReceive);
            transform.position = posReceive;
        }
    }
}

