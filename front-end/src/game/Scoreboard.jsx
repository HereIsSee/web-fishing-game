
import React, { useEffect, useState, useRef } from "react";
import "./Scoreboard.css";

const Scoreboard = ({ scoreboardData }) => {
  const [now, setNow] = useState(Date.now());
  const rafRef = useRef();

  useEffect(() => {
    const tick = () => {
      setNow(Date.now());
      rafRef.current = requestAnimationFrame(tick);
    };
    rafRef.current = requestAnimationFrame(tick);
    return () => {
      if (rafRef.current) cancelAnimationFrame(rafRef.current);
    };
  }, []);

  if (!scoreboardData)
    return <div className="scoreboard">Loading scoreboard...</div>;

  let displayTime = "--";
  const serverEnd = scoreboardData._serverEndTime ?? null;
  if (serverEnd != null) {
    const remainingMs = serverEnd - now;
    displayTime = Math.max(0, Math.ceil(remainingMs / 1000));
  } else {
    const rem =
      scoreboardData.timeRemaining ??
      scoreboardData.remainingTime ??
      scoreboardData.gameTime ??
      scoreboardData.timeLeft ??
      scoreboardData.seconds ??
      null;
    displayTime = rem != null ? Math.max(0, Math.ceil(rem)) : "--";
  }

  const isEnded =
    scoreboardData.currentGameState === "Ended" ||
    scoreboardData.currentGameState === "Finished" ||
    displayTime === 0;

  return (
    <div className="scoreboard">
      <h3>Fishing Game Scoreboard</h3>
      <div className="game-info">
        <div className="game-state">
          Status: {scoreboardData.currentGameState}
        </div>
        <div className="game-time">Time left: {displayTime}</div>
      </div>

      {isEnded && (
        <div className="game-over-banner">
          <h2>Game Over</h2>
          <p>Final scores:</p>
        </div>
      )}

      <div className="scores-list">
        {Object.entries(scoreboardData.playerScores || {})
          .sort(([, a], [, b]) => b - a)
          .map(([playerName, score]) => (
            <div key={playerName} className="player-score">
              <span className="player-name">{playerName}</span>
              <span className="score">{score} pts</span>
            </div>
          ))}
      </div>
    </div>
  );
};

export default Scoreboard;
