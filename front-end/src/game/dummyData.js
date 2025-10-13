const playersData = [
    {
        PositionX: 500,
        PositionY: 500,
    },
    {
        PositionX: 350,
        PositionY: 500,
    },
];

const fishesData = [
    {
        PositionX: 350,
        PositionY:200,
    },
    {
        PositionX: 230,
        PositionY:230,
    },
    {
        PositionX: 150,
        PositionY: 400,
    },
    {
        PositionX: 150,
        PositionY: 490,
    },
]
const obstaclesData = [
    {
        PositionX: 500,
        PositionY: 0,
        height: 300,
        width: 50,
    },
    {
        PositionX: 300,
        PositionY: 0,
        height: 200,
        width: 30,
    }
];
const gameEnvironmentData = {
    width: 800,
    height: 600,
    waterHeight: 500,
};

export { playersData, fishesData, gameEnvironmentData, obstaclesData}