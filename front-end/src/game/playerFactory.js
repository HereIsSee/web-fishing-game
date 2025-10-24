const playerFactory = (canvasContext, playerData, myConnectionId) => {
  // console.log(playerData);
  let { connectionId, name, boat, fishingRod } = playerData;

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
    canvasContext.arc(hookX, hookY, 5, 0, Math.PI * 2);
    canvasContext.fill();
  };

  const hasHookedFish = (fishId, fishPositionX, fishPositionY) => {
    const hookX = playerData.fishingRod.positionX;
    const hookY = playerData.fishingRod.positionY;

    if (
      hookX - 5 < fishPositionX &&
      hookX + 5 > fishPositionX &&
      hookY - 5 < fishPositionY &&
      hookY + 5 > fishPositionY
    ) {
      return fishId;
    }
    return null;
  };

  return { drawPlayer, drawHook, hasHookedFish };
};

export default playerFactory;
