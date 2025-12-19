import { useRef, useEffect } from "react";
import playerFactory from "./playerFactory";
import gameEnvironmentFactory from "./gameEnvironmentFactory";
import fishFactory from "./fishFactory";
import obstaclesFactory from "./objstaclesFactoroy";
import InputAdapter from "./inputAdapter";

const getHazardSpeedMultiplier = (hookX, hookY, hazardZones) => {
  if (!hazardZones || hazardZones.length === 0) return 1;

  let mult = 1;
  for (const z of hazardZones) {
    const dx = hookX - z.x;
    const dy = hookY - z.y;
    const dist = Math.sqrt(dx * dx + dy * dy);
    if (dist <= z.radius) {
      mult = Math.min(mult, z.speedMultiplier ?? 1);
    }
  }
  return mult;
};

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
    if (!gameEnvironmentData) return;

    const canvas = canvasRef.current;
    const ctx = canvas.getContext("2d");
    const inputAdapter = inputAdapterRef.current;

    canvas.width = gameEnvironmentData.width;
    canvas.height = gameEnvironmentData.height;

    const handleKeyDown = (e) => inputAdapter.handleKeyDown(e);
    const handleKeyUp = (e) => inputAdapter.handleKeyUp(e);

    window.addEventListener("keydown", handleKeyDown);
    window.addEventListener("keyup", handleKeyUp);

    const lastCastTimeRef = { current: 0 };
    const castCooldownMs = 300;

    const draw = () => {
      ctx.save();
      ctx.clearRect(0, 0, canvas.width, canvas.height);

      // Flip coordinate system (your existing setup)
      ctx.translate(0, canvas.height);
      ctx.scale(1, -1);

      const gameEnvironment = gameEnvironmentFactory(ctx, gameEnvironmentData);
      const player = playersRef.current?.[myConnectionId];
      const commands = inputAdapter.getCommands();

      if (player) {
        const isRodCast = !!player.fishingRod?.cast;
        let hookSpeed = 5;

        // Freeze
        if (player.isFrozen && player.freezeEndTime) {
          const now = Date.now();
          const freezeEndMs = new Date(player.freezeEndTime).getTime();
          if (now < freezeEndMs) hookSpeed = 0;
          else player.isFrozen = false;
        }

        // Slow from fish effects
        if (player.isSlowed && player.slowdownEndTime) {
          const now = Date.now();
          const slowdownEndMs = new Date(player.slowdownEndTime).getTime();
          if (now < slowdownEndMs) hookSpeed *= 0.5;
          else player.isSlowed = false;
        }

        if (isRodCast) {
          // Hazard zone slowdown (environment-driven)
          const hx = player.fishingRod.positionX;
          const hy = player.fishingRod.positionY;
          hookSpeed *= getHazardSpeedMultiplier(
            hx,
            hy,
            gameEnvironmentData.hazardZones
          );

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
              ?.invoke(
                "MoveHook",
                player.fishingRod.positionX,
                player.fishingRod.positionY
              )
              .catch((err) => console.error(err));
          }
        } else {
          if (commands.moveLeft)
            player.boat.positionX -= player.boat.movementSpeed;
          if (commands.moveRight)
            player.boat.positionX += player.boat.movementSpeed;

          if (commands.moveLeft || commands.moveRight) {
            connection
              ?.invoke("MoveBoatTo", player.boat.positionX)
              .catch((err) => console.error(err));
          }
        }

        // Bounds
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

        // Cast toggle
        const now = Date.now();
        if (
          commands.castTrigger &&
          now - lastCastTimeRef.current > castCooldownMs
        ) {
          connection
            ?.invoke("ToggleFishingRodCast")
            .catch((err) => console.error(err));
          lastCastTimeRef.current = now;
          inputAdapter.clearCastTrigger();
        }
      }

      const players = Object.values(playersRef.current || {}).map((p) =>
        playerFactory(ctx, p, myConnectionId)
      );
      const fishes = (fishesRef.current || []).map((f) => fishFactory(ctx, f));
      const obstacles = (obstaclesRef.current || []).map((o) =>
        obstaclesFactory(ctx, o)
      );

      // Draw normal scene
      gameEnvironment.drawEnvironment();
      obstacles.forEach((o) => o.drawObstacle());
      fishes.forEach((f) => f.drawFish());

      players.forEach((p) => {
        p.drawPlayer();
        p.drawHook();

        fishes.forEach((f) => {
          const caughtFishId = p.hasHookedFish(
            f.id,
            f.positionX,
            f.positionY,
            f.radius
          );
          if (caughtFishId !== null) {
            connection
              ?.invoke("CatchFish", caughtFishId)
              .catch((err) => console.error("Failed to catch fish:", err));
          }
        });
      });

      // Draw DARK MASK LAST (water-only, with real punch-out)
      const me = playersRef.current?.[myConnectionId];
      const spotlight = me?.fishingRod?.cast
        ? { x: me.fishingRod.positionX, y: me.fishingRod.positionY }
        : null;

      gameEnvironment.drawDarkMask?.(spotlight);

      ctx.restore();
      requestAnimationFrame(draw);
    };

    draw();

    return () => {
      window.removeEventListener("keydown", handleKeyDown);
      window.removeEventListener("keyup", handleKeyUp);
    };
  }, [connection, gameEnvironmentData, myConnectionId]);

  return <canvas ref={canvasRef} />;
};

export default GameCanvas;
