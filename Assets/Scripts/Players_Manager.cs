using UnityEngine;
using Unity.Netcode;
using Zeigblair.Core.Singletons;

public class Players_Manager : NetworkSingleton<Players_Manager>
{
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>();
    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                Debug.Log($"{id} connected");
                playersInGame.Value++;
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer)
            {
                Debug.Log($"{id} disconnected");
                playersInGame.Value--;
            }
        };
    }
}
