const fishFactory = (canvasContext, fishData) => {
  let { id, positionX, positionY, movementSpeed, hasBeenHooked } = fishData;

  const drawFish = () => {
    canvasContext.fillStyle = "#d9e25de8";

    const x = fishData.positionX;
    const y = fishData.positionY;

    canvasContext.beginPath();
    canvasContext.arc(x, y, 6, 0, 2 * Math.PI);
    canvasContext.fill();
  };

  return { id, positionX, positionY, drawFish };
};

export default fishFactory;
