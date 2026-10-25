using UnityEngine;
using System.Collections.Generic;

public class FFDDeform : MeshDeformBase
{
    [Header("FFD网格分辨率")]
    public int resX = 4;
    public int resY = 4;
    public int resZ = 4;

    [Header("调试绘制控制点")]
    public bool drawGizmos = true;
    public float gizmoSize = 0.15f;

    private Bounds modelBounds;
    private Vector3[,,] controlPoints; // [x,y,z]控制点数组

    void Start()
    {
        InitFFDGrid();
    }

    // 自动根据模型包围盒初始化控制点网格
    public void InitFFDGrid()
    {
        if (originalVertices == null)
        {
            Debug.LogWarning("原始顶点为空，无法初始化FFD");
            return;
        }
        // 计算包围盒
        modelBounds = new Bounds();
        modelBounds.SetMinMax(Vector3.one * float.MaxValue, Vector3.one * float.MinValue);
        foreach (var v in originalVertices)
        {
            modelBounds.Encapsulate(v);
        }

        controlPoints = new Vector3[resX, resY, resZ];
        Vector3 min = modelBounds.min;
        Vector3 size = modelBounds.size;

        // 均匀生成网格控制点
        for (int xi = 0; xi < resX; xi++)
        {
            float u = (float)xi / (resX - 1);
            for (int yi = 0; yi < resY; yi++)
            {
                float v = (float)yi / (resY - 1);
                for (int zi = 0; zi < resZ; zi++)
                {
                    float w = (float)zi / (resZ - 1);
                    Vector3 pos = min + new Vector3(u * size.x, v * size.y, w * size.z);
                    controlPoints[xi, yi, zi] = pos;
                }
            }
        }
    }

    // 三线性插值FFD变形
    public override void Deform(float unused)
    {
        if (originalVertices == null || controlPoints == null) return;
        Vector3[] verts = new Vector3[originalVertices.Length];
        Vector3 bMin = modelBounds.min;
        Vector3 bSize = modelBounds.size;

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 v = originalVertices[i];
            // 转到包围盒内归一化UVW [0,1]
            float u = (v.x - bMin.x) / bSize.x;
            float vv = (v.y - bMin.y) / bSize.y;
            float w = (v.z - bMin.z) / bSize.z;
            u = Mathf.Clamp01(u);
            vv = Mathf.Clamp01(vv);
            w = Mathf.Clamp01(w);

            // 找到网格索引
            float fx = u * (resX - 1);
            float fy = vv * (resY - 1);
            float fz = w * (resZ - 1);

            int ix0 = Mathf.FloorToInt(fx);
            int iy0 = Mathf.FloorToInt(fy);
            int iz0 = Mathf.FloorToInt(fz);
            int ix1 = Mathf.Min(ix0 + 1, resX - 1);
            int iy1 = Mathf.Min(iy0 + 1, resY - 1);
            int iz1 = Mathf.Min(iz0 + 1, resZ - 1);

            float su = fx - ix0;
            float sv = fy - iy0;
            float sw = fz - iz0;

            // 8个控制点
            Vector3 c000 = controlPoints[ix0, iy0, iz0];
            Vector3 c100 = controlPoints[ix1, iy0, iz0];
            Vector3 c010 = controlPoints[ix0, iy1, iz0];
            Vector3 c110 = controlPoints[ix1, iy1, iz0];
            Vector3 c001 = controlPoints[ix0, iy0, iz1];
            Vector3 c101 = controlPoints[ix1, iy0, iz1];
            Vector3 c011 = controlPoints[ix0, iy1, iz1];
            Vector3 c111 = controlPoints[ix1, iy1, iz1];

            // 三线性插值
            Vector3 interp = Vector3.Lerp(
                Vector3.Lerp(Vector3.Lerp(c000, c100, su), Vector3.Lerp(c010, c110, su), sv),
                Vector3.Lerp(Vector3.Lerp(c001, c101, su), Vector3.Lerp(c011, c111, su), sv),
                sw
            );
            verts[i] = interp;
        }

        targetMesh.vertices = verts;
        targetMesh.RecalculateNormals();
        targetMesh.RecalculateBounds();
    }

    // 重置FFD控制点回到包围盒原始网格
    public void ResetFFDControlPoints()
    {
        InitFFDGrid();
    }

    // Gizmos绘制控制点
    void OnDrawGizmosSelected()
    {
        if (!drawGizmos || controlPoints == null) return;
        Gizmos.color = Color.red;
        for (int x = 0; x < resX; x++)
            for (int y = 0; y < resY; y++)
                for (int z = 0; z < resZ; z++)
                {
                    Gizmos.DrawSphere(controlPoints[x, y, z], gizmoSize);
                }
    }

    // 【演示用】随机扰动控制点，一键看到FFD变形效果
    [ContextMenu("Random Move Control Points")]
    public void RandomPerturbControl()
    {
        for (int x = 0; x < resX; x++)
            for (int y = 0; y < resY; y++)
                for (int z = 0; z < resZ; z++)
                {
                    controlPoints[x, y, z] += Random.insideUnitSphere * 0.25f;
                }
    }
}
