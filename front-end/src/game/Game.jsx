import React, { useEffect, useRef } from "react";
import * as PIXI from "pixi.js";

export default function Game() {
  const containerRef = useRef(null);

  useEffect(() => {
    const app = new PIXI.Application();

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
      const player = new PIXI.Graphics();
      player.beginFill(0x8B4513);
      player.drawRect(0, 0, 80, 60);
      player.endFill();
      player.x = 100;
      player.y = 440;
      app.stage.addChild(player);

      // Input handling
      const keys = {};
      const handleDown = (e) => (keys[e.key] = true);
      const handleUp = (e) => (keys[e.key] = false);
      window.addEventListener("keydown", handleDown);
      window.addEventListener("keyup", handleUp);

      // Game loop
      app.ticker.add(() => {
        if (keys["ArrowLeft"]) player.x -= 5;
        if (keys["ArrowRight"]) player.x += 5;

        if (player.x < 0) player.x = 0;
        if (player.x + 40 > app.screen.width) {
          player.x = app.screen.width - 40;
        }
      });

      // Cleanup
      return () => {
        app.destroy(true, true);
        window.removeEventListener("keydown", handleDown);
        window.removeEventListener("keyup", handleUp);
      };
    });
  }, []);

  return <div ref={containerRef} />;
}
