using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public GameObject planePrefab; // Префаб плоскости
    public float planeSpawnDistance = 10f; // Расстояние, на котором создается новая плоскость
    public int planesToSpawn = 3; // Количество плоскостей, которые нужно создать

    private Transform carTransform; // Трансформ машины
    private Vector3 lastPlanePosition; // Последняя позиция созданной плоскости

    private void Start()
    {
        carTransform = transform; // Получаем трансформ машины
        lastPlanePosition = carTransform.position; // Начальная позиция - позиция машины

        // Создаем начальные плоскости
        for (int i = 0; i < planesToSpawn; i++)
        {
            SpawnPlane();
        }
    }

    private void Update()
    {
        // Проверяем расстояние между машиной и последней созданной плоскостью
        float distanceToLastPlane = Vector3.Distance(carTransform.position, lastPlanePosition);

        // Если расстояние меньше заданного, создаем новую плоскость
        if (distanceToLastPlane < planeSpawnDistance)
        {
            SpawnPlane();
        }
    }

    private void SpawnPlane()
    {
        // Определяем направление, в котором создается плоскость
        Vector3 planeSpawnDirection = carTransform.forward;

        // Создаем новую плоскость впереди машины
        GameObject newPlane = Instantiate(planePrefab, lastPlanePosition + planeSpawnDirection * planeSpawnDistance, Quaternion.identity);

        // Обновляем позицию последней плоскости
        lastPlanePosition = newPlane.transform.position;
    }
}
