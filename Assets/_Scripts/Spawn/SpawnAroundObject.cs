using UnityEngine;

public class SpawnAroundObject : MonoBehaviour
{
    static CreationEarth creationEarth;

    [Tooltip("��� �������, ������ �������� ��������� ������ ������� � ������������ ���������")]
    public string spawnAroundTag = "Player";
    internal Transform spawnAroundObject;

    [Tooltip("�������������, ������� �������� � ��������� ��������. ���� True, �� ������ ������� �������")]
    public bool isSpawning = false;

    [System.Serializable]
    public class SpawnGroup
    {
        [Tooltip("������ ���� ��������, ������� ����� �������")]
        public Transform[] spawnObjects;

        [Tooltip("���� �������� �������� � ��������.")]
        public float spawnRate = 5;
        internal float spawnRateCount = 0;
        internal int spawnIndex = 0;

        [Tooltip("����������, �� ������� ���� ������ ��������� ������������ spawnAroundObject")]
        public Vector2 spawnObjectDistance = new Vector2(10, 20);
    }

    [Tooltip("������ ����� ��������. ��� ����� ����, ��������, ��������� ����������, �������� ��� ����� ��� ����������� � ���� ������")]
    public SpawnGroup[] spawnGroups;

    internal int index;

    private void Start()
    {
        // ��������� ��������� ���������� ��� �������� �������
        if (creationEarth == null) creationEarth = GameObject.FindObjectOfType<CreationEarth>();
    }

    /// <summary>
    /// Update ���������� ������ ����, ���� MonoBehaviour �������.
    /// </summary>
    void Update()
    {
        // ���� � ��� ��� ������� ��� �������� ������, ���� ��� �� ����� � ���������
        if (spawnAroundObject == null && spawnAroundTag != string.Empty && GameObject.FindGameObjectWithTag(spawnAroundTag)) spawnAroundObject = GameObject.FindGameObjectWithTag(spawnAroundTag).transform;

        // ���� �� �� �������, �� �� ����������
        if (isSpawning == false) return;

        // �������� ����� ��� ������ ��������, ������� ���� � ������� �������
        for (index = 0; index < spawnGroups.Length; index++)
        {
            // ���� ���� ������� ��� ��������, ����������
            if (spawnGroups[index].spawnObjects.Length > 0)
            {
                // ������� �� ���������� �������� �������
                if (spawnGroups[index].spawnRateCount > 0) spawnGroups[index].spawnRateCount -= Time.deltaTime;
                else
                {
                    // ������� ��������� ������ � ������
                    Spawn(spawnGroups[index].spawnObjects, spawnGroups[index].spawnIndex, spawnGroups[index].spawnObjectDistance);

                    // ��������� � ���������� ������� �������� � ������
                    spawnGroups[index].spawnIndex++;

                    // ���������� ������, ���� ��������� ����� ������
                    if (spawnGroups[index].spawnIndex > spawnGroups[index].spawnObjects.Length - 1) spawnGroups[index].spawnIndex = 0;

                    // ���������� ������� �������� ��������
                    spawnGroups[index].spawnRateCount = spawnGroups[index].spawnRate;
                }

            }
        }
    }

    /// <summary>
    /// ������� ������ �� ������ ���������� ������� �� �������
    /// </summary>
    /// <param name="spawnArray"></param>
    /// <param name="spawnIndex"></param>
    /// <param name="spawnGap"></param>
    public void Spawn(Transform[] spawnArray, int spawnIndex, Vector2 spawnGap)
    {
        // ���� ������ ����, �� �� ����������
        if (spawnArray[spawnIndex] == null) return;

        // ������� ����� ������ �� ������ �������, ������� �������� �������� �� ������
        Transform newSpawn = Instantiate(spawnArray[spawnIndex]) as Transform;

        // ������� ������ � ������� �������
        if (spawnAroundObject) newSpawn.position = spawnAroundObject.transform.position;// + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

        // ������� ������ ��������� �������, ����� ���������� ��� ������ �� ��������� ���������� �� ����� ��������
        newSpawn.eulerAngles = Vector3.up * Random.Range(0, 360);
        newSpawn.Translate(Vector3.forward * Random.Range(spawnGap.x, spawnGap.y), Space.Self);

        // ����� ������� ��� �������, ����� �������� �� ����� ��������
        newSpawn.eulerAngles += Vector3.up * 180;

        // ��������� ������ �� ��� �� ������, ��� � ������� ����� ��������
        //if (spawnAroundObject) newSpawn.position += Vector3.up * spawnAroundObject.position.y;

        RaycastHit hit;

        if (Physics.Raycast(newSpawn.position + Vector3.up * 5, -10 * Vector3.up, out hit, 100, creationEarth.groundLayer)) newSpawn.position = hit.point;


    }
}

