using UnityEngine;
using System.Collections;

public class NetworkingPlayerContainer
{
    private string _player;
    private NetworkViewID _viewID;

    public string Player { get { return _player; } set { _player = value; } }
    public NetworkViewID viewID { get { return _viewID; } set { _viewID = value; } }

    public NetworkingPlayerContainer(string player, NetworkViewID viewID)
    {
        _player = player;
        _viewID = viewID;
    }
}