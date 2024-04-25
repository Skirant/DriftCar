using UnityEngine;

public class CarController: MonoBehaviour
{
    public float MoveSpeed = 50;
    public float MaxSpeed = 15;
    public float Drag = 0.98f;
    public float StreerAngle = 20;
    public float Traction = 1;

    private float speed;

    private Vector3 MoveForce;

    [Header("Поворот колес")]
    [Tooltip("Угол на который будет поворачивать колесо")]
    public float maxAngle = 30f; // Настраиваемый максимальный угол
    [Tooltip("Сюда надо заднии колеса")]
    public Transform leftWheelMesh, rightWheelMesh; // Меш колес

    [Header("Эффекты под колес Угол + Скорость")]
    [Tooltip("Угол при котором будет появлятся Эффекты")]
    public float AngleDrift = 30;
    [Tooltip("Скорость при котором будет появлятся Эффекты")]
    public float SpeedDrift = 30;
    [Tooltip("Сюда добавляем дым")]
    public ParticleSystem[] effectPrefabs;
    [Tooltip("Сюда добавляем черные линии")]
    public GameObject[] BlackLines;

    private void Update()
    {
        MoveForce += transform.forward * MoveSpeed * Time.deltaTime;
        transform.position += MoveForce * Time.deltaTime;

        PhoneController();

        DeckTopController();

        MoveForce = Vector3.ClampMagnitude(MoveForce, MaxSpeed);

        speed = MoveForce.magnitude;

        DragCar();

        DriftCar();

        //print(Vector3.Angle(MoveForce.normalized, transform.forward));

        TernWheels();       
    }

    //Постепенное замедление авто
    private void DragCar()
    {
        MoveForce *= Drag;
    }

    //MoveForce - дрииф авто
    private void DriftCar()
    {
        Debug.DrawRay(transform.position, MoveForce.normalized * 3);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        MoveForce = Vector3.Lerp(MoveForce.normalized, transform.forward, Traction * Time.deltaTime) * MoveForce.magnitude;
    }

    //Создание дыма при дрифте(!!!Задний ход в игре не предвиден поэтому при заднем ходе возникают баги!!!)
    private void ApplyEffectIfAngleIsGreaterThan30(Transform wheelMesh)
    {
        float angle = Vector3.Angle(MoveForce.normalized, wheelMesh.transform.forward);

        if (angle > AngleDrift & speed >= SpeedDrift)
        {
            foreach (ParticleSystem effectPrefab in effectPrefabs)
            {
                effectPrefab.Emit(1);
            }

            foreach (GameObject blackLine in BlackLines)
            {
                blackLine.GetComponentInChildren<TrailRenderer>().emitting = true;
            }
        }
        else
        {
            foreach (GameObject blackLine in BlackLines)
            {
                blackLine.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }

    //Поворот колес
    private void TernWheels()
    {
        float angle = maxAngle * Input.GetAxis("Horizontal"); // Ввод угла поворота

        // Примените угол к вращению колес
        leftWheelMesh.localRotation = Quaternion.Euler(0, angle, 0);
        rightWheelMesh.localRotation = Quaternion.Euler(0, angle, 0);

        ApplyEffectIfAngleIsGreaterThan30(leftWheelMesh);
        ApplyEffectIfAngleIsGreaterThan30(rightWheelMesh);
    }

    //Управление с пошью телефона
    private void PhoneController()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float steerInputToch = touch.position.x < Screen.width / 2 ? -1 : 1;
            transform.Rotate(Vector3.up * steerInputToch * MoveForce.magnitude * StreerAngle * Time.deltaTime);
        }
    }

    //Управление с пошью клавиатуры
    private void DeckTopController()
    {
        float steerInputKeys = Input.GetAxis("Horizontal");//DT
        transform.Rotate(Vector3.up * steerInputKeys * MoveForce.magnitude * StreerAngle * Time.deltaTime);
    }
}