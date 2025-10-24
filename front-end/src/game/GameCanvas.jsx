import { useRef, useEffect } from "react";
import playerFactory from "./playerFactory";
import gameEnvironmentFactory from "./gameEnvironmentFactory";
import fishFactory from "./fishFactory";
import obstaclesFactory from "./objstaclesFactoroy";

const GameCanvas = ({
  myConnectionId,
  connection,
  playersData,
  fishesData,
  gameEnvironmentData,
  obstaclesData,
}) => {
  const canvasRef = useRef(null);

  // refs that track the latest data without re-rendering
  const playersRef = useRef(playersData);
  const fishesRef = useRef(fishesData);
  const obstaclesRef = useRef(obstaclesData);

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
    canvas.width = gameEnvironmentData.width;
    canvas.height = gameEnvironmentData.height;

    const keys = {};

    // Track key states
    const handleDown = (e) => {
      keys[e.key] = true;
    };
    const handleUp = (e) => {
      keys[e.key] = false;
    };

    window.addEventListener("keydown", handleDown);
    window.addEventListener("keyup", handleUp);

    // --- Use a ref to persist cast cooldown ---
    const lastCastTimeRef = { current: 0 };
    const castCooldownMs = 300; // 0.3s

    const draw = () => {
      ctx.save();
      ctx.clearRect(0, 0, canvas.width, canvas.height);

      ctx.translate(0, canvas.height);
      ctx.scale(1, -1);

      const gameEnvironment = gameEnvironmentFactory(ctx, gameEnvironmentData);

      const player = playersRef.current[myConnectionId];
      if (player) {
        // Boat movement (ArrowLeft / ArrowRight)
        if (keys["ArrowLeft"])
          player.boat.positionX -= player.boat.movementSpeed;
        if (keys["ArrowRight"])
          player.boat.positionX += player.boat.movementSpeed;

        // Hook movement (WASD)
        const hookSpeed = 5; // adjust as needed
        if (keys["w"] || keys["W"]) player.fishingRod.positionY += hookSpeed;
        if (keys["s"] || keys["S"]) player.fishingRod.positionY -= hookSpeed;
        if (keys["a"] || keys["A"]) player.fishingRod.positionX -= hookSpeed;
        if (keys["d"] || keys["D"]) player.fishingRod.positionX += hookSpeed;

        // Keep fishingRod inside water bounds
        if (player.fishingRod.positionX < 0) player.fishingRod.positionX = 0;
        if (player.fishingRod.positionX > gameEnvironmentData.width)
          player.fishingRod.positionX = gameEnvironmentData.width;
        if (player.fishingRod.positionY < 0) player.fishingRod.positionY = 0;
        if (player.fishingRod.positionY > gameEnvironmentData.waterHeight)
          player.fishingRod.positionY = gameEnvironmentData.waterHeight;

        // Send fishingRod position to server
        if (keys["w"] || keys["a"] || keys["s"] || keys["d"]) {
          connection
            .invoke(
              "MoveHook",
              player.fishingRod.positionX,
              player.fishingRod.positionY
            )
            .catch((err) => console.error(err));
        }

        const castKey = " ";
        const now = Date.now();
        if (keys[castKey] && now - lastCastTimeRef.current > castCooldownMs) {
          connection
            .invoke("ToggleFishingRodCast")
            .catch((err) => console.error(err));
          lastCastTimeRef.current = now;
        }

        // Keep boat inside bounds
        if (player.boat.positionX < 0) player.boat.positionX = 0;
        if (
          player.boat.positionX >
          gameEnvironmentData.width - player.boat.width
        )
          player.boat.positionX = gameEnvironmentData.width - player.boat.width;

        // Send boat position
        if (keys["ArrowLeft"] || keys["ArrowRight"]) {
          connection
            .invoke("MoveBoatTo", player.boat.positionX)
            .catch((err) => console.error(err));
        }
      }

      const players = Object.values(playersRef.current).map((p) =>
        playerFactory(ctx, p, myConnectionId)
      );
      const fishes = fishesRef.current.map((f) => fishFactory(ctx, f));
      const obstacles = obstaclesRef.current.map((o) =>
        obstaclesFactory(ctx, o)
      );

      gameEnvironment.drawEnvironment();
      obstacles.forEach((o) => o.drawObstacle());
      fishes.forEach((f) => f.drawFish());
      players.forEach((p) => {
        p.drawPlayer();
        p.drawHook();

        fishes.forEach((f) => {
          const caughtFishId = p.hasHookedFish(f.id, f.positionX, f.positionY);

          if (caughtFishId !== null) {
            console.log("Player has caught fish: ", caughtFishId);
            connection
              .invoke("CatchFish", caughtFishId)
              .catch((err) => console.error("Failed to catch fish:", err));
          }
        });
      });

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
