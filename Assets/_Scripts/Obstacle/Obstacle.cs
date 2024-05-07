using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Tooltip("”щерб, причиненный этим преп€тствием")]
    public float damage = 1;

    [Tooltip("Ёффект, который создаетс€ в месте расположени€ этого объекта при ударе по нему")]
    public Transform hitEffect;

    [Tooltip("—ледует ли устран€ть это преп€тствие при наезде автомобил€?")]
    public bool removeOnHit = false;

    [Tooltip("—лучайное вращение, заданное объекту только по оси Y")]
    public float randomRotation = 360;

    void Start()
    {
        // Set a random rotation angle for the object
        transform.eulerAngles += Vector3.up * Random.Range(-randomRotation, randomRotation);

        // Resets the color of an obstacle periodically
        //InvokeRepeating("ResetColor", 0, 0.5f);
    }

    /// <summary>
    /// Is executed when this obstacle touches another object with a trigger collider
    /// </summary>
    /// <param name="other"><see cref="Collider"/></param>
    void OnTriggerStay(Collider other)
    {
        // If the hurt delay is over, and this obstacle was hit by a car, damage the car
        if (other.GetComponent<CarController>())
        {
            //if ( other.GetComponent<ECCCar>().hurtDelayCount <= 0 )
            //{
            // Reset the hurt delay
            //other.GetComponent<ECCCar>().hurtDelayCount = other.GetComponent<ECCCar>().hurtDelay;

            // Damage the car
            /*other.GetComponent<CarController>().ChangeHealth(-damage);

            // If there is a hit effect, create it
            if (other.GetComponent<CarController>().health - damage > 0 && other.GetComponent<CarController>().hitEffect) Instantiate(other.GetComponent<CarController>().hitEffect, transform.position, transform.rotation);
            //}

            // If there is a hit effect, create it
            if (hitEffect) Instantiate(hitEffect, transform.position, transform.rotation);

            // Remove the object from the game
            if (removeOnHit == true) Destroy(gameObject);*/
        }
    }

    public void ResetColor()
    {
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
    }
}

