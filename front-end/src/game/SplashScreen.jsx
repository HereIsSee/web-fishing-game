import React, { useEffect, useState } from 'react';
import GameFacade, { WebUIClient } from './GameFacade';
import './Scoreboard.css';

const SplashScreen = ({ 
  playerScores = {}, 
  onRestart = () => {}, 
  onClose = () => {},
  connection = null,
  fishCollection: propFishCollection = null,  // Can receive from props
  performanceStats = null
}) => {
  const containerId = 'game-area';
  const [localFishCollection, setLocalFishCollection] = useState(null);
  const [hasFetched, setHasFetched] = useState(false);

  // Use prop fishCollection if available, otherwise use local state
  const fishCollection = propFishCollection || localFishCollection;

  const normalizedFishCollection = fishCollection
    ? {
        totalCaught: fishCollection.totalCaught ?? fishCollection.TotalCaught ?? 0,
        uniqueTypes: fishCollection.uniqueTypes ?? fishCollection.UniqueTypes ?? 0,
        fishByType: fishCollection.fishByType ?? fishCollection.FishByType ?? {},
        allFish: fishCollection.allFish ?? fishCollection.AllFish ?? [],
        playerName: fishCollection.playerName ?? fishCollection.PlayerName ?? "",
      }
    : null;
  
  // Derive achievements from fishCollection
  const achievements = normalizedFishCollection?.fishByType || {};
  const totalCatches = normalizedFishCollection?.totalCaught || 0;

  // Debug achievements data
  useEffect(() => {
    console.log("🏆 SplashScreen fishCollection:", normalizedFishCollection);
    console.log("🏆 Derived achievements:", achievements);
    console.log("🏆 Total catches:", totalCatches);
  }, [normalizedFishCollection, achievements, totalCatches]);

  // Fetch fish collection only once when component mounts (if not received via props)
  useEffect(() => {
    if (propFishCollection || !connection || hasFetched) return;

    const onFishCollection = (stats) => {
      console.log("🎣 SplashScreen received FishCollection:", stats);
      setLocalFishCollection(stats);
    };

    // Listen for the response (and clean up to avoid duplicate handlers)
    connection.off("FishCollection", onFishCollection);
    connection.on("FishCollection", onFishCollection);

    // Note: HubConnection.state is an enum; avoid string checks.
    // Just request and mark as fetched to prevent repeated invokes.
    connection
      .invoke("ShowPlayerFishCollection")
      .then(() => {
        console.log("✅ Fish collection requested");
        setHasFetched(true);
      })
      .catch((err) => {
        console.error("❌ Error fetching fish collection:", err);
        setHasFetched(true);
      });

    return () => {
      try {
        connection.off("FishCollection", onFishCollection);
      } catch (e) {
        // ignore
      }
    };
  }, [connection, hasFetched, propFishCollection]);

  // Normalize performanceStats (server sends camelCase by default)
  const facadeOps = performanceStats?.facadeOperations ?? performanceStats?.FacadeOperations ?? null;
  const normalizedFacadeOps = facadeOps
    ? {
        SessionDurationSeconds: facadeOps.SessionDurationSeconds ?? facadeOps.sessionDurationSeconds ?? 0,
        TotalMemoryUsedMB: facadeOps.TotalMemoryUsedMB ?? facadeOps.totalMemoryUsedMB ?? 0,
        TotalOperations: facadeOps.TotalOperations ?? facadeOps.totalOperations ?? 0,
        OperationStats: facadeOps.OperationStats ?? facadeOps.operationStats ?? {},
      }
    : null;

  const normalizedPerformanceStats = normalizedFacadeOps
    ? { facadeOperations: normalizedFacadeOps }
    : null;

  // Handle UI setup
  useEffect(() => {
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

  return (
    <>
      <div id={containerId} />
      
      {/* Achievements Section - Always show */}
      <div className="achievements-panel">
        <h2>🏆 Achievements Unlocked</h2>
        <p className="total-catches">Total Catches: <strong>{totalCatches}</strong></p>
        
        {achievements && Object.keys(achievements).length > 0 ? (
          <div className="achievement-grid">
            {Object.entries(achievements)
              .filter(([fishType, count]) => count > 0) // Only show fish that were caught
              .flatMap(([fishType, count]) => {
                const achievementList = [];
                
                // Define achievements based on fish type and count
                if (fishType === 'BlackFish') {
                  if (count >= 1) achievementList.push({ icon: '🖤', name: 'First Rare Catch', desc: `Caught your first BlackFish` });
                  if (count >= 5) achievementList.push({ icon: '⚡', name: 'Speed Hunter', desc: `Caught 5 BlackFish` });
                  if (count >= 10) achievementList.push({ icon: '🌟', name: 'Speed Demon', desc: `Mastered catching fast fish (${count} BlackFish)` });
                } else if (fishType === 'BlueFish') {
                  if (count >= 1) achievementList.push({ icon: '🎣', name: "Beginner's Luck", desc: `Caught your first fish` });
                  if (count >= 20) achievementList.push({ icon: '💙', name: 'Blue Collector', desc: `Caught 20 BlueFish` });
                  if (count >= 50) achievementList.push({ icon: '🌊', name: 'Ocean Master', desc: `Caught 50 BlueFish` });
                } else if (fishType === 'YellowFish') {
                  if (count >= 1) achievementList.push({ icon: '💛', name: 'Golden Opportunity', desc: `Caught your first YellowFish` });
                  if (count >= 5) achievementList.push({ icon: '✨', name: 'Golden Touch', desc: `Caught 5 YellowFish` });
                  if (count >= 15) achievementList.push({ icon: '👑', name: 'Gold Rush', desc: `Master of yellow fish (${count} YellowFish)` });
                } else if (fishType === 'BombFish') {
                  if (count >= 1) achievementList.push({ icon: '💣', name: 'Risk Taker', desc: `Brave enough to catch a BombFish` });
                  if (count >= 3) achievementList.push({ icon: '🎰', name: 'Adrenaline Junkie', desc: `Caught 3 BombFish` });
                  if (count >= 10) achievementList.push({ icon: '💥', name: 'Demolition Expert', desc: `Mastered the danger (${count} BombFish)` });
                } else if (fishType === 'FatFish') {
                  if (count >= 1) achievementList.push({ icon: '🐋', name: 'Big Catch', desc: `Caught your first FatFish` });
                  if (count >= 3) achievementList.push({ icon: '🎣', name: 'Whale Hunter', desc: `Caught 3 rare FatFish` });
                  if (count >= 10) achievementList.push({ icon: '👑', name: 'Leviathan Master', desc: `Legendary FatFish master (${count} catches)` });
                }
                
                return achievementList;
              })
              .map((achievement, index) => (
                <div key={index} className="achievement-card">
                  <div className="achievement-icon">{achievement.icon}</div>
                  <div className="achievement-details">
                    <h4>{achievement.name}</h4>
                    <p className="achievement-count">{achievement.desc}</p>
                  </div>
                </div>
              ))}
          </div>
        ) : (
          <p className="no-achievements">No fish caught yet in this session</p>
        )}
      </div>

      {/* Performance Stats Section */}
      {normalizedPerformanceStats && (
        <div className="performance-panel">
          <h2>📊 Proxy Pattern - Performance Results</h2>
          <p className="section-subtitle">Greitaveikos ir atminties naudojimo rezultatai</p>
          
          <div className="performance-sections">
            {/* GameFacade Performance */}
            {normalizedPerformanceStats.facadeOperations && (
              <div className="performance-section">
                <h3>🎮 GameFacade Operations</h3>
                <div className="stats-overview">
                  <div className="stat-item">
                    <span className="stat-label">Session Duration:</span>
                    <span className="stat-value">{normalizedPerformanceStats.facadeOperations.SessionDurationSeconds?.toFixed(2)}s</span>
                  </div>
                  <div className="stat-item">
                    <span className="stat-label">Total Memory:</span>
                    <span className="stat-value">{normalizedPerformanceStats.facadeOperations.TotalMemoryUsedMB?.toFixed(2)} MB</span>
                  </div>
                  <div className="stat-item">
                    <span className="stat-label">Total Operations:</span>
                    <span className="stat-value">{normalizedPerformanceStats.facadeOperations.TotalOperations}</span>
                  </div>
                </div>
                
                {normalizedPerformanceStats.facadeOperations.OperationStats && (
                  <div className="operations-list">
                    {Object.entries(normalizedPerformanceStats.facadeOperations.OperationStats).map(([opName, stats]) => (
                      <div key={opName} className="operation-stat">
                        <h4>{opName}</h4>
                        <div className="op-details">
                          <span>Calls: {stats.TotalCalls ?? stats.totalCalls}</span>
                          <span>Avg: {(stats.AverageTimeMs ?? stats.averageTimeMs)?.toFixed?.(3)}ms</span>
                          <span>Total: {(stats.TotalTimeMs ?? stats.totalTimeMs)?.toFixed?.(2)}ms</span>
                          <span>Memory: {(((stats.TotalMemoryBytes ?? stats.totalMemoryBytes) ?? 0) / 1024)?.toFixed(2)} KB</span>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            )}

            {/* GameHub Performance */}
            {performanceStats.hubOperations && (
              <div className="performance-section">
                <h3>🎯 GameHub Operations</h3>
                <div className="stats-overview">
                  <div className="stat-item">
                    <span className="stat-label">Session Duration:</span>
                    <span className="stat-value">{performanceStats.hubOperations.SessionDurationSeconds?.toFixed(2)}s</span>
                  </div>
                  <div className="stat-item">
                    <span className="stat-label">Total Memory:</span>
                    <span className="stat-value">{performanceStats.hubOperations.TotalMemoryUsedMB?.toFixed(2)} MB</span>
                  </div>
                  <div className="stat-item">
                    <span className="stat-label">Total Operations:</span>
                    <span className="stat-value">{performanceStats.hubOperations.TotalOperations}</span>
                  </div>
                </div>
                
                {performanceStats.hubOperations.OperationStats && (
                  <div className="operations-list">
                    {Object.entries(performanceStats.hubOperations.OperationStats).map(([opName, stats]) => (
                      <div key={opName} className="operation-stat">
                        <h4>{opName}</h4>
                        <div className="op-details">
                          <span>Calls: {stats.TotalCalls}</span>
                          <span>Avg: {stats.AverageTimeMs?.toFixed(3)}ms</span>
                          <span>Total: {stats.TotalTimeMs?.toFixed(2)}ms</span>
                          <span>Memory: {(stats.TotalMemoryBytes / 1024)?.toFixed(2)} KB</span>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            )}
            
            {/* VirtualAudioSubsystemProxy Performance */}
            {performanceStats.audioProxyOperations && (
              <div className="performance-section">
                <h3>🎵 VirtualAudioSubsystemProxy (Lazy Loading)</h3>
                <div className="stats-overview">
                  <div className="stat-item">
                    <span className="stat-label">Session Duration:</span>
                    <span className="stat-value">{performanceStats.audioProxyOperations.SessionDurationSeconds?.toFixed(2)}s</span>
                  </div>
                  <div className="stat-item">
                    <span className="stat-label">Total Memory:</span>
                    <span className="stat-value">{performanceStats.audioProxyOperations.TotalMemoryUsedMB?.toFixed(2)} MB</span>
                  </div>
                  <div className="stat-item">
                    <span className="stat-label">Initialized:</span>
                    <span className="stat-value">{performanceStats.audioProxyOperations.IsInitialized ? '✅ Yes' : '❌ No'}</span>
                  </div>
                </div>
                
                {performanceStats.audioProxyOperations.OperationStats && Object.keys(performanceStats.audioProxyOperations.OperationStats).length > 0 && (
                  <div className="operations-list">
                    {Object.entries(performanceStats.audioProxyOperations.OperationStats).map(([opName, stats]) => (
                      <div key={opName} className="operation-stat">
                        <h4>{opName}</h4>
                        <div className="op-details">
                          <span>Calls: {stats.TotalCalls}</span>
                          <span>Avg: {stats.AverageTimeMs?.toFixed(3)}ms</span>
                          <span>Total: {stats.TotalTimeMs?.toFixed(2)}ms</span>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            )}
            
            {/* SessionAccessControlProxy Performance */}
            {performanceStats.sessionProxyOperations && performanceStats.sessionProxyOperations.ProxyType !== "Not initialized" && (
              <div className="performance-section">
                <h3>🔐 SessionAccessControlProxy (Security)</h3>
                <div className="stats-overview">
                  <div className="stat-item">
                    <span className="stat-label">Session Duration:</span>
                    <span className="stat-value">{performanceStats.sessionProxyOperations.SessionDurationSeconds?.toFixed(2)}s</span>
                  </div>
                  <div className="stat-item">
                    <span className="stat-label">Total Memory:</span>
                    <span className="stat-value">{performanceStats.sessionProxyOperations.TotalMemoryUsedMB?.toFixed(2)} MB</span>
                  </div>
                  <div className="stat-item">
                    <span className="stat-label">Authorized:</span>
                    <span className="stat-value">{performanceStats.sessionProxyOperations.AuthorizedOperations}</span>
                  </div>
                  <div className="stat-item">
                    <span className="stat-label">Blocked:</span>
                    <span className="stat-value">{performanceStats.sessionProxyOperations.UnauthorizedAttempts}</span>
                  </div>
                  <div className="stat-item">
                    <span className="stat-label">Block Rate:</span>
                    <span className="stat-value">{performanceStats.sessionProxyOperations.BlockedPercentage}</span>
                  </div>
                </div>
                
                {performanceStats.sessionProxyOperations.OperationStats && Object.keys(performanceStats.sessionProxyOperations.OperationStats).length > 0 && (
                  <div className="operations-list">
                    {Object.entries(performanceStats.sessionProxyOperations.OperationStats).map(([opName, stats]) => (
                      <div key={opName} className="operation-stat">
                        <h4>{opName}</h4>
                        <div className="op-details">
                          <span>Calls: {stats.TotalCalls}</span>
                          <span>Avg: {stats.AverageTimeMs?.toFixed(3)}ms</span>
                          <span>Total: {stats.TotalTimeMs?.toFixed(2)}ms</span>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      )}
      
      {normalizedFishCollection && (
        <div className="fish-collection-modal">
          <div className="fish-collection-content">
            <h2>🎣 Your Catch Report</h2>
            
            <div className="fish-stats">
              <h3>📊 Statistics</h3>
              <p>Total Fish Caught: <strong>{normalizedFishCollection.totalCaught}</strong></p>
              <p>Unique Fish Types: <strong>{normalizedFishCollection.uniqueTypes}</strong></p>
            </div>

            <div className="fish-by-type">
              <h3>🐟 Fish by Type</h3>
              <ul>
                {Object.entries(normalizedFishCollection.fishByType || {}).map(([type, count]) => (
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
                {(normalizedFishCollection.allFish || []).map((fish, index) => (
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
              onClick={() => setLocalFishCollection(null)}
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