import { useState, useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import './App.css'
import Game from './game/Game';

function App() {
  const [playerName, setPlayerName] = useState('');
  const [joined, setJoined] = useState(false);
  const [connection, setConnection] = useState(null);
  const [gameStarted, setGameStarted] = useState(false);
  useEffect(() => {
    // sukuriamas SignalR ryšys
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5112/gamehub',{
        skipNegotiation: true,      
        transport: signalR.HttpTransportType.WebSockets 
      })
      .configureLogging(signalR.LogLevel.Debug)
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.on("BoatMoved", (playerId, positionX) => {
        console.log(`Player ${playerId} moved ${positionX}`);
        // Update other players' boat positions visually
      });
    }
  }, [connection]);

const joinGame = async () => {
    if (!playerName.trim() || !connection) return;

    try {
      console.log('🔄 Starting connection...');
      await connection.start();
      console.log('✅ Connected! Calling JoinSession...');

      // Subscribe to backend events
      connection.on('PlayerJoined', (newPlayerName) => {
        console.log('🎉 Player joined:', newPlayerName);
        // You could update state here if you track players
        // setPlayers(prev => [...prev, newPlayerName]);
      });

      connection.on('PlayerLeft', (connectionId) => {
        console.log('🚪 Player left:', connectionId);
        // Update state if tracking players
        // setPlayers(prev => prev.filter(p => p.connectionId !== connectionId));
      });

      connection.on('GameStarted', (timerDuration) => {
        console.log('⏳ Game started! Timer duration:', timerDuration);
        setGameStarted(true); // Start the game in UI
      });

      // Invoke join session on backend
      await connection.invoke('JoinSession', playerName);
      console.log('✅ JoinSession called!');

      setJoined(true);
    } catch (err) {
      console.error('❌ FULL Error:', err);
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
      ) : gameStarted ? (
        <div>
          <Game connection={connection} />
          <button onClick={()=> setGameStarted(false)}>Leave Game</button>
        </div>
      ) : (
        <div className="game-screen">
          <h2>Welcome, {playerName}!</h2>
          <p>Ready to fish! 🎣</p>
          <button onClick={() => setGameStarted(true)}>Start Game</button>
        </div>
      )}

    </div>
  );
}

export default App
