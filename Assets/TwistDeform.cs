using UnityEngine;

public class TwistDeform : MeshDeformBase
{
    public float twistStrength = 60f;

    public override void Deform(float param)
    {
        if (originalVertices == null) return;
        Vector3[] verts = originalVertices.Clone() as Vector3[];

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = verts[i];
            float angle = twistStrength * v.y * Mathf.Deg2Rad;
            float c = Mathf.Cos(angle);
            float s = Mathf.Sin(angle);
            float nx = v.x * c - v.z * s;
            float nz = v.x * s + v.z * c;
            verts[i] = new Vector3(nx, v.y, nz);
        }
        targetMesh.vertices = verts;
        targetMesh.RecalculateNormals();
        targetMesh.RecalculateBounds();
    }
}
