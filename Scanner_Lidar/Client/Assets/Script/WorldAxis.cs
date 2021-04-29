using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAxis : MonoBehaviour
{
    /// <summary>
    /// @Author Veljko Markovic
    /// @Version 01.04.2021
    /// 
    /// Fonte: https://docs.unity3d.com/ScriptReference/GL.html
    /// </summary>

    // When added to an object, draws colored rays from the
    // transform position.
    public int lineCount = 100;
    public float radius = 3.0f;

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);

        // Asse X
        GL.Color(Color.red);
        GL.Vertex3(-2500, 0, 0);
        GL.Vertex3(2500, 0, 0);
        
        // Asse Y
        GL.Color(Color.green);
        GL.Vertex3(0, -2500, 0);
        GL.Vertex3(0, 2500, 0);

        // Asse Z
        GL.Color(Color.blue);
        GL.Vertex3(0, 0, -2500);
        GL.Vertex3(0, 0, 2500);

        GL.End();
        GL.PopMatrix();
    }
}
