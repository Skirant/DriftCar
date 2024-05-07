using System;
using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    internal Vector3 targetPosition;
    internal Transform thisTransform;
    internal float chaseAngle;
    internal RaycastHit groundHitInfo;

    public Transform player; // ссылка на игрока

    [Header("Эффекты при управлении")]
    [Tooltip("Ускорение")]
    public float MoveSpeed = 50;
    [Tooltip("Максимальная скорость")]
    public float MaxSpeed = 15;
    [Tooltip("Замедении -1 нет замедления -0 нет скорости")]
    public float Drag = 0.98f;
    [Tooltip("Угол поворота машины")]
    public float StreerAngle = 20;
    internal float currentRotation = 0;
    [Tooltip("Сила заноса чем выше тем хуже машина входит в занос")]
    public float Traction = 1;

    private float speed;

    private Vector3 MoveForce;

    /*[Header("Эффекты при управлении")]
    [Tooltip("Эффект, который появляется, когда эту машину ударяет другая машина")]
    public Transform hitEffect;*/

    [Header("Поворот колес")]
    //[Tooltip("Угол на который будет поворачивать колесо")]
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

    [Header("Настройки ИИ")]
    [Tooltip("Это Бот")]
    public bool thisIsBot = true;
    [Tooltip("Прямое расстояние области обнаружения препятствий для этой машины AI")]
    public float detectDistance = 3;
    [Tooltip("Ширина области обнаружения препятствий для этой машины AI")]
    public float detectAngle = 2;
    [Tooltip("Сделать AI машинами стараются избегать препятствий. Препятствиями являются объекты, к которым прикреплен компонент ECCObstacle")]
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
        // Перемещаем игрока вперед

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

    private void BotController()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        //Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);

        //transform.rotation = Quaternion.Slerp(transform.rotation, rotationToPlayer, StreerAngle * Time.deltaTime);

        // Вычисляем угол поворота колеса на основе направления к игроку
        float wheelTurnAngle = Vector3.SignedAngle(transform.forward, directionToPlayer, transform.up) * 4;

        // Поворачиваем колеса бота
        TernWheelsBot(wheelTurnAngle);

        float sphereRadius = 0.5f; // Размер сферы для SphereCast

        Ray rayRight = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * detectAngle * 0.5f + transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);
        Ray rayLeft = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * -detectAngle * 0.5f - transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);

        // Создаем сферические лучи вправо и влево
        RaycastHit hitRight;
        RaycastHit hitLeft;

        bool obstacleDetectedRight = Physics.SphereCast(rayRight, sphereRadius, out hitRight, detectDistance);
        bool obstacleDetectedLeft = Physics.SphereCast(rayLeft, sphereRadius, out hitLeft, detectDistance);

        // Если обнаружено препятствие справа, сворачиваем влево
        if (avoidObstacles && obstacleDetectedRight && IsObstacle(hitRight))
        {
            Rotate(-1);
        }
        // Если обнаружено препятствие слева, сворачиваем вправо
        else if (avoidObstacles && obstacleDetectedLeft && IsObstacle(hitLeft))
        {
            Rotate(1);
        }
        // Если нет обнаруженных препятствий, продолжаем движение вперед
        else
        {
            // Поворачиваем автомобиль, пока он не достигнет желаемого угла погони с любой стороны игрока
            if (Vector3.Angle(thisTransform.forward, targetPosition - thisTransform.position) > chaseAngle)
            {
                Rotate(ChaseAngle(thisTransform.forward, targetPosition - thisTransform.position, Vector3.up));
                //print(" Rotate(ChaseAngle(thisTransform.forward, targetPosition - thisTransform.position, Vector3.up))");
            }
            else // В противном случае, прекращаем поворот
            {
                Rotate(0);
                //print("Rotate(0)");
            }
        }

        bool IsObstacle(RaycastHit hit)
        {
            // Добавьте здесь любую дополнительную проверку для определения, является ли попавший объект препятствием
            // Например, вы можете проверить тег или тип компонента объекта
            return hit.transform.GetComponent<Obstacle>() || (hit.transform.GetComponent<CarController>() && GameObject.FindWithTag("Player") != this);
        }
    }

    public void Rotate(float rotateDirection)
    {
        // Если автомобиль поворачивается влево или вправо, заставьте его заносить и наклоняться в направлении его вращения
        if (rotateDirection != 0)
        {
            // Поворачиваем автомобиль на основе направления управления
            transform.localEulerAngles += Vector3.up * rotateDirection * StreerAngle * 10 * Time.deltaTime;

            // Увеличиваем текущую вращение
            currentRotation += rotateDirection * StreerAngle * 10 * Time.deltaTime;

            // Отобразим вектор направления вращения
            Debug.DrawRay(transform.position, transform.up * rotateDirection * 5, Color.red);

            // Отобразим вектор увеличения текущей вращения
            Debug.DrawRay(transform.position, transform.up * rotateDirection * StreerAngle * 10 * Time.deltaTime, Color.blue);

            // Если текущее вращение больше 360, вычитаем 360
            if (currentRotation > 360) currentRotation -= 360;

            /*// Заставляем основание автомобиля заносить на основе угла вращения
            thisTransform.Find("Base").localEulerAngles = new Vector3(rightAngle, Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle + Mathf.Sin(Time.time * 50) * hurtDelayCount * 50, Time.deltaTime), 0);*/

            /*// Заставляем шасси наклоняться в стороны на основе угла вращения
            if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, rotateDirection * leanAngle, Time.deltaTime);*/

            /*// Воспроизводим анимацию заноса. В этой анимации вы можете вызывать все виды эффектов, такие как пыль, следы от заноса и т. д.
            GetComponent<Animator>().Play("Skid");*/

            /*// Проходим через все колеса, заставляя их вращаться, и заставляем передние колеса поворачиваться вбок на основе вращения
            for (index = 0; index < wheels.Length; index++)
            {
                // Поворачиваем передние колеса вбок на основе вращения
                if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime * 10);

                // Вращаем колесо
                wheels[index].Find("WheelObject").Rotate(Vector3.right * Time.deltaTime * speed * 20, Space.Self);
            }*/
        }
        else // В противном случае, если мы больше не вращаемся, выпрямляем автомобиль и передние колеса
        {
            // Возвращаем основание автомобиля к его углу 0
            thisTransform.Find("Base").localEulerAngles = Vector3.up * Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, 0, Time.deltaTime * 5);

            /*// Возвращаем шасси к его углу 0
            if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, 0, Time.deltaTime * 5);*/

            /*// Воспроизводим анимацию движения. В этой анимации мы останавливаем все ранее вызванные эффекты, такие как пыль, следы от заноса и т. д
            GetComponent<Animator>().Play("Move");

            // Проходим через все колеса, заставляя их вращаться быстрее, чем при повороте, и возвращаем передние колеса к их углу 0
            for (index = 0; index < wheels.Length; index++)
            {
                // Возвращаем передние колеса к их углу 0
                if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, 0, Time.deltaTime * 5);

                // Вращаем колесо быстрее
                wheels[index].Find("WheelObject").Rotate(Vector3.right * Time.deltaTime * speed * 30, Space.Self);
            }*/
        }
    }

    public float ChaseAngle(Vector3 forward, Vector3 targetDirection, Vector3 up)
    {
        // Вычисляем угол подхода
        float approachAngle = Vector3.Dot(Vector3.Cross(up, forward), targetDirection);

        // Если угол больше 0, мы подходим с левой стороны (поэтому мы должны повернуть вправо)
        if (approachAngle > 0f)
        {
            return 1f;
        }
        else if (approachAngle < 0f) // В противном случае, если угол меньше 0, мы подходим с правой стороны (поэтому мы должны повернуть влево)
        {
            return -1f;
        }
        else // В противном случае, мы находимся в пределах углового диапазона, поэтому нам не нужно вращаться
        {
            return 0f;
        }
    }

    private void TernWheelsBot(float angle)
    {
        // Примените угол к вращению колес
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