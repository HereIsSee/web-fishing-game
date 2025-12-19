import React, { useEffect, useState, useRef } from "react";
import "./Scoreboard.css";

const Scoreboard = ({ scoreboardData, fishCollection }) => {
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

  const totalCaught = fishCollection?.totalCaught ?? fishCollection?.TotalCaught ?? 0;
  const uniqueTypes = fishCollection?.uniqueTypes ?? fishCollection?.UniqueTypes ?? 0;
  const fishByType = fishCollection?.fishByType ?? fishCollection?.FishByType ?? {};
  const allFish = fishCollection?.allFish ?? fishCollection?.AllFish ?? [];

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
        <>
          <div className="game-over-banner">
            <h2>Game Over</h2>
            <p>Final scores:</p>
          </div>
          
          {/* 🎣 FISH COLLECTION SECTION - ALWAYS SHOW DETAILS */}
          {fishCollection && (
            <div className="fish-collection">
              <div className="fish-summary">
                <h4>🎣 Your Catch: {totalCaught} fish</h4>
              </div>
              
              <div className="fish-details">
                <div className="fish-stats">
                  <p><strong>Total caught:</strong> {totalCaught}</p>
                  <p><strong>Unique types:</strong> {uniqueTypes}</p>
                </div>
                
                <div className="fish-by-type">
                  <h5>Fish by Type:</h5>
                  <ul>
                    {Object.entries(fishByType || {}).map(([type, count]) => (
                      <li key={type}>
                        <span className="fish-type">{type}:</span>
                        <span className="fish-count">{count}</span>
                      </li>
                    ))}
                  </ul>
                </div>
                
                <div className="all-fish-list">
                  <h5>All Fish Caught:</h5>
                  <div className="fish-items">
                    {(allFish || []).map((fish, index) => (
                      <div key={index} className="fish-item" style={{ color: fish.Color }}>
                        <span className="fish-number">{index + 1}.</span>
                        <span className="fish-type">{fish.Type}</span>
                        <span className="fish-points">+{fish.Points} points</span>
                      </div>
                    ))}
                  </div>
                </div>
              </div>
            </div>
          )}
        </>
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