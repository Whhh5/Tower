using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class CameraMgr : Singleton<CameraMgr>, IUpdateBase
{
    public int UpdateLevelID { get; set; }
    public EUpdateLevel UpdateLevel => EUpdateLevel.Level3;

    public float UpdateDelta { get; set; }
    public float LasteUpdateTime { get; set; }

    public override void Awake()
    {
        base.Awake();

        ScreenSize = new Vector2(Screen.width, Screen.height);
        m_MainCamera = Camera.main;
        m_ToFieldOfViewValue = m_MainCamera.fieldOfView;
        m_ToViewPos = m_MainCamera.transform.position;
    }
    public override void Start()
    {
        base.Start();

        GTools.LifecycleMgr.AddUpdate(this);
    }



    private Vector2 m_ToEdgeRatio = new Vector2(0.2f, 0.1f);
    private float m_MoveSpeed = 10;
    private float m_FieldOfViewValueInterval = 3;
    private Vector2 ScreenSize;
    private Camera m_MainCamera = null;
    private float m_ToFieldOfViewValue = 0;
    private Vector3 m_ToViewPos = Vector3.zero;
    public void OnUpdate()
    {


        Vector2 mousePos = Input.mousePosition;
        if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height)
        {
            return;
        }
        if (!Input.GetKey(KeyCode.V))
        {
            return;
        }

        Vector2 mouseSpeed = Vector2.zero;
        var ratio = mousePos / ScreenSize;
        if (ratio.x > 0.5f)
        {
            var ration = Mathf.Clamp01(Mathf.Max(ratio.x - (1 - m_ToEdgeRatio.x), 0) / m_ToEdgeRatio.x);
            mouseSpeed.x = ration * m_MoveSpeed;
        }
        else
        {
            var ration = Mathf.Clamp01(Mathf.Max(m_ToEdgeRatio.x - ratio.x, 0) / m_ToEdgeRatio.x);
            mouseSpeed.x = -ration * m_MoveSpeed;
        }

        if (ratio.y > 0.5f)
        {
            var ration = Mathf.Clamp01(Mathf.Max(ratio.y - (1 - m_ToEdgeRatio.y), 0) / m_ToEdgeRatio.y);
            mouseSpeed.y = ration * m_MoveSpeed;
        }
        else
        {
            var ration = Mathf.Clamp01(Mathf.Max(m_ToEdgeRatio.y - ratio.y, 0) / m_ToEdgeRatio.y);
            mouseSpeed.y = -ration * m_MoveSpeed;
        }

        m_ToViewPos += new Vector3(mouseSpeed.x, 0, mouseSpeed.y) * Time.deltaTime;
        m_MainCamera.transform.position = Vector3.Lerp(m_MainCamera.transform.position, m_ToViewPos, Time.deltaTime * m_MoveSpeed);



        var scrolDelta = Input.mouseScrollDelta;
        if (scrolDelta.y != 0)
        {
            m_ToFieldOfViewValue = Mathf.Clamp(-scrolDelta.y * m_FieldOfViewValueInterval + m_ToFieldOfViewValue, 10, 60);
        }
        var fieldOfViewValue = Mathf.Lerp(m_MainCamera.fieldOfView, m_ToFieldOfViewValue, Time.deltaTime * m_MoveSpeed);
        m_MainCamera.fieldOfView = fieldOfViewValue;
    }
}
