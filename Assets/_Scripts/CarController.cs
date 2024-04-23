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

    [Header("������� �����")]
    [Tooltip("���� �� ������� ����� ������������ ������")]
    public float maxAngle = 30f; // ������������� ������������ ����
    public Transform leftWheelMesh, rightWheelMesh; // ��� �����

    [Header("��� �� ��� ����� ���� + ��������")]
    [Tooltip("���� ��� ������� ����� ��������� ���")]
    public float AngleDrift = 30;
    [Tooltip("�������� ��� ������� ����� ��������� ���")]
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

    //����������� ���������� ����
    private void DragCar()
    {
        MoveForce *= Drag;        
    }

    //MoveForce - ����� ����
    private void DriftCar()
    {
        Debug.DrawRay(transform.position, MoveForce.normalized * 3);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        MoveForce = Vector3.Lerp(MoveForce.normalized, transform.forward, Traction * Time.deltaTime) * MoveForce.magnitude;
    }

    //�������� ���� ��� ������(!!!������ ��� � ���� �� ��������� ������� ��� ������ ���� ��������� ����!!!)
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

    //������� �����
    private void TernWheels()
    {
        float angle = maxAngle * Input.GetAxis("Horizontal"); // ���� ���� ��������

        // ��������� ���� � �������� �����
        leftWheelMesh.localRotation = Quaternion.Euler(0, angle, 0);
        rightWheelMesh.localRotation = Quaternion.Euler(0, angle, 0);

        ApplyEffectIfAngleIsGreaterThan30(leftWheelMesh);
        ApplyEffectIfAngleIsGreaterThan30(rightWheelMesh);
    }
}