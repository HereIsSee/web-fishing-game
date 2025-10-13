function gameEnvironmentFactory(canvasContext, environmentData){

    const drawBackground = ()=>{
        canvasContext.fillStyle = '#7ec5cae8';

        canvasContext.beginPath();
        canvasContext.fillRect(
            0,
            0,
            environmentData.width,
            environmentData.height
        );
    }
    const drawWater = ()=>{
        canvasContext.fillStyle = '#3b67b8e8';

        canvasContext.beginPath();
        canvasContext.fillRect(
            0,
            0,
            environmentData.width,
            environmentData.waterHeight
        );
    }

    const drawEnvironment = () =>{
        drawBackground();
        drawWater();
    }


    return {drawEnvironment}
}

export default gameEnvironmentFactory;