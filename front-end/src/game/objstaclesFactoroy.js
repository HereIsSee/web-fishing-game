const obstaclesFactory = (canvasContext, obstacleData)=>{

    const drawObstacle = () =>{
        canvasContext.fillStyle = "#55974cff";
        canvasContext.lineWidth = 12;

        const x = obstacleData.PositionX;
        const y = obstacleData.PositionY;
        const height = obstacleData.height;
        const width = obstacleData.width;

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