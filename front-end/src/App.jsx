import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import "./App.css";
import GameCanvas from "./game/GameCanvas";
import SplashScreen from "./game/SplashScreen";
import Scoreboard from "./game/Scoreboard.jsx";
import { playSynthSound } from "./game/audioGenerator.js";
import {
  // playersData,
  // fishesData,
  gameEnvironmentData,
  obstaclesData,
} from "./game/dummyData.js";

const useAudioManager = () => {
  const audioContextRef = useRef(null);
  const audioBuffersRef = useRef({});

  const initAudioContext = () => {
    if (!audioContextRef.current) {
      try {
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        audioContextRef.current = audioContext;
        console.log(" Audio context initialized");
      } catch (e) {
        console.error(" Audio context failed:", e);
      }
    }
    return audioContextRef.current;
  };

  const playSound = async (soundType) => {
    const audioContext = initAudioContext();
    if (!audioContext) return;

    try {
      const soundMap = {
        catch: "/sounds/catch.wav",
        miss: "/sounds/miss.wav",
        freeze: "/sounds/freeze.wav",
        bomb: "/sounds/bomb.wav",
      };

      const soundPath = soundMap[soundType];
      if (!soundPath) {
        console.warn(`⚠️ Unknown sound type: ${soundType}`);
        return;
      }

      try {
        if (!audioBuffersRef.current[soundType]) {
          const response = await fetch(soundPath);
          if (!response.ok) throw new Error(`Failed to load ${soundPath}`);
          const arrayBuffer = await response.arrayBuffer();
          const audioBuffer = await audioContext.decodeAudioData(arrayBuffer);
          audioBuffersRef.current[soundType] = audioBuffer;
        }

        const source = audioContext.createBufferSource();
        source.buffer = audioBuffersRef.current[soundType];
        source.connect(audioContext.destination);
        source.start(0);
        console.log(` Playing file-based sound: ${soundType}`);
      } catch (fileError) {
        console.log(` File load failed, using synthesized ${soundType} sound`);
        playSynthSound(audioContext, soundType);
      }
    } catch (e) {
      console.error(` Error playing sound ${soundType}:`, e);
      try {
        const audioContext = initAudioContext();
        if (audioContext) playSynthSound(audioContext, soundType);
      } catch (synthError) {
        console.error(` Synth fallback also failed:`, synthError);
      }
    }
  };

  return { playSound, initAudioContext };
};

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
  const [persistentPlayerId, setPersistentPlayerId] = useState(null);
  const [fishCollection, setFishCollection] = useState(null);

  // Small ref used to allow the next ScoreboardUpdated to reset the timer
  // only when a real PlayerJoined event happened. This prevents routine
  // scoreboard updates (e.g., fish caught) from resetting the countdown.
  const allowTimerResetRef = useRef(false);

  const { playSound, initAudioContext } = useAudioManager();

  // If two or more players have joined and the scoreboard isn't running yet,
  // start a local countdown as a client-side fallback (60 seconds).
  useEffect(() => {
    console.log("🎯 [FRONTEND] Setting up page unload detection for leaving");
    const handleBeforeUnload = async () => {
      console.log("🎯 [FRONTEND] Browser/tab closing - notifying server");
      if (connection && connection.state === 'Connected') {
        try {
          // Try to notify server we're leaving
          await connection.invoke("LeaveSession");
        } catch (e) {
          // Ignore - page is unloading anyway
        }
      }
    };

    window.addEventListener('beforeunload', handleBeforeUnload);
    
    return () => {
      window.removeEventListener('beforeunload', handleBeforeUnload);
    };
  }, [connection]);




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

  const getPersistentId = () => {
  let pid = localStorage.getItem('fishing_persistent_id');
    if (!pid) {
      pid = 'player_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
      localStorage.setItem('fishing_persistent_id', pid);
    }
    return pid;
  };

  const joinGame = async () => {

    console.log("🔄 Clearing previous session data...");
    localStorage.removeItem(`active_${playerName}`);
    
    

    if (!playerName.trim()) return;
    console.log(`🎯 [FRONTEND] Player name: "${playerName}"`);

    

    // CHECK: Prevent multiple tabs with same player
    const browserSessionId = localStorage.getItem('browser_session_id');
    const activeBrowser = localStorage.getItem('active_browser');

    console.log("🔍 TAB CHECK - BrowserSessionId:", browserSessionId, "ActiveBrowser:", activeBrowser);

    if (activeBrowser && activeBrowser === browserSessionId) {
      console.log("❌ TAB BLOCKED - Same browser session detected!");
      alert("❗ You already have this game open in another tab!");
      return;
    } else {
      console.log("✅ TAB ALLOWED - No active session or different session");
    }

    // Prevent multiple clicks
    if (connection && connection.state === 'Connected') {
      console.log("⚠️ Already connected!");
      return;
    }
        

    try {
      console.log("🔄 Starting connection...");
      
      // Create NEW connection if none exists or if disconnected
      let currentConnection = connection;
      
      if (!currentConnection || currentConnection.state === 'Disconnected') {
        const newConnection = new signalR.HubConnectionBuilder()
          .withUrl("http://localhost:5112/gamehub", {
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets,
          })
          .configureLogging(signalR.LogLevel.Debug)
          .build();
        
        setConnection(newConnection);
        currentConnection = newConnection;
      }
      
      // Start connection if not already started
      if (currentConnection.state !== 'Connected') {
        await currentConnection.start();
        console.log("✅ Connected!");
      }

      let sessionId = localStorage.getItem('browser_session_id');
      if (!sessionId) {
        sessionId = 'browser_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
        localStorage.setItem('browser_session_id', sessionId);
      }
      localStorage.setItem('active_browser', sessionId);
      console.log("✅ SET ACTIVE BROWSER:", sessionId, "for player:", playerName);
      // Subscribe to backend events (only once)
      if (!currentConnection._eventsRegistered) {
        currentConnection.on("ReceiveAllPlayers", (allPlayers) => {
          console.log("📥 Received all existing players:", allPlayers);
          const playersObj = {};
          allPlayers.forEach((player) => {
            playersObj[player.connectionId] = player;
          });
          setPlayersData(playersObj);
        });

        currentConnection.on("PlayerJoined", (playerData) => {
          console.log("🎉 Player joined:", playerData);
          setPlayersData((prevPlayers) => ({
            ...prevPlayers,
            [playerData.connectionId]: playerData,
          }));
          try {
            allowTimerResetRef.current = true;
          } catch (e) {
            // ignore
          }
        });

        currentConnection.on("PlayerLeft", (connectionId) => {
          console.log("🚪 Player left:", connectionId);
          console.log("🗑️ Removing player data for connection:", connectionId);
          
          setPlayersData((prevPlayers) => {
            const newPlayers = { ...prevPlayers };
            delete newPlayers[connectionId];
            console.log("✅ Players after removal:", Object.keys(newPlayers));
            return newPlayers;
          });
        });

        currentConnection.on("GameStarted", (timerDuration) => {
          console.log("⏳ Game started! Timer duration:", timerDuration);
        });

        currentConnection.on("BoatMoved", (playerId, positionX) => {
          console.log(`Player ${playerId} moved ${positionX}`);
        });

        currentConnection.on("ReceiveConnectionId", (id) => {
          console.log("My connection ID:", id);
          setMyConnectionId(id);
        });

        currentConnection.on("BoatMovedTo", (playerData) => {
          setPlayersData((prevPlayers) => ({
            ...prevPlayers,
            [playerData.connectionId]: playerData,
          }));
        });

        currentConnection.on("UpdateFishes", (fishes) => {
          setFishesData(fishes);
        });

        currentConnection.on("FishingRodCastChanged", (playerData) => {
          setPlayersData((prevPlayers) => ({
            ...prevPlayers,
            [playerData.connectionId]: playerData,
          }));
        });

        currentConnection.on("HookMovedTo", (playerData) => {
          setPlayersData((prevPlayers) => ({
            ...prevPlayers,
            [playerData.connectionId]: playerData,
          }));
        });

        currentConnection.on("PlayerUpdated", (playerData) => {
          console.log("🔄 Player updated with freeze/slowdown:", playerData);
          setPlayersData((prevPlayers) => ({
            ...prevPlayers,
            [playerData.connectionId]: playerData,
          }));
        });

        currentConnection.on("ScoreboardUpdated", (data) => {
          try {
            const receivedAt = Date.now();
            const rem =
              data.timeRemaining ??
              data.remainingTime ??
              data.timeLeft ??
              data.seconds ??
              null;
            const serverTs = data.serverTimestamp ?? data.serverTime ?? null;
            if (rem != null) {
              const serverEndTime = (serverTs != null)
                ? serverTs + rem * 1000
                : Date.now() + rem * 1000;
              data._serverEndTime = serverEndTime;
              data._receivedAt = receivedAt;
            } else {
              data._receivedAt = receivedAt;
            }

            setScoreboardData((prev) => {
              if (!prev) return data;
              const oldEnd = prev._serverEndTime ?? null;
              const newEnd = data._serverEndTime ?? null;

              if (prev.currentGameState !== data.currentGameState) {
                try {
                  allowTimerResetRef.current = false;
                } catch (e) {}
                return data;
              }

              if (allowTimerResetRef.current) {
                try {
                  allowTimerResetRef.current = false;
                } catch (e) {}
                return data;
              }

              if (oldEnd && newEnd && prev.currentGameState === data.currentGameState) {
                const diff = Math.abs(oldEnd - newEnd);
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

        currentConnection.on("PlaySound", (soundType) => {
          console.log(`Received PlaySound event: ${soundType}`);
          playSound(soundType);
        });

        currentConnection.on("GameEnded", (result) => {
          console.log("🎉 Game ended! Winner:", result.winner, "Scores:", result.playerScores);
          setScoreboardData({
            playerScores: result.playerScores,
            currentGameState: "Finished",
            remainingTime: 0
          });

          // 🎣 NEW: Fetch fish collection when game ends
          if (currentConnection.state === 'Connected') {
            currentConnection.invoke("ShowPlayerFishCollection");
          }
        });

        currentConnection.on("FishCollection", (stats) => {
          console.log("🎣🎣🎣 YOUR CATCH REPORT 🎣🎣🎣", stats);
          
          // SAFE ACCESS - handle null/undefined
          const total = stats?.TotalCaught ?? 0;
          console.log(`Total fish caught: ${total}`);
          
          setFishCollection(stats);
          
          console.log("\n📊 By type:");
          
          // FIXED: Add null check before Object.entries
          const fishByType = stats?.FishByType ?? {};
          for (const [type, count] of Object.entries(fishByType)) {
            console.log(`  ${type}: ${count} fish`);
          }
        });

        currentConnection.on("GameReset", () => {
          console.log("🔄 Game reset by server");
          setPlayersData({});
          setFishesData([]);
          setScoreboardData(null);
          setFishCollection(null); // ✅ Also reset fish collection
        });

        currentConnection.on("ReceivePersistentId", (serverPersistentId) => {
          console.log("🔑 Received persistent ID:", serverPersistentId);
          localStorage.setItem('fishing_persistent_id', serverPersistentId);
          setPersistentPlayerId(serverPersistentId);
        });

        currentConnection.on("ScoreSaved", (data) => {
          console.log("💾 Score saved:", data);
          if (data.encryptedData) {
            localStorage.setItem(`fishing_save_${playerName}`, data.encryptedData);
          }
        });

        currentConnection.on("ScoreLoaded", (data) => {
          console.log("🔄 Score loaded:", data);
          console.log(`Loaded saved score: ${data.score} points!`);
        });

        currentConnection.on("SaveFailed", (error) => {
          console.error("❌ Save failed:", error);
        });

        currentConnection.on("LoadFailed", (error) => {
          console.error("❌ Load failed:", error);
        });

        currentConnection.on("ClearActivePlayer", (playerName) => {
          console.log("🧹🧹🧹 SERVER CLEAR ACTIVE PLAYER CALLED FOR:", playerName);
          console.log("🧹 BEFORE clear - active_browser:", localStorage.getItem('active_browser'));
          
          localStorage.removeItem('active_browser');
          localStorage.removeItem(`active_${playerName}`);
          
          console.log("🧹 AFTER clear - active_browser:", localStorage.getItem('active_browser'));
        });

        // Connection state handlers
        currentConnection.onclose(() => {
          console.log("🔌 Connection closed");
          
          // Clear the browser lock immediately
          localStorage.removeItem('active_browser');
          
          // ALSO clear by player name (remove old logic)
          localStorage.removeItem(`active_${playerName}`);
          
          setJoined(false);
        });


        currentConnection.onreconnecting(() => {
          console.log("🔄 Reconnecting...");
        });

        currentConnection.onreconnected(() => {
          console.log("✅ Reconnected!");
          localStorage.setItem(`active_${playerName}`, 'active');
        });

        // Mark that events have been registered
        currentConnection._eventsRegistered = true;
      }

      // Invoke join session
      const persistentId = getPersistentId();
      console.log("🔑 Using persistent ID:", persistentId);
      
      await currentConnection.invoke("JoinSession", playerName, persistentId);
      console.log("✅ JoinSession called with persistent ID:", persistentId);

      setJoined(true);

    } catch (err) {
      console.error("❌ Connection error:", err);
      
      // Reset connection state on error
      localStorage.setItem(`active_${playerName}`, 'disconnected');
      
      if (connection) {
        try {
          await connection.stop();
        } catch (stopErr) {
          // Ignore stop errors
        }
        setConnection(null);
      }
      setJoined(false);
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
          <Scoreboard 
            scoreboardData={scoreboardData} 
            fishCollection={fishCollection}
          />
          
          {/* KEEP SPLASHSCREEN BUT PASS fishCollection TO IT */}
          {(scoreboardData && (scoreboardData.currentGameState === 'Ended' || scoreboardData.currentGameState === 'Finished')) ? (
            <SplashScreen 
              playerScores={scoreboardData.playerScores || {}} 
              onRestart={handlePlayAgain} 
              onClose={() => {}} 
              fishCollection={fishCollection}  // ADD THIS
              connection={connection}  // ADD THIS
            />
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
