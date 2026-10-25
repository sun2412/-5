using UnityEngine;
using System.IO;

public class ObjLoader : MonoBehaviour
{
    public static Mesh LoadObj(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("OBJ文件不存在：" + filePath);
            return null;
        }
        string[] lines = File.ReadAllLines(filePath);
        System.Collections.Generic.List<Vector3> verts = new System.Collections.Generic.List<Vector3>();
        System.Collections.Generic.List<int> tris = new System.Collections.Generic.List<int>();

        foreach (var line in lines)
        {
            var tokens = line.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) continue;
            if (tokens[0] == "v")
            {
                verts.Add(new Vector3(
                    float.Parse(tokens[1]),
                    float.Parse(tokens[2]),
                    float.Parse(tokens[3])
                ));
            }
            else if (tokens[0] == "f")
            {
                // f 顶点/uv/法向，只取顶点索引；面可能是三角/四边形/N边形
                int n = tokens.Length - 1;
                int[] idx = new int[n];
                for (int k = 0; k < n; k++)
                {
                    int vi = int.Parse(tokens[k + 1].Split('/')[0]);
                    idx[k] = vi > 0 ? vi - 1 : verts.Count + vi; // 支持负数索引
                }
                // 扇形三角化：v0-v1-v2, v0-v2-v3, ...
                for (int k = 1; k < n - 1; k++)
                {
                    tris.Add(idx[0]);
                    tris.Add(idx[k]);
                    tris.Add(idx[k + 1]);
                }
            }
        }
        Mesh m = new Mesh();
        m.vertices = verts.ToArray();
        m.triangles = tris.ToArray();
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }
}
