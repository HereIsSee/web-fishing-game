import { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import "./App.css";
import GameCanvas from "./game/GameCanvas";
import {
  // playersData,
  // fishesData,
  gameEnvironmentData,
  obstaclesData,
} from "./game/dummyData.js";

function App() {
  const [playerName, setPlayerName] = useState("");
  const [joined, setJoined] = useState(false);
  const [connection, setConnection] = useState(null);
  const [myConnectionId, setMyConnectionId] = useState(null);

  // ----- This data will have to be initialized when the player joins the game
  // ----- and then updated accordingly to what you get from the backend
  // ----- events or player's own inputs movements
  const [playersData, setPlayersData] = useState({});
  const [fishesData, setFishesData] = useState([]);
  // const [gameEnvironmentData, setGameEnvironmentData] = useState({});
  // const [obstaclesData, setObstaclesData] = useState([]);
  // -------------------------------------------------------------------------

  useEffect(() => {
    // sukuriamas SignalR ryšys
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5112/gamehub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .configureLogging(signalR.LogLevel.Debug)
      .build();

    setConnection(newConnection);
  }, []);

  const joinGame = async () => {
    if (!playerName.trim() || !connection) return;

    try {
      console.log("🔄 Starting connection...");
      await connection.start();
      console.log("✅ Connected! ConnectionId: ");

      // Subscribe to backend events

      connection.on("ReceiveAllPlayers", (allPlayers) => {
        console.log("📥 Received all existing players:", allPlayers);

        const playersObj = {};
        allPlayers.forEach((player) => {
          playersObj[player.connectionId] = player;
        });

        setPlayersData(playersObj);
      });

      connection.on("PlayerJoined", (playerData) => {
        console.log("🎉 Player joined:", playerData);
        setPlayersData((prevPlayers) => ({
          ...prevPlayers,
          [playerData.connectionId]: playerData,
        }));
      });

      connection.on("PlayerLeft", (connectionId) => {
        console.log("🚪 Player left:", connectionId);
        // Update state if tracking players
        // setPlayers(prev => prev.filter(p => p.connectionId !== connectionId));
      });

      connection.on("GameStarted", (timerDuration) => {
        console.log("⏳ Game started! Timer duration:", timerDuration);
        //setGameStarted(true); // Start the game in UI
      });

      connection.on("BoatMoved", (playerId, positionX) => {
        console.log(`Player ${playerId} moved ${positionX}`);
        // Update other players' boat positions visually
      });

      connection.on("ReceiveConnectionId", (id) => {
        console.log("My connection ID:", id);
        setMyConnectionId(id);
      });
      connection.on("BoatMovedTo", (playerData) => {
        setPlayersData((prevPlayers) => ({
          ...prevPlayers,
          [playerData.connectionId]: playerData,
        }));
      });

      connection.on("UpdateFishes", (fishes) => {
        // Loop through fishes and update their positions in your canvas/scene
        setFishesData(fishes);
        // console.log(fishes);
      });

      connection.on("FishingRodCastChanged", (playerData) => {
        setPlayersData((prevPlayers) => ({
          ...prevPlayers,
          [playerData.connectionId]: playerData,
        }));
      });

      connection.on("HookMovedTo", (playerData) => {
        setPlayersData((prevPlayers) => ({
          ...prevPlayers,
          [playerData.connectionId]: playerData,
        }));
      });

      // Invoke join session on backendd
      await connection.invoke("JoinSession", playerName);
      console.log("✅ JoinSession called!");

      setJoined(true);
    } catch (err) {
      console.error("❌ FULL Error:", err);
    }
  };

  return (
    <div className="app">
      <h1>Fishing Game</h1>
      {!joined ? (
        <div className="join-screen">
          <h2>Join the Fishing Game</h2>
          <input
            type="text"
            placeholder="Enter your name"
            value={playerName}
            onChange={(e) => setPlayerName(e.target.value)}
          />
          <button onClick={joinGame}>Join Session</button>
        </div>
      ) : (
        <div>
          <h1>Got here</h1>
          <GameCanvas
            myConnectionId={myConnectionId}
            connection={connection}
            playersData={playersData}
            fishesData={fishesData}
            gameEnvironmentData={gameEnvironmentData}
            obstaclesData={obstaclesData}
          />
        </div>
      )}
    </div>
  );
}

export default App;
