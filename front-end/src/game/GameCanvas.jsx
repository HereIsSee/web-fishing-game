import { useRef, useEffect } from 'react';
import playerFactory from './playerFactory';
import gameEnvironmentFactory from './gameEnvironmentFactory';
import fishFactory from './fishFactory';
import obstaclesFactory from './objstaclesFactoroy';

const GameCanvas = ({ myConnectionId, connection, playersData, fishesData, gameEnvironmentData, obstaclesData }) => {
  const canvasRef = useRef(null);
  
  // refs that track the latest data without re-rendering
  const playersRef = useRef(playersData);
  const fishesRef = useRef(fishesData);
  const obstaclesRef = useRef(obstaclesData);

  useEffect(() => { playersRef.current = playersData; }, [playersData]);
  useEffect(() => { fishesRef.current = fishesData; }, [fishesData]);
  useEffect(() => { obstaclesRef.current = obstaclesData; }, [obstaclesData]);

  useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext("2d");
    canvas.width = gameEnvironmentData.width;
    canvas.height = gameEnvironmentData.height;

    const keys = {};

    // Track key states
    const handleDown = (e) => { keys[e.key] = true; };
    const handleUp = (e) => { keys[e.key] = false; };

    window.addEventListener("keydown", handleDown);
    window.addEventListener("keyup", handleUp);

    const draw = () => {
      ctx.save();
      ctx.clearRect(0, 0, canvas.width, canvas.height);

      ctx.translate(0, canvas.height);
      ctx.scale(1, -1);

      const gameEnvironment = gameEnvironmentFactory(ctx, gameEnvironmentData);

      const player = playersRef.current[myConnectionId];
      if (player) {
        if (keys["ArrowLeft"]) player.boat.positionX -= player.boat.movementSpeed;
        if (keys["ArrowRight"]) player.boat.positionX += player.boat.movementSpeed;

        // Keep player in bounds
        if (player.boat.positionX < 0) player.boat.positionX = 0;
        if (player.boat.positionX > gameEnvironmentData.width - player.boat.width) {
          player.boat.positionX = gameEnvironmentData.width - player.boat.width;
        }

        // Send position to server continuously
        if (keys["ArrowLeft"] || keys["ArrowRight"]) {
          connection.invoke("MoveBoatTo", player.boat.positionX).catch(err => console.error(err));
        }
      }


      const players = Object.values(playersRef.current).map(p => playerFactory(ctx, p, myConnectionId));
      const fishes = fishesRef.current.map(f => fishFactory(ctx, f));
      const obstacles = obstaclesRef.current.map(o => obstaclesFactory(ctx, o));

      gameEnvironment.drawEnvironment();
      obstacles.forEach(o => o.drawObstacle());
      fishes.forEach(f => f.drawFish());
      players.forEach(p => p.drawPlayer());

      ctx.restore();
      requestAnimationFrame(draw);
    };

    draw();

    return () => {
      window.removeEventListener("keydown", handleDown);
      window.removeEventListener("keyup", handleUp);
    };
  }, [connection, gameEnvironmentData]);

  return <canvas ref={canvasRef} />;
};


export default GameCanvas;