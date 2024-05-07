using UnityEngine;

public class SpawnAroundObject : MonoBehaviour
{
    static CreationEarth creationEarth;

    [Tooltip("Тег объекта, вокруг которого создаются другие объекты в ограниченном диапазоне")]
    public string spawnAroundTag = "Player";
    internal Transform spawnAroundObject;

    [Tooltip("Переключатель, который включает и выключает создание. Если True, мы сейчас создаем объекты")]
    public bool isSpawning = false;

    [System.Serializable]
    public class SpawnGroup
    {
        [Tooltip("Список всех объектов, которые будут созданы")]
        public Transform[] spawnObjects;

        [Tooltip("Темп создания объектов в секундах.")]
        public float spawnRate = 5;
        internal float spawnRateCount = 0;
        internal int spawnIndex = 0;

        [Tooltip("Расстояние, на котором этот объект создается относительно spawnAroundObject")]
        public Vector2 spawnObjectDistance = new Vector2(10, 20);
    }

    [Tooltip("Массив групп создания. Это могут быть, например, вражеские автомобили, предметы для сбора или препятствия в виде камней")]
    public SpawnGroup[] spawnGroups;

    internal int index;

    private void Start()
    {
        // Сохраняем некоторые переменные для удобного доступа
        if (creationEarth == null) creationEarth = GameObject.FindObjectOfType<CreationEarth>();
    }

    /// <summary>
    /// Update вызывается каждый кадр, если MonoBehaviour включен.
    /// </summary>
    void Update()
    {
        // Если у нас нет объекта для создания вокруг, ищем его на сцене и назначаем
        if (spawnAroundObject == null && spawnAroundTag != string.Empty && GameObject.FindGameObjectWithTag(spawnAroundTag)) spawnAroundObject = GameObject.FindGameObjectWithTag(spawnAroundTag).transform;

        // Если мы не создаем, то не продолжаем
        if (isSpawning == false) return;

        // Проходим через все группы создания, считаем вниз и создаем объекты
        for (index = 0; index < spawnGroups.Length; index++)
        {
            // Если есть объекты для создания, продолжаем
            if (spawnGroups[index].spawnObjects.Length > 0)
            {
                // Считаем до следующего создания объекта
                if (spawnGroups[index].spawnRateCount > 0) spawnGroups[index].spawnRateCount -= Time.deltaTime;
                else
                {
                    // Создаем следующий объект в группе
                    Spawn(spawnGroups[index].spawnObjects, spawnGroups[index].spawnIndex, spawnGroups[index].spawnObjectDistance);

                    // Переходим к следующему объекту создания в списке
                    spawnGroups[index].spawnIndex++;

                    // Сбрасываем индекс, если достигаем конца списка
                    if (spawnGroups[index].spawnIndex > spawnGroups[index].spawnObjects.Length - 1) spawnGroups[index].spawnIndex = 0;

                    // Сбрасываем счетчик скорости создания
                    spawnGroups[index].spawnRateCount = spawnGroups[index].spawnRate;
                }

            }
        }
    }

    /// <summary>
    /// Создает объект на основе выбранного индекса из массива
    /// </summary>
    /// <param name="spawnArray"></param>
    /// <param name="spawnIndex"></param>
    /// <param name="spawnGap"></param>
    public void Spawn(Transform[] spawnArray, int spawnIndex, Vector2 spawnGap)
    {
        // Если массив пуст, то не продолжаем
        if (spawnArray[spawnIndex] == null) return;

        // Создаем новый объект на основе индекса, который циклично проходит по списку
        Transform newSpawn = Instantiate(spawnArray[spawnIndex]) as Transform;

        // Создаем объект в целевой позиции
        if (spawnAroundObject) newSpawn.position = spawnAroundObject.transform.position;// + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

        // Вращаем объект случайным образом, затем перемещаем его вперед на случайное расстояние от точки создания
        newSpawn.eulerAngles = Vector3.up * Random.Range(0, 360);
        newSpawn.Translate(Vector3.forward * Random.Range(spawnGap.x, spawnGap.y), Space.Self);

        // Затем вращаем его обратно, чтобы смотреть на точку создания
        newSpawn.eulerAngles += Vector3.up * 180;

        // Размещаем объект на той же высоте, что и целевая точка создания
        //if (spawnAroundObject) newSpawn.position += Vector3.up * spawnAroundObject.position.y;

        RaycastHit hit;

        if (Physics.Raycast(newSpawn.position + Vector3.up * 5, -10 * Vector3.up, out hit, 100, creationEarth.groundLayer)) newSpawn.position = hit.point;


    }
}

