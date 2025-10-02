import { useState, useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import './App.css'

function App() {
  const [playerName, setPlayerName] = useState('');
  const [joined, setJoined] = useState(false);
  const [connection, setConnection] = useState(null);

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

  const joinGame = async () => {
  if (!playerName.trim() || !connection) return;
  
  try {
    console.log('🔄 Starting connection...');
    await connection.start();
    console.log('✅ Connected! Calling JoinSession...');
    
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
      ) : (
        <div className="game-screen">
          <h2>Welcome, {playerName}!</h2>
          <p>Ready to fish! 🎣</p>
          <button onClick={() => console.log('Start game!')}>Start Game</button>
        </div>
      )}
    </div>
  );
}

export default App
