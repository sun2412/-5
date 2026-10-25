using UnityEngine;
using UnityEngine.UI;

public enum DeformMode
{
    None, Taper, Twist, Bend, FFD
}

public class DeformManager : MonoBehaviour
{
    [HideInInspector] public TaperDeform taper;
    [HideInInspector] public TwistDeform twist;
    [HideInInspector] public BendDeform bend;
    [HideInInspector] public FFDDeform ffd;
    public Slider paramSlider;

    private DeformMode currentMode = DeformMode.None;

    void Start()
    {
        GameObject deformObj = GameObject.Find("DeformModel");
        taper = deformObj.GetComponent<TaperDeform>();
        twist = deformObj.GetComponent<TwistDeform>();
        bend = deformObj.GetComponent<BendDeform>();
        ffd = deformObj.GetComponent<FFDDeform>();

        // 自动找Slider
        if (paramSlider == null)
        {
            GameObject sliderObj = GameObject.Find("Slider_Param");
            if (sliderObj != null)
                paramSlider = sliderObj.GetComponent<Slider>();
        }

        // 打印调试信息
        Debug.Log($"taper:{taper != null}, twist:{twist != null}, bend:{bend != null}, ffd:{ffd != null}");
    }

    public void SetModeNone()
    {
        ResetAll();
        currentMode = DeformMode.None;
    }
    public void SetTaper()
    {
        ResetAll();
        currentMode = DeformMode.Taper;
    }
    public void SetTwist()
    {
        ResetAll();
        currentMode = DeformMode.Twist;
    }
    public void SetBend()
    {
        ResetAll();
        currentMode = DeformMode.Bend;
    }
    public void SetFFD()
    {
        ResetAll();
        ffd.InitFFDGrid();
        ffd.RandomPerturbControl(); // 没有UI拖动控制点，先随机扰动让FFD有可见效果
        currentMode = DeformMode.FFD;
    }

    void Update()
    {
        // 没有Slider时也要能变形，用默认参数1
        if (taper == null || twist == null || bend == null || ffd == null)
            return;

        float p = paramSlider != null ? paramSlider.value : 1f;
        switch (currentMode)
        {
            case DeformMode.Taper: taper.Deform(p); break;
            case DeformMode.Twist: twist.Deform(p); break;
            case DeformMode.Bend: bend.Deform(p); break;
            case DeformMode.FFD: ffd.Deform(p); break;
        }
    }

    void ResetAll()
    {
        taper.ResetMesh();
        twist.ResetMesh();
        bend.ResetMesh();
        ffd.ResetMesh();
        ffd.ResetFFDControlPoints();
    }
}
