const playerFactory = (canvasContext, playerData, myConnectionId) => {
  // console.log(playerData);
  let { connectionId, name, boat, fishingRod } = playerData;
  const hookRadius = 5;

  const drawPlayer = () => {
    playerData.connectionId === myConnectionId
      ? (canvasContext.fillStyle = "#472020")
      : (canvasContext.fillStyle = "#a10909ff");

    const x = playerData.boat.positionX;
    const y = playerData.boat.positionY;

    canvasContext.beginPath();
    canvasContext.moveTo(x, y);
    canvasContext.lineTo(x - 20, y);
    canvasContext.lineTo(x - 30, y + 20);
    canvasContext.lineTo(x + 30, y + 20);
    canvasContext.lineTo(x + 20, y);
    canvasContext.fill();

    try {
      if (playerData.connectionId === myConnectionId) {
        canvasContext.save();
        canvasContext.strokeStyle = "#FFD700"; 
        canvasContext.lineWidth = 3;
        canvasContext.strokeRect(x - 35, y - 5, 70, 30);
        canvasContext.restore();
      }
    } catch (e) {}

    try {
      const boatHeight = 20; 
      const nameX = x; 
      const nameGameY = y + boatHeight + 6; 
      const pixelX = nameX;
      const pixelY = canvasContext.canvas.height - nameGameY - 8;

      canvasContext.save();
      canvasContext.setTransform(1, 0, 0, 1, 0, 0);
      canvasContext.fillStyle = "#000000";
      canvasContext.font = "14px sans-serif";
      canvasContext.textAlign = "center";
      canvasContext.fillText(name || "Unknown", pixelX, pixelY);
      canvasContext.restore();
    } catch (e) {
    }
  };

  const drawHook = () => {
    if (!playerData.fishingRod?.cast) return;
    const hookX = playerData.fishingRod.positionX;
    const hookY = playerData.fishingRod.positionY;

    canvasContext.strokeStyle = "#000000"; 
    canvasContext.lineWidth = 2;

    const boatX = playerData.boat.positionX;
    const boatY = playerData.boat.positionY;
    canvasContext.beginPath();
    canvasContext.moveTo(boatX, boatY);
    canvasContext.lineTo(hookX, hookY);
    canvasContext.stroke();

    canvasContext.fillStyle = "#ff0000"; 
    canvasContext.beginPath();
    canvasContext.arc(hookX, hookY, hookRadius, 0, Math.PI * 2);
    canvasContext.fill();
  };

  const hasHookedFish = (fishId, fishX, fishY, fishRadius) => {
    if (!playerData.fishingRod?.cast) return null;

    const hookX = playerData.fishingRod.positionX;
    const hookY = playerData.fishingRod.positionY;

    const dx = hookX - fishX;
    const dy = hookY - fishY;
    const distance = Math.sqrt(dx * dx + dy * dy);

    if (distance <= hookRadius + fishRadius) {
      return fishId;
    }

    return null;
  };


  const drawControlArrow = () => {
    if (playerData.connectionId !== myConnectionId) return;
    try {
      const x = playerData.boat.positionX;
      const y = playerData.boat.positionY;
      const height = 20;
      const pixelX = x;
      const pixelY = canvasContext.canvas.height - (y + height + 20);
      canvasContext.save();
      canvasContext.setTransform(1, 0, 0, 1, 0, 0);
      canvasContext.fillStyle = "#000000";
      canvasContext.beginPath();
      canvasContext.moveTo(pixelX, pixelY);
      canvasContext.lineTo(pixelX - 8, pixelY + 12);
      canvasContext.lineTo(pixelX + 8, pixelY + 12);
      canvasContext.closePath();
      canvasContext.fill();
      canvasContext.restore();
    } catch (e) {}
  };

  return { drawPlayer, drawHook, hasHookedFish, drawControlArrow };
};

export default playerFactory;
