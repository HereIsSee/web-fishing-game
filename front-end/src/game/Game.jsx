import React, { useEffect, useRef } from "react";
import * as PIXI from "pixi.js";

export default function Game({ connection }) {
  const containerRef = useRef(null);
  const playersRef = useRef(new Map());

  useEffect(() => {
    const app = new PIXI.Application();
    const keys = {};

    // PIXI v7 requires init() before use
    app.init({
      width: 800,
      height: 600,
      background: 0x87ceeb, // light sky-blue
    }).then(() => {
       if (containerRef.current && !containerRef.current.hasChildNodes()) {
        containerRef.current.appendChild(app.view);
      }

      // Ground
      const ground = new PIXI.Graphics();
      ground.beginFill(0x00008B, 0.3); // brown
      ground.drawRect(0, 500, 800, 100);
      ground.endFill();
      app.stage.addChild(ground);

      // Player
      const localPlayer  = new PIXI.Graphics();
      localPlayer.beginFill(0x8B4513);
      localPlayer.drawRect(0, 0, 80, 60);
      localPlayer.endFill();
      localPlayer.x = 100;
      localPlayer.y = 440;
      app.stage.addChild(localPlayer);
      playersRef.current.set("local", localPlayer);

      // Input handling
      const handleDown = (e) => {
        if (keys[e.key]) return; // Prevent duplicate calls
        
        keys[e.key] = true;
        
        if (e.key === "ArrowLeft" || e.key === "ArrowRight") {
          // Send movement immediately on key press
          const direction = e.key === "ArrowLeft" ? "left" : "right";
          connection.invoke("MoveBoat", direction).catch(err => {
            console.error("Failed to move boat:", err);
          });
        }
      };

      const handleUp = (e) => {
        keys[e.key] = false;
      };

      window.addEventListener("keydown", handleDown);
      window.addEventListener("keyup", handleUp);

      // LISTEN for other players' movements
      connection.on("BoatMovedTo", (playerId, positionX) => {
        console.log(`Player ${playerId} moved to ${positionX}`);
        
        // Create or update other player's boat
        if (!playersRef.current.has(playerId)) {
          const otherPlayer = new PIXI.Graphics();
          otherPlayer.beginFill(0xFF0000);
          otherPlayer.drawRect(0, 0, 80, 60);
          otherPlayer.endFill();
          otherPlayer.x = positionX; // ← USE THE RECEIVED POSITION
          otherPlayer.y = 440;
          app.stage.addChild(otherPlayer);
          playersRef.current.set(playerId, otherPlayer);
        }

        // Move the other player's boat to exact position
        const player = playersRef.current.get(playerId);
        player.x = positionX; 
      });
      
      // Game loop
      app.ticker.add(() => {
        if (keys["ArrowLeft"]) localPlayer.x -= 5;
        if (keys["ArrowRight"]) localPlayer.x += 5;

        // Send continuous position while key is held
        if (keys["ArrowLeft"] || keys["ArrowRight"]) {
          connection.invoke("MoveBoatTo", localPlayer.x).catch(console.error);
        }

        // Keep local player in bounds
        if (localPlayer.x < 0) localPlayer.x = 0;
        if (localPlayer.x + 40 > app.screen.width) {
          localPlayer.x = app.screen.width - 40;
        }
      });

      // Cleanup
      return () => {
        connection.off("BoatMovedTo");
        app.destroy(true, true);
        window.removeEventListener("keydown", handleDown);
        window.removeEventListener("keyup", handleUp);
      };
    });
  }, [connection]);

  return <div ref={containerRef} />;
}
