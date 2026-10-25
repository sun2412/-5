using UnityEngine;

public class MeshDeformBase : MonoBehaviour
{
    [Header("基础引用")]
    public MeshFilter meshFilter;
    protected Vector3[] originalVertices;
    protected Mesh targetMesh;

    protected virtual void Awake()
    {
        InitMesh();
    }

    // 新增：公开初始化方法，给外部调用
    public void InitMesh()
    {
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
        if (meshFilter?.mesh != null)
        {
            targetMesh = meshFilter.mesh;
            originalVertices = targetMesh.vertices.Clone() as Vector3[];
        }
    }

    public virtual void ResetMesh()
    {
        if (originalVertices == null || targetMesh == null) return;
        targetMesh.vertices = originalVertices.Clone() as Vector3[];
        targetMesh.RecalculateNormals();
        targetMesh.RecalculateBounds();
    }

    public virtual void Deform(float param)
    {

    }
}
