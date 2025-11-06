import React from 'react';
import './Scoreboard.css';

const Scoreboard = ({ scoreboardData }) => {
    console.log('Scoreboard data:', scoreboardData); // ADD THIS LINE

    if (!scoreboardData) return <div className="scoreboard">Loading scoreboard...</div>;

  return (
    <div className="scoreboard">
      <h3>Fishing Game Scoreboard</h3>
      <div className="game-info">
        <div className="game-state">Status: {scoreboardData.currentGameState}</div>
        <div className="timer">Time: {scoreboardData.remainingTime}s</div>
      </div>
      <div className="scores">
        {Object.entries(scoreboardData.playerScores)
          .sort(([,a], [,b]) => b - a)
          .map(([playerName, score]) => (
            <div key={playerName} className="player-score">
              <span className="player-name">{playerName}</span>
              <span className="score">{score} pts</span>
            </div>
          ))
        }
      </div>
    </div>
  );
};

export default Scoreboard;