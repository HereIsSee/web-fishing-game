function gameEnvironmentFactory(canvasContext, environmentData) {
  console.log(environmentData);
  const drawBackground = () => {
    canvasContext.fillStyle = environmentData.skyColor ?? "#7ec5cae8";
    canvasContext.fillRect(0, 0, environmentData.width, environmentData.height);
  };

  const drawWater = () => {
    canvasContext.fillStyle = environmentData.waterColor ?? "#3b67b8e8";
    canvasContext.fillRect(
      0,
      0,
      environmentData.width,
      environmentData.waterHeight
    );
  };

  const drawHazardZones = () => {
    const zones = environmentData.hazardZones ?? [];
    if (!zones.length) return;

    canvasContext.save();

    canvasContext.globalAlpha = 0.22;
    canvasContext.fillStyle = "#ff3b30";

    for (const z of zones) {
      canvasContext.beginPath();
      canvasContext.arc(z.x, z.y, z.radius, 0, Math.PI * 2);
      canvasContext.fill();
    }

    canvasContext.globalAlpha = 0.75;
    canvasContext.strokeStyle = "#ffffff";
    canvasContext.lineWidth = 2;

    for (const z of zones) {
      canvasContext.beginPath();
      canvasContext.arc(z.x, z.y, z.radius, 0, Math.PI * 2);
      canvasContext.stroke();
    }

    canvasContext.restore();
  };

  const drawEnvironment = () => {
    drawBackground();
    drawWater();
    drawHazardZones();
  };

  // ✅ FIXED: always darken water, punch-out only if spotlight exists
  const drawDarkMask = (spotlight) => {
    const dark = environmentData.darkWater;
    if (!dark?.enabled) return;

    const r = dark.visibleRadius ?? 120;

    canvasContext.save();
    canvasContext.beginPath();

    // Water rectangle
    canvasContext.rect(
      0,
      0,
      environmentData.width,
      environmentData.waterHeight
    );

    // Punch-out ONLY if hook exists
    if (
      spotlight &&
      typeof spotlight.x === "number" &&
      typeof spotlight.y === "number"
    ) {
      canvasContext.arc(
        spotlight.x,
        spotlight.y,
        r,
        0,
        Math.PI * 2,
        true // reverse direction → subtract
      );
    }

    canvasContext.clip();

    // Darken water
    canvasContext.fillStyle = "#000000";
    canvasContext.fillRect(
      0,
      0,
      environmentData.width,
      environmentData.waterHeight
    );

    canvasContext.restore();
  };

  return { drawEnvironment, drawDarkMask };
}

export default gameEnvironmentFactory;
