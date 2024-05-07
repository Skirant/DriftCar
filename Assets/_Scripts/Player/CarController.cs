using System;
using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    internal Vector3 targetPosition;
    internal Transform thisTransform;
    internal float chaseAngle;
    internal RaycastHit groundHitInfo;

    public Transform player; // ������ �� ������

    [Header("������� ��� ����������")]
    [Tooltip("���������")]
    public float MoveSpeed = 50;
    [Tooltip("������������ ��������")]
    public float MaxSpeed = 15;
    [Tooltip("��������� -1 ��� ���������� -0 ��� ��������")]
    public float Drag = 0.98f;
    [Tooltip("���� �������� ������")]
    public float StreerAngle = 20;
    internal float currentRotation = 0;
    [Tooltip("���� ������ ��� ���� ��� ���� ������ ������ � �����")]
    public float Traction = 1;

    private float speed;

    private Vector3 MoveForce;

    /*[Header("������� ��� ����������")]
    [Tooltip("������, ������� ����������, ����� ��� ������ ������� ������ ������")]
    public Transform hitEffect;*/

    [Header("������� �����")]
    //[Tooltip("���� �� ������� ����� ������������ ������")]
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

    [Header("��������� ��")]
    [Tooltip("��� ���")]
    public bool thisIsBot = true;
    [Tooltip("������ ���������� ������� ����������� ����������� ��� ���� ������ AI")]
    public float detectDistance = 3;
    [Tooltip("������ ������� ����������� ����������� ��� ���� ������ AI")]
    public float detectAngle = 2;
    [Tooltip("������� AI �������� ��������� �������� �����������. ������������� �������� �������, � ������� ���������� ��������� ECCObstacle")]
    public bool avoidObstacles = true;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        thisTransform = this.transform;
    }

    private void Update()
    {
        MoveForce += transform.forward * MoveSpeed * Time.deltaTime;
        transform.position += MoveForce * Time.deltaTime;
        // ���������� ������ ������

        if (thisIsBot != true)
        {
            PhoneController();

            DeckTopController();
        }


        MoveForce = Vector3.ClampMagnitude(MoveForce, MaxSpeed);

        speed = MoveForce.magnitude;

        DragCar();

        DriftCar();

        //print(Vector3.Angle(MoveForce.normalized, transform.forward));

        TernWheels();
        if (thisIsBot)
        {
            BotController();
        }

        Destroy();

        targetPosition = player.transform.position;
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

    private void BotController()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        //Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);

        //transform.rotation = Quaternion.Slerp(transform.rotation, rotationToPlayer, StreerAngle * Time.deltaTime);

        // ��������� ���� �������� ������ �� ������ ����������� � ������
        float wheelTurnAngle = Vector3.SignedAngle(transform.forward, directionToPlayer, transform.up) * 4;

        // ������������ ������ ����
        TernWheelsBot(wheelTurnAngle);

        float sphereRadius = 0.5f; // ������ ����� ��� SphereCast

        Ray rayRight = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * detectAngle * 0.5f + transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);
        Ray rayLeft = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * -detectAngle * 0.5f - transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);

        // ������� ����������� ���� ������ � �����
        RaycastHit hitRight;
        RaycastHit hitLeft;

        bool obstacleDetectedRight = Physics.SphereCast(rayRight, sphereRadius, out hitRight, detectDistance);
        bool obstacleDetectedLeft = Physics.SphereCast(rayLeft, sphereRadius, out hitLeft, detectDistance);

        // ���� ���������� ����������� ������, ����������� �����
        if (avoidObstacles && obstacleDetectedRight && IsObstacle(hitRight))
        {
            Rotate(-1);
        }
        // ���� ���������� ����������� �����, ����������� ������
        else if (avoidObstacles && obstacleDetectedLeft && IsObstacle(hitLeft))
        {
            Rotate(1);
        }
        // ���� ��� ������������ �����������, ���������� �������� ������
        else
        {
            // ������������ ����������, ���� �� �� ��������� ��������� ���� ������ � ����� ������� ������
            if (Vector3.Angle(thisTransform.forward, targetPosition - thisTransform.position) > chaseAngle)
            {
                Rotate(ChaseAngle(thisTransform.forward, targetPosition - thisTransform.position, Vector3.up));
                //print(" Rotate(ChaseAngle(thisTransform.forward, targetPosition - thisTransform.position, Vector3.up))");
            }
            else // � ��������� ������, ���������� �������
            {
                Rotate(0);
                //print("Rotate(0)");
            }
        }

        bool IsObstacle(RaycastHit hit)
        {
            // �������� ����� ����� �������������� �������� ��� �����������, �������� �� �������� ������ ������������
            // ��������, �� ������ ��������� ��� ��� ��� ���������� �������
            return hit.transform.GetComponent<Obstacle>() || (hit.transform.GetComponent<CarController>() && GameObject.FindWithTag("Player") != this);
        }
    }

    public void Rotate(float rotateDirection)
    {
        // ���� ���������� �������������� ����� ��� ������, ��������� ��� �������� � ����������� � ����������� ��� ��������
        if (rotateDirection != 0)
        {
            // ������������ ���������� �� ������ ����������� ����������
            transform.localEulerAngles += Vector3.up * rotateDirection * StreerAngle * 10 * Time.deltaTime;

            // ����������� ������� ��������
            currentRotation += rotateDirection * StreerAngle * 10 * Time.deltaTime;

            // ��������� ������ ����������� ��������
            Debug.DrawRay(transform.position, transform.up * rotateDirection * 5, Color.red);

            // ��������� ������ ���������� ������� ��������
            Debug.DrawRay(transform.position, transform.up * rotateDirection * StreerAngle * 10 * Time.deltaTime, Color.blue);

            // ���� ������� �������� ������ 360, �������� 360
            if (currentRotation > 360) currentRotation -= 360;

            /*// ���������� ��������� ���������� �������� �� ������ ���� ��������
            thisTransform.Find("Base").localEulerAngles = new Vector3(rightAngle, Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle + Mathf.Sin(Time.time * 50) * hurtDelayCount * 50, Time.deltaTime), 0);*/

            /*// ���������� ����� ����������� � ������� �� ������ ���� ��������
            if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, rotateDirection * leanAngle, Time.deltaTime);*/

            /*// ������������� �������� ������. � ���� �������� �� ������ �������� ��� ���� ��������, ����� ��� ����, ����� �� ������ � �. �.
            GetComponent<Animator>().Play("Skid");*/

            /*// �������� ����� ��� ������, ��������� �� ���������, � ���������� �������� ������ �������������� ���� �� ������ ��������
            for (index = 0; index < wheels.Length; index++)
            {
                // ������������ �������� ������ ���� �� ������ ��������
                if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime * 10);

                // ������� ������
                wheels[index].Find("WheelObject").Rotate(Vector3.right * Time.deltaTime * speed * 20, Space.Self);
            }*/
        }
        else // � ��������� ������, ���� �� ������ �� ���������, ���������� ���������� � �������� ������
        {
            // ���������� ��������� ���������� � ��� ���� 0
            thisTransform.Find("Base").localEulerAngles = Vector3.up * Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, 0, Time.deltaTime * 5);

            /*// ���������� ����� � ��� ���� 0
            if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, 0, Time.deltaTime * 5);*/

            /*// ������������� �������� ��������. � ���� �������� �� ������������� ��� ����� ��������� �������, ����� ��� ����, ����� �� ������ � �. �
            GetComponent<Animator>().Play("Move");

            // �������� ����� ��� ������, ��������� �� ��������� �������, ��� ��� ��������, � ���������� �������� ������ � �� ���� 0
            for (index = 0; index < wheels.Length; index++)
            {
                // ���������� �������� ������ � �� ���� 0
                if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, 0, Time.deltaTime * 5);

                // ������� ������ �������
                wheels[index].Find("WheelObject").Rotate(Vector3.right * Time.deltaTime * speed * 30, Space.Self);
            }*/
        }
    }

    public float ChaseAngle(Vector3 forward, Vector3 targetDirection, Vector3 up)
    {
        // ��������� ���� �������
        float approachAngle = Vector3.Dot(Vector3.Cross(up, forward), targetDirection);

        // ���� ���� ������ 0, �� �������� � ����� ������� (������� �� ������ ��������� ������)
        if (approachAngle > 0f)
        {
            return 1f;
        }
        else if (approachAngle < 0f) // � ��������� ������, ���� ���� ������ 0, �� �������� � ������ ������� (������� �� ������ ��������� �����)
        {
            return -1f;
        }
        else // � ��������� ������, �� ��������� � �������� �������� ���������, ������� ��� �� ����� ���������
        {
            return 0f;
        }
    }

    private void TernWheelsBot(float angle)
    {
        // ��������� ���� � �������� �����
        leftWheelMesh.localRotation = Quaternion.Euler(0, angle, 0);
        rightWheelMesh.localRotation = Quaternion.Euler(0, angle, 0);

        ApplyEffectIfAngleIsGreaterThan30(leftWheelMesh);
        ApplyEffectIfAngleIsGreaterThan30(rightWheelMesh);
    }

    private void Destroy()
    {
        if (thisIsBot)
        {
            //Destroy(gameObject, 10);
        }
    }
}