const fishFactory = (canvasContext, fishData) =>{
    const drawFish = ()=>{
        canvasContext.fillStyle = '#d9e25de8';

        const x = fishData.PositionX;
        const y = fishData.PositionY;

        canvasContext.beginPath();
        canvasContext.arc(x,y,6,0,2*Math.PI);
        canvasContext.fill();
    }

    return {drawFish};
}

export default fishFactory;