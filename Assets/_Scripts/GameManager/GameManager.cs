using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Various canvases for the UI
    public Transform gameCanvas;
    public Transform healthCanvas;
    public Transform pauseCanvas;
    public Transform gameOverCanvas;

    [Tooltip("Объект проигрывателя, назначенный из папки проекта или из магазина")]
    public CarController playerObject;

    [Tooltip("Наземный объект, который повторяет позицию игрока и создает ощущение движения")]
    public Transform groundObject;

}