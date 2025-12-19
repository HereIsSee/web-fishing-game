const obstaclesFactory = (canvasContext, obstacleData)=>{

    const drawObstacle = () =>{
        canvasContext.fillStyle = "#90EE90"; // Light green
        canvasContext.lineWidth = 12;

        const x = obstacleData.PositionX || obstacleData.positionX;
        const y = obstacleData.PositionY || obstacleData.positionY;
        const height = obstacleData.Height || obstacleData.height;
        const width = obstacleData.Width || obstacleData.width;

        canvasContext.beginPath();
        canvasContext.moveTo(x,y);
        canvasContext.lineTo(x-width/2, y);
        canvasContext.lineTo(x-width/2, y+height);
        canvasContext.lineTo(x+width/2, y+height);
        canvasContext.lineTo(x+width/2, y);

        canvasContext.fill();
    }

    return {drawObstacle}
}

export default obstaclesFactory;