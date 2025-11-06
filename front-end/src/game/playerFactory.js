const playerFactory = (canvasContext, playerData, myConnectionId) => {
  // console.log(playerData);
  let { connectionId, name, boat, fishingRod } = playerData;
  const hookRadius = 5;

  const drawPlayer = () => {
    // console.log(myConnectionId)
    // console.log(playerData.connectionId)

    playerData.connectionId === myConnectionId
      ? (canvasContext.fillStyle = "#472020")
      : (canvasContext.fillStyle = "#a10909ff");

    const x = playerData.boat.positionX;
    const y = playerData.boat.positionY;

    // console.log(x, y);

    canvasContext.beginPath();
    canvasContext.moveTo(x, y);
    canvasContext.lineTo(x - 20, y);
    canvasContext.lineTo(x - 30, y + 20);
    canvasContext.lineTo(x + 30, y + 20);
    canvasContext.lineTo(x + 20, y);

    canvasContext.fill();
  };

  const drawHook = () => {
    if (!playerData.fishingRod.cast) return;
    const hookX = playerData.fishingRod.positionX;
    const hookY = playerData.fishingRod.positionY;

    canvasContext.strokeStyle = "#000000"; // line color for the fishing line
    canvasContext.lineWidth = 2;

    // Draw line from boat to hook
    canvasContext.beginPath();
    canvasContext.moveTo(playerData.boat.positionX, playerData.boat.positionY);
    canvasContext.lineTo(hookX, hookY);
    canvasContext.stroke();

    // Draw the hook itself
    canvasContext.fillStyle = "#ff0000"; // red hook
    canvasContext.beginPath();
    canvasContext.arc(hookX, hookY, hookRadius, 0, Math.PI * 2);
    canvasContext.fill();
  };

  const hasHookedFish = (fishId, fishX, fishY, fishRadius) => {
    if (!playerData.fishingRod.cast) return null;

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

  return { drawPlayer, drawHook, hasHookedFish };
};

export default playerFactory;
