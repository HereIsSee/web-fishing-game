const fishFactory = (canvasContext, fishData) => {
  let { id, positionX, positionY, radius, movementSpeed, hasBeenHooked } =
    fishData;
  //console.log(fishData);
  const drawFish = () => {
    canvasContext.fillStyle = fishData.color;

    const x = fishData.positionX;
    const y = fishData.positionY;

    canvasContext.beginPath();
    canvasContext.arc(x, y, fishData.radius, 0, 2 * Math.PI);
    canvasContext.fill();
  };

  return { id, positionX, positionY, radius, drawFish };
};

export default fishFactory;
