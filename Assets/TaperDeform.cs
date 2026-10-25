using UnityEngine;

public class TaperDeform : MeshDeformBase
{
    public float taperStrength = 0.5f;
    public Vector2 yRange = new Vector2(-1, 1);

    public override void Deform(float param)
    {
        if (originalVertices == null) return;
        Vector3[] verts = originalVertices.Clone() as Vector3[];
        float t = Mathf.Clamp01((param - yRange.x) / (yRange.y - yRange.x));

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = verts[i];
            float scale = 1.0f + taperStrength * v.y;
            v.x *= scale;
            v.z *= scale;
            verts[i] = v;
        }
        targetMesh.vertices = verts;
        targetMesh.RecalculateNormals();
        targetMesh.RecalculateBounds();
    }
}
