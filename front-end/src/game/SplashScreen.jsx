import React, { useEffect, useState } from 'react';
import GameFacade, { WebUIClient } from './GameFacade';
import './Scoreboard.css';

const SplashScreen = ({ 
  playerScores = {}, 
  onRestart = () => {}, 
  onClose = () => {},
  connection = null  // Add connection prop
}) => {
  const containerId = 'game-area';
  const [fishCollection, setFishCollection] = useState(null);

  useEffect(() => {
    // Fetch fish collection when splash screen loads
    if (connection && connection.state === 'Connected') {
      fetchFishCollection();
    }

    const facade = new GameFacade();
    const client = new WebUIClient(facade, containerId);
    client.showScores(playerScores);

    const container = document.getElementById(containerId);
    const startBtn = container ? container.querySelector('#splash-restart') : null;
    const closeBtn = container ? container.querySelector('#splash-close') : null;

    if (startBtn) startBtn.addEventListener('click', onRestart);
    if (closeBtn) closeBtn.addEventListener('click', onClose);

    return () => {
      if (startBtn) startBtn.removeEventListener('click', onRestart);
      if (closeBtn) closeBtn.removeEventListener('click', onClose);
      facade.clearSplash(containerId);
    };
  }, [playerScores, onRestart, onClose]);

  const fetchFishCollection = async () => {
    try {
      await connection.invoke("ShowPlayerFishCollection");
    } catch (err) {
      console.error("❌ Error fetching fish collection:", err);
    }
  };

  return (
    <>
      <div id={containerId} />
      {fishCollection && (
        <div className="fish-collection-modal">
          <div className="fish-collection-content">
            <h2>🎣 Your Catch Report</h2>
            
            <div className="fish-stats">
              <h3>📊 Statistics</h3>
              <p>Total Fish Caught: <strong>{fishCollection.TotalCaught}</strong></p>
              <p>Unique Fish Types: <strong>{fishCollection.UniqueTypes}</strong></p>
            </div>

            <div className="fish-by-type">
              <h3>🐟 Fish by Type</h3>
              <ul>
                {Object.entries(fishCollection.FishByType).map(([type, count]) => (
                  <li key={type}>
                    <span className="fish-type">{type}:</span>
                    <span className="fish-count">{count} fish</span>
                  </li>
                ))}
              </ul>
            </div>

            <div className="all-fish-list">
              <h3>📝 All Fish Caught (in order)</h3>
              <div className="fish-items">
                {fishCollection.AllFish.map((fish, index) => (
                  <div key={index} className="fish-item" style={{ color: fish.Color }}>
                    <span className="fish-number">{index + 1}.</span>
                    <span className="fish-type">{fish.Type}</span>
                    <span className="fish-points">+{fish.Points} points</span>
                  </div>
                ))}
              </div>
            </div>

            <button 
              className="close-collection-btn" 
              onClick={() => setFishCollection(null)}
            >
              Close
            </button>
          </div>
        </div>
      )}
    </>
  );
};

export default SplashScreen;