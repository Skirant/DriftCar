using UnityEngine;
using UnityEngine.UIElements;

//Скрипт двигает текстуру земли создавая эффект движения по ней
public class CreationEarth : MonoBehaviour
{
    public Transform cameraHolder;
    public Transform playerObject;
    internal Transform miniMap;
    public Transform groundObject;

    public LayerMask groundLayer;

    internal float cameraRotate = 0;
    public float groundTextureSpeed = 0;

    private Renderer groundRenderer;

    void Start()
    {
        if (groundObject)
        {
            groundRenderer = groundObject.GetComponent<Renderer>();
        }
    }

    void LateUpdate()
    {
        if (playerObject)
        {
            if (cameraHolder)
            {
                cameraHolder.position = playerObject.position;

                if (cameraRotate > 0)
                {
                    cameraHolder.eulerAngles = Vector3.up * Mathf.LerpAngle(cameraHolder.eulerAngles.y, playerObject.eulerAngles.y, Time.deltaTime * cameraRotate);
                }
            }

            if (miniMap)
            {
                miniMap.position = playerObject.position;
            }

            if (groundObject && groundRenderer)
            {
                // Only change the x and z coordinate, keep y constant
                groundObject.position = new Vector3(playerObject.position.x, groundObject.position.y, playerObject.position.z);

                // Update texture UV if needed
                float offsetX = playerObject.position.x * groundTextureSpeed;
                float offsetZ = playerObject.position.z * groundTextureSpeed;
                Vector2 offset = new Vector2(offsetX, offsetZ);
                groundRenderer.material.SetTextureOffset("_BaseMap", offset);
            }
        }
    }
}
