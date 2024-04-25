using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public GameObject planePrefab; // ������ ���������
    public float planeSpawnDistance = 10f; // ����������, �� ������� ��������� ����� ���������
    public int planesToSpawn = 3; // ���������� ����������, ������� ����� �������

    private Transform carTransform; // ��������� ������
    private Vector3 lastPlanePosition; // ��������� ������� ��������� ���������

    private void Start()
    {
        carTransform = transform; // �������� ��������� ������
        lastPlanePosition = carTransform.position; // ��������� ������� - ������� ������

        // ������� ��������� ���������
        for (int i = 0; i < planesToSpawn; i++)
        {
            SpawnPlane();
        }
    }

    private void Update()
    {
        // ��������� ���������� ����� ������� � ��������� ��������� ����������
        float distanceToLastPlane = Vector3.Distance(carTransform.position, lastPlanePosition);

        // ���� ���������� ������ ���������, ������� ����� ���������
        if (distanceToLastPlane < planeSpawnDistance)
        {
            SpawnPlane();
        }
    }

    private void SpawnPlane()
    {
        // ���������� �����������, � ������� ��������� ���������
        Vector3 planeSpawnDirection = carTransform.forward;

        // ������� ����� ��������� ������� ������
        GameObject newPlane = Instantiate(planePrefab, lastPlanePosition + planeSpawnDirection * planeSpawnDistance, Quaternion.identity);

        // ��������� ������� ��������� ���������
        lastPlanePosition = newPlane.transform.position;
    }
}
