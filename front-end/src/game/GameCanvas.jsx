import { useRef, useEffect } from "react";
import playerFactory from "./playerFactory";
import gameEnvironmentFactory from "./gameEnvironmentFactory";
import fishFactory from "./fishFactory";
import obstaclesFactory from "./objstaclesFactoroy";
import InputAdapter from "./inputAdapter";

const GameCanvas = ({
  myConnectionId,
  connection,
  playersData,
  fishesData,
  gameEnvironmentData,
  obstaclesData,
}) => {
  const canvasRef = useRef(null);
  const playersRef = useRef(playersData);
  const fishesRef = useRef(fishesData);
  const obstaclesRef = useRef(obstaclesData);
  const inputAdapterRef = useRef(new InputAdapter());

  useEffect(() => {
    playersRef.current = playersData;
  }, [playersData]);
  useEffect(() => {
    fishesRef.current = fishesData;
  }, [fishesData]);
  useEffect(() => {
    obstaclesRef.current = obstaclesData;
  }, [obstaclesData]);

  useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext("2d");
    const inputAdapter = inputAdapterRef.current;

    canvas.width = gameEnvironmentData.width;
    canvas.height = gameEnvironmentData.height;

    const handleKeyDown = (e) => {
      inputAdapter.handleKeyDown(e);
    };
    const handleKeyUp = (e) => {
      inputAdapter.handleKeyUp(e);
    };

    window.addEventListener("keydown", handleKeyDown);
    window.addEventListener("keyup", handleKeyUp);

    const lastCastTimeRef = { current: 0 };
    const castCooldownMs = 300;

    const draw = () => {
      ctx.save();
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      ctx.translate(0, canvas.height);
      ctx.scale(1, -1);

      const gameEnvironment = gameEnvironmentFactory(ctx, gameEnvironmentData);
      const player = playersRef.current[myConnectionId];
      const commands = inputAdapter.getCommands();

      if (player) {
        const isRodCast = player.fishingRod.cast;

        console.log("Rod cast:", isRodCast);
        console.log("Commands:", commands);

        // Determine hook speed and apply freeze/slow effects (from old version)
        let hookSpeed = 5;

        if (player.isFrozen && player.freezeEndTime) {
          const now = Date.now();
          const freezeEndMs = new Date(player.freezeEndTime).getTime();
          if (now < freezeEndMs) {
            hookSpeed = 0;
          } else {
            player.isFrozen = false;
          }
        }

        if (player.isSlowed && player.slowdownEndTime) {
          const now = Date.now();
          const slowdownEndMs = new Date(player.slowdownEndTime).getTime();
          if (now < slowdownEndMs) {
            hookSpeed *= 0.5;
          } else {
            player.isSlowed = false;
          }
        }

        if (isRodCast) {
          // ROD CAST: Move hook with both WASD and Arrow keys (via inputAdapter commands)
          if (commands.moveUp) player.fishingRod.positionY += hookSpeed;
          if (commands.moveDown) player.fishingRod.positionY -= hookSpeed;
          if (commands.moveLeft) player.fishingRod.positionX -= hookSpeed;
          if (commands.moveRight) player.fishingRod.positionX += hookSpeed;

          if (
            commands.moveUp ||
            commands.moveDown ||
            commands.moveLeft ||
            commands.moveRight
          ) {
            connection
              .invoke(
                "MoveHook",
                player.fishingRod.positionX,
                player.fishingRod.positionY
              )
              .catch((err) => console.error(err));
          }
        } else {
          // ROD NOT CAST: Move boat with both A/D and Arrow Left/Right (via inputAdapter commands)
          if (commands.moveLeft) player.boat.positionX -= player.boat.movementSpeed;
          if (commands.moveRight) player.boat.positionX += player.boat.movementSpeed;

          if (commands.moveLeft || commands.moveRight) {
            connection
              .invoke("MoveBoatTo", player.boat.positionX)
              .catch((err) => console.error(err));
          }
        }

        // Apply bounds (same as before)
        if (player.fishingRod.positionX < 0) player.fishingRod.positionX = 0;
        if (player.fishingRod.positionX > gameEnvironmentData.width)
          player.fishingRod.positionX = gameEnvironmentData.width;
        if (player.fishingRod.positionY < 0) player.fishingRod.positionY = 0;
        if (player.fishingRod.positionY > gameEnvironmentData.waterHeight)
          player.fishingRod.positionY = gameEnvironmentData.waterHeight;

        if (player.boat.positionX < 0) player.boat.positionX = 0;
        if (
          player.boat.positionX >
          gameEnvironmentData.width - player.boat.width
        )
          player.boat.positionX = gameEnvironmentData.width - player.boat.width;

        // Cast toggle with cooldown (using inputAdapter cast trigger)
        const now = Date.now();
        if (commands.castTrigger && now - lastCastTimeRef.current > castCooldownMs) {
          connection.invoke("ToggleFishingRodCast").catch((err) => console.error(err));
          lastCastTimeRef.current = now;
          inputAdapter.clearCastTrigger();
        }
      }

      const players = Object.values(playersRef.current).map((p) =>
        playerFactory(ctx, p, myConnectionId)
      );
      const fishes = fishesRef.current.map((f) => fishFactory(ctx, f));
      const obstacles = obstaclesRef.current.map((o) => obstaclesFactory(ctx, o));

      gameEnvironment.drawEnvironment();
      obstacles.forEach((o) => o.drawObstacle());
      fishes.forEach((f) => f.drawFish());
      players.forEach((p) => {
        p.drawPlayer();
        p.drawHook();
        fishes.forEach((f) => {
          const caughtFishId = p.hasHookedFish(f.id, f.positionX, f.positionY, f.radius);
          if (caughtFishId !== null) {
            connection.invoke("CatchFish", caughtFishId).catch((err) => console.error("Failed to catch fish:", err));
          }
        });
      });

      ctx.restore();
      requestAnimationFrame(draw);
    };

    draw();

    return () => {
      window.removeEventListener("keydown", handleKeyDown);
      window.removeEventListener("keyup", handleKeyUp);
    };
  }, [connection, gameEnvironmentData]);

  return <canvas ref={canvasRef} />;
};

export default GameCanvas;
