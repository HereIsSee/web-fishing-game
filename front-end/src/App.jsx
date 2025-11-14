import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import "./App.css";
import GameCanvas from "./game/GameCanvas";
import SplashScreen from "./game/SplashScreen";
import Scoreboard from "./game/Scoreboard.jsx";
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
  const [scoreboardData, setScoreboardData] = useState(null);

  // Small ref used to allow the next ScoreboardUpdated to reset the timer
  // only when a real PlayerJoined event happened. This prevents routine
  // scoreboard updates (e.g., fish caught) from resetting the countdown.
  const allowTimerResetRef = useRef(false);

  // If two or more players have joined and the scoreboard isn't running yet,
  // start a local countdown as a client-side fallback (60 seconds).
  useEffect(() => {
    try {
      const playerCount = Object.keys(playersData || {}).length;
      if (playerCount >= 2) {
        // if server already set scoreboardData to Running, don't overwrite
        if (!scoreboardData || scoreboardData.currentGameState !== "Running") {
          setScoreboardData((prev) => ({
            ...(prev || { playerScores: {} }),
            currentGameState: "Running",
            timeRemaining: 60,
            playerScores: (prev && prev.playerScores) || {},
          }));
        }
      }
    } catch (e) {
      console.error("Failed to start local countdown:", e);
    }
  }, [playersData]);

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

        // Mark that a real player joined: allow the next ScoreboardUpdated to reset the timer.
        // This does not change how joining works; it only controls timer behavior.
        try {
          allowTimerResetRef.current = true;
        } catch (e) {
          // ignore
        }
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

      connection.on("PlayerUpdated", (playerData) => {
        console.log("🔄 Player updated with freeze/slowdown:", playerData);
        setPlayersData((prevPlayers) => ({
          ...prevPlayers,
          [playerData.connectionId]: playerData,
        }));
      });

      connection.on("ScoreboardUpdated", (data) => {
        try {
          const receivedAt = Date.now();
          // infer server-provided remaining seconds (if present)
          const rem =
            data.timeRemaining ??
            data.remainingTime ??
            data.timeLeft ??
            data.seconds ??
            null;
          const serverTs = data.serverTimestamp ?? data.serverTime ?? null;
          if (rem != null) {
            // If server provided its own timestamp, compute end time from that. Otherwise assume server time ~= now.
            const serverEndTime = (serverTs != null)
              ? serverTs + rem * 1000
              : Date.now() + rem * 1000;
            data._serverEndTime = serverEndTime;
            data._receivedAt = receivedAt;
          } else {
            data._receivedAt = receivedAt;
          }

          // Avoid resetting the timer for tiny server-side updates (e.g., on fish caught).
          setScoreboardData((prev) => {
            if (!prev) return data;
            const oldEnd = prev._serverEndTime ?? null;
            const newEnd = data._serverEndTime ?? null;

            // If game state changed on server, accept update (reset if server sent new time)
            if (prev.currentGameState !== data.currentGameState) {
              // consume join flag defensively
              try {
                allowTimerResetRef.current = false;
              } catch (e) {}
              return data;
            }

            // If a real player joined (PlayerJoined event seen), then allow the next scoreboard update to reset the timer.
            if (allowTimerResetRef.current) {
              try {
                allowTimerResetRef.current = false; // consume flag
              } catch (e) {}
              return data;
            }

            if (oldEnd && newEnd && prev.currentGameState === data.currentGameState) {
              const diff = Math.abs(oldEnd - newEnd);
              // If difference is small (<1s), don't reset the timer; only merge scores
              if (diff <= 1000) {
                return {
                  ...prev,
                  playerScores: data.playerScores ?? prev.playerScores,
                  _receivedAt: receivedAt,
                };
              }
            }
            return data;
          });
          console.log("🟢 SCOREBOARD DATA RECEIVED:", data);
        } catch (e) {
          console.error("Error handling ScoreboardUpdated:", e);
          setScoreboardData(data);
        }
      });

      connection.on("GameEnded", (result) => {
        console.log("🎉 Game ended! Winner:", result.winner, "Scores:", result.playerScores);
        // Update scoreboard to show Finished state with final scores
        setScoreboardData({
          playerScores: result.playerScores,
          currentGameState: "Finished",
          remainingTime: 0
        });
      });

      connection.on("GameReset", () => {
        console.log("🔄 Game reset by server");
        // Reset client state
        setPlayersData({});
        setFishesData([]);
        setScoreboardData(null);
        // Game is ready for next round (same players still connected)
      });

      // Invoke join session on backendd
      await connection.invoke("JoinSession", playerName);
      console.log("✅ JoinSession called!");

      setJoined(true);
    } catch (err) {
      console.error("❌ FULL Error:", err);
    }
  };

  const handlePlayAgain = async () => {
    if (!connection) return;
    try {
      console.log("🔄 Requesting game reset...");
      await connection.invoke("ResetGame");
      console.log("✅ Game reset request sent!");
    } catch (err) {
      console.error("❌ Error resetting game:", err);
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
          <Scoreboard scoreboardData={scoreboardData} />
          {(scoreboardData && (scoreboardData.currentGameState === 'Ended' || scoreboardData.currentGameState === 'Finished')) ? (
            <SplashScreen playerScores={scoreboardData.playerScores || {}} onRestart={handlePlayAgain} onClose={() => { /* close splash: clear server state or just hide */ }} />
          ) : (
            <GameCanvas
            myConnectionId={myConnectionId}
            connection={connection}
            playersData={playersData}
            fishesData={fishesData}
            gameEnvironmentData={gameEnvironmentData}
            obstaclesData={obstaclesData}
          />
          )}
        </div>
      )}
    </div>
  );
}

export default App;
