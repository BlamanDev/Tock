using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook 
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        TockPlayer player = gamePlayer.GetComponent<TockPlayer>();
        player.PlayerColor = lobby.playerColor;
        player.name = lobby.playerName;
        player.PlayerIndex = lobby.slot;
    }
}
