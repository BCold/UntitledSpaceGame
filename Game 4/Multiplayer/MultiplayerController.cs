using Godot;
using System;

public partial class MultiplayerController : Control
{	
		[Export] private int port = 8910;
		[Export] private string address = "127.0.0.1";

		private ENetMultiplayerPeer peer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
	}

	/// <summary>
	/// Runs when the connection fails and only on the client.
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
    private void ConnectionFailed()
    {
        GD.Print("CONNECTION FAILED.");
    }

	/// <summary>
	/// Runs when the connection is successful and only on the clients.
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
    private void ConnectedToServer()
    {
        GD.Print("CONNECTED TO SERVER.");
		// RpcID means I want an RPC to this specific ID. In this case the host.
		RpcId(1,"SendPlayerInformation", GetNode<LineEdit>("LineEdit").Text, Multiplayer.GetUniqueId());
    }

	/// <summary>
	/// Runs when a player disconnects and runs on all peers.
	/// </summary>
	/// <param name="id">id of the player that disconnected</param>
	/// <exception cref="NotImplementedException"></exception>
    private void PeerDisconnected(long id){
		GD.Print($"{id} DISCCONNECTED.");
    }

	/// <summary>
	/// Runs when a player connects and runs on all peers.
	/// </summary>
	/// <param name="id">id of the player that connected</param>
	/// <exception cref="NotImplementedException"></exception>
	public void PeerConnected(long id){
		GD.Print($"{id} CONNECTED.");
	}

	public void _on_host_button_down(){
		peer = new();
		Error error = peer.CreateServer(port, 2);
		if (error != Error.Ok){
			GD.Print($"ERROR: CANNOT HOST! {error}");
			return;
		}
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("WAITING FOR PLAYERS...");
		// Host sending themselves their own player information
		SendPlayerInformation(GetNode<LineEdit>("LineEdit").Text, 1);
	}

	public void _on_join_button_down(){
		peer = new();
		Error error = peer.CreateClient(address, port);
		if (error != Error.Ok){
			GD.Print($"ERROR: CANNOT JOIN! {error}");
			return;
		}
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("JOINING GAME...");
	}

	public void _on_start_game_button_down(){
		Rpc("StartGame");
	}

	// RPC
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true,TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame(){
		foreach(var item in GameManager.Players){
			GD.Print($"{item.Name} is playing.");
		}
		Node2D scene = ResourceLoader.Load<PackedScene>("res://Levels/Test.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		this.Hide();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SendPlayerInformation(string name, int id){
		PlayerInfo playerInfo = new(){
			Name = name,
			Id = id
		};

		if (!GameManager.Players.Contains(playerInfo)){
			GameManager.Players.Add(playerInfo);
		}

		if (Multiplayer.IsServer()){
			foreach (var item in GameManager.Players){
				Rpc("SendPlayerInformation",item.Name, item.Id);
			}
		}
	}
}
