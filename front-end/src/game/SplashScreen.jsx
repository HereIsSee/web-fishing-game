import React, { useEffect } from 'react';
import GameFacade, { WebUIClient } from './GameFacade';
import './Scoreboard.css'; 

const SplashScreen = ({ playerScores = {}, onRestart = () => {}, onClose = () => {} }) => {
  useEffect(() => {
    const facade = new GameFacade();
    const client = new WebUIClient(facade, 'game-area'); 
    client.showScores(playerScores);

    const el = document.getElementById('game-area');
    const startBtn = el ? el.querySelector('#splash-restart') : null;
    const closeBtn = el ? el.querySelector('#splash-close') : null;
    if (startBtn) startBtn.addEventListener('click', onRestart);
    if (closeBtn) closeBtn.addEventListener('click', onClose);

    return () => {
      if (startBtn) startBtn.removeEventListener('click', onRestart);
      if (closeBtn) closeBtn.removeEventListener('click', onClose);
      facade.clearSplash('game-area');
    };
  }, [playerScores, onRestart, onClose]);

  return null;
};

export default SplashScreen;
