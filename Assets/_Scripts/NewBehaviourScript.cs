using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [MenuItem("Tools/ChangeShader")]
    public static void ChangeShader()
    {
        Shader newShader = Shader.Find("Standard");
        if (newShader == null)
        {
            Debug.LogError("Unable to find shader: " + "Standard");
            return;
        }

        foreach (Material material in Resources.FindObjectsOfTypeAll<Material>())
        {
            material.shader = newShader;
        }
    }
}
