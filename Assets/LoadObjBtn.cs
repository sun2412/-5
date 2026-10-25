using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadObjBtn : MonoBehaviour
{
    public GameObject deformTarget;
    [Tooltip("加载后模型最大边的目标长度（单位：米）")]
    public float targetSize = 2f;

    public void LoadObjFile()
    {
        string path = Application.streamingAssetsPath + "/test.obj";
        Mesh m = ObjLoader.LoadObj(path);
        if (m == null)
        {
            Debug.Log("OBJ加载失败！检查路径和文件");
            return;
        }

        // 归一化尺寸：把模型最大边缩放到 targetSize，避免 obj 单位过大/过小
        m.RecalculateBounds();
        Bounds b = m.bounds;
        float maxDim = Mathf.Max(b.size.x, b.size.y, b.size.z);
        if (maxDim > 0.0001f)
        {
            float k = targetSize / maxDim;
            Vector3[] vs = m.vertices;
            for (int i = 0; i < vs.Length; i++)
                vs[i] *= k;
            m.vertices = vs;
            m.RecalculateNormals();
            m.RecalculateBounds();
        }

        MeshFilter mf = deformTarget.GetComponent<MeshFilter>();
        mf.mesh = m;

        var allDeform = deformTarget.GetComponents<MeshDeformBase>();
        foreach (var d in allDeform)
        {
            d.InitMesh();  // 这里改了！不再调用Awake
        }
        var ffd = deformTarget.GetComponent<FFDDeform>();
        ffd.InitFFDGrid();
    }
}
