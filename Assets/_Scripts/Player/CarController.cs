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

    [Header("������� �����")]
    [Tooltip("���� �� ������� ����� ������������ ������")]
    public float maxAngle = 30f; // ������������� ������������ ����
    [Tooltip("���� ���� ������ ������")]
    public Transform leftWheelMesh, rightWheelMesh; // ��� �����

    [Header("������� ��� ����� ���� + ��������")]
    [Tooltip("���� ��� ������� ����� ��������� �������")]
    public float AngleDrift = 30;
    [Tooltip("�������� ��� ������� ����� ��������� �������")]
    public float SpeedDrift = 30;
    [Tooltip("���� ��������� ���")]
    public ParticleSystem[] effectPrefabs;
    [Tooltip("���� ��������� ������ �����")]
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

    //���������� � ����� ��������
    private void PhoneController()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float steerInputToch = touch.position.x < Screen.width / 2 ? -1 : 1;
            transform.Rotate(Vector3.up * steerInputToch * MoveForce.magnitude * StreerAngle * Time.deltaTime);
        }
    }

    //���������� � ����� ����������
    private void DeckTopController()
    {
        float steerInputKeys = Input.GetAxis("Horizontal");//DT
        transform.Rotate(Vector3.up * steerInputKeys * MoveForce.magnitude * StreerAngle * Time.deltaTime);
    }
}