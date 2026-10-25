using UnityEngine;

public class BendDeform : MeshDeformBase
{
    public float bendStrength = 0.8f;

    public override void Deform(float param)
    {
        if (originalVertices == null) return;
        Vector3[] verts = originalVertices.Clone() as Vector3[];

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = verts[i];
            float rad = bendStrength * v.x;
            float r = 1 / bendStrength;
            float newX = r * Mathf.Sin(rad);
            float newZ = r * (1 - Mathf.Cos(rad)) + v.z;
            verts[i] = new Vector3(newX, v.y, newZ);
        }
        targetMesh.vertices = verts;
        targetMesh.RecalculateNormals();
        targetMesh.RecalculateBounds();
    }
}
