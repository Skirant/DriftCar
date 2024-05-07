using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Various canvases for the UI
    public Transform gameCanvas;
    public Transform healthCanvas;
    public Transform pauseCanvas;
    public Transform gameOverCanvas;

    [Tooltip("������ �������������, ����������� �� ����� ������� ��� �� ��������")]
    public CarController playerObject;

    [Tooltip("�������� ������, ������� ��������� ������� ������ � ������� �������� ��������")]
    public Transform groundObject;

}