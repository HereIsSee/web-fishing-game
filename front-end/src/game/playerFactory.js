const playerFactory = (canvasContext, playerData, myConnectionId) =>{

    const drawPlayer = () =>{
        console.log(myConnectionId)
        // console.log(playerData.connectionId)
        
        playerData.connectionId === myConnectionId
            ? canvasContext.fillStyle = '#472020'
            : canvasContext.fillStyle = '#a10909ff'

        const x = playerData.boat.positionX;
        const y = playerData.boat.positionY;

        // console.log(x, y);

        canvasContext.beginPath();
        canvasContext.moveTo(x, y);
        canvasContext.lineTo(x-20, y);
        canvasContext.lineTo(x-30, y+20)
        canvasContext.lineTo(x+30, y+20);
        canvasContext.lineTo(x+20, y);

        canvasContext.fill();
    }

    return {drawPlayer};
}

export default playerFactory;