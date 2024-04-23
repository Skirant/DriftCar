using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CarController : MonoBehaviour
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
    public Transform leftWheelMesh, rightWheelMesh; // Меш колес

    [Header("Дым из под колес Угол + Скорость")]
    [Tooltip("Угол при котором будет появлятся дым")]
    public float AngleDrift = 30;
    [Tooltip("Скорость при котором будет появлятся дым")]
    public float SpeedDrift = 30;
    public ParticleSystem[] effectPrefabs;

    private void Update()
    {
        MoveForce += transform.forward * MoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += MoveForce * Time.deltaTime;

        float steerInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerInput * MoveForce.magnitude * StreerAngle * Time.deltaTime);

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
}