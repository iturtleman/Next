using UnityEngine;
using System.Collections;

public class CubeMoveAuthoritative : MonoBehaviour
{

    public NetworkPlayer theOwner;


    Vector3 lastPosition;
    public float speed = 5;

    float lastClientHInput = 0f;
    float lastClientVInput = 0f;
    float serverCurrentHInput = 0f;
    float serverCurrentVInput = 0f;

    void Awake()
    {
        if (Network.isClient)
        {
            enabled = false;
            //Debug.Log("Turning off");
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
            //Debug.Log("Turning on");
        }
    }

    void Update()
    {
        if (Network.player == theOwner)
        {
            float HInput = Input.GetAxis("Horizontal");
            float VInput = Input.GetAxis("Vertical");
            if (HInput != lastClientHInput || VInput != lastClientVInput)
            {
                lastClientHInput = HInput;
                lastClientVInput = VInput;
                if (Network.isServer)
                {
                    SendMovementInput(HInput, VInput);
                }
                else if (Network.isClient)
                {
                    networkView.RPC("SendMovementInput", RPCMode.Server, HInput, VInput);
                }
            }

        }
        //does the actual moving
        if (Network.isServer)
        {
            Vector3 moveDirection = new Vector3(serverCurrentHInput, 0, serverCurrentVInput);
            transform.Translate(speed * moveDirection * Time.deltaTime);
            if (transform.position.y < -5)
            {
                transform.position = new Vector3(transform.position.x, -4, transform.position.z);
            }
        }
    }

    [RPC]
    void SendMovementInput(float HInput, float VInput)
    {
        serverCurrentHInput = HInput;
        serverCurrentVInput = VInput;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = transform.position;
            var rot = transform.rotation;
            var scale = transform.localScale;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            stream.Serialize(ref scale);
        }
        else
        {
            Vector3 posReceive = Vector3.zero;
            var rot = transform.rotation;
            var scale = transform.localScale;
            stream.Serialize(ref posReceive);
            stream.Serialize(ref rot);
            stream.Serialize(ref scale);
            transform.position = posReceive;
            transform.rotation = rot;
            transform.localScale = scale;
        }
    }
}

