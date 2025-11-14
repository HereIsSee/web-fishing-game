import React, { useEffect } from 'react';
import GameFacade, { WebUIClient } from './GameFacade';
import './Scoreboard.css';

const SplashScreen = ({ playerScores = {}, onRestart = () => {}, onClose = () => {} }) => {
  const containerId = 'game-area';

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

  return <div id={containerId} />;
};

export default SplashScreen;
