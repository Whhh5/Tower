
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using B1;
using System.Collections.Generic;
using B1.UI;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;
using B1.Event;
using System.IO;
using System.Collections;
using UnityEngine.UI;

[InitializeOnLoad]
public static class EditorEditor
{
    private static readonly Type kToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
    private static ScriptableObject sCurrentToolbar;


    static EditorEditor()
    {
        EditorApplication.update += OnUpdate;
    }

    private static void OnUpdate()
    {
        if (sCurrentToolbar == null)
        {
            UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(kToolbarType);
            sCurrentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
            if (sCurrentToolbar != null)
            {
                FieldInfo root = sCurrentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                VisualElement concreteRoot = root.GetValue(sCurrentToolbar) as VisualElement;

                VisualElement toolbarZone = concreteRoot.Q("ToolbarZoneRightAlign");
                VisualElement parent = new VisualElement()
                {
                    style = {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                            }
                };
                IMGUIContainer container = new IMGUIContainer();
                container.onGUIHandler += OnGuiBody;
                parent.Add(container);
                toolbarZone.Add(parent);


                // left
                VisualElement toolbarZoneLeft = concreteRoot.Q("ToolbarZoneLeftAlign");
                VisualElement parentLeft = new()
                {
                    style = {
                                flexGrow = 1,
                                flexDirection = FlexDirection.RowReverse,
                            }
                };
                IMGUIContainer container2 = new IMGUIContainer();
                container2.onGUIHandler += OnGuiBody2;
                parentLeft.Add(container2);



                toolbarZoneLeft.Add(parentLeft);
            }
        }

    }

    private static void OnGuiBody2()
    {
        //自定义按钮加在此处
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("×1", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            Time.timeScale = 1;
        }
        if (GUILayout.Button(new GUIContent("×2", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            Time.timeScale = 2;
        }
        if (GUILayout.Button(new GUIContent("×5", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            Time.timeScale = 5;
        }
        if (GUILayout.Button(new GUIContent("×10", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            Time.timeScale = 10;
        }
        GUILayout.EndHorizontal();
    }
    private static void OnGuiBody()
    {
        //自定义按钮加在此处
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Game Debuger", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<GameDebugerWindow>())
            {
                var window = EditorWindow.CreateWindow<GameDebugerWindow>();
                window.Show();
            }
        }
        if (GUILayout.Button(new GUIContent("Debuger", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<DebugerWindow>())
            {
                var window = EditorWindow.CreateWindow<DebugerWindow>();
                window.Show();
            }
        }
        if (GUILayout.Button(new GUIContent("My Node Editor", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            var window = EditorWindow.GetWindow<MyNodeEditor>();

            window.titleContent = new GUIContent("My Node Editor");
        }
        GUILayout.EndHorizontal();
    }
}

public class GameDebugerWindow : EditorWindow
{
    public Vector3 m_CurMousePosition = Vector3.zero;

    public Vector3Int m_CreateRoadParams = Vector3Int.one;
    private void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = GTools.MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, (int)ELayer.Terrain))
            {
                m_CurMousePosition = hit.point;
            }
        }
        EditorGUILayout.BeginHorizontal();
        {
            m_CurMousePosition = EditorGUILayout.Vector3Field("", m_CurMousePosition);
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Create Player", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
            {
                GTools.AssetsMgr.LoadPrefabPoolAsync<Person_Player>(EAssetName.Person_Player, null, m_CurMousePosition);
            }
            if (GUILayout.Button("Create Enemy", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
            {
                GTools.AssetsMgr.LoadPrefabPoolAsync<Person_Enemy>(EAssetName.Person_Enemy_Crab, null, m_CurMousePosition);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        {
            m_CreateRoadParams = EditorGUILayout.Vector3IntField("Create Road", m_CreateRoadParams);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Create Chunk", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
                {
                    WorldMapMgr.Ins.CreateChunkTest();
                }
                if (GUILayout.Button("Clear Chunk", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
                {
                    WorldMapMgr.Ins.CreateMapChunk_Clear();
                }
                if (GUILayout.Button("Create Alt", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
                {
                    WorldMapMgr.Ins.CreateAltsData();
                }
                if (GUILayout.Button("Create Road", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
                {
                    WorldMapMgr.Ins.CreateRoadDataTest(m_CreateRoadParams.x, m_CreateRoadParams.y, m_CreateRoadParams.z);
                }
                if (GUILayout.Button("Extend Road", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
                {
                    WorldMapMgr.Ins.CreateRoadExtend();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Create Spawn Point", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
                {
                    WorldMapMgr.Ins.InitMonsterSpawnPointData();
                }
                if (GUILayout.Button("Create Spawn Point", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
                {
                    WorldMapMgr.Ins.CreateTowerLight();
                }
                if (GUILayout.Button("Create Monster", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
                {
                    MonsterMgr.Ins.CreateEntityTest();
                }
                if (GUILayout.Button("Clear Monster", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
                {
                    MonsterMgr.Ins.ClearEntity();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Create Hero", GUILayout.Width(150), GUILayout.Height(50)) && Application.isPlaying)
                {
                    
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Test Mathf", GUILayout.Width(150), GUILayout.Height(50)))
                {

                    var targetDirection = Vector3.right;

                    var dot = Vector3.Dot(Vector3.forward, targetDirection);

                    var temp1 = dot / Vector3.Distance(targetDirection, Vector3.zero);
                    var temp2 = Mathf.Acos(temp1);
                    var angle = temp2 * Mathf.Rad2Deg;

                    var coss = Vector3.Cross(Vector3.forward, targetDirection);

                    var symbol = coss.y / Mathf.Abs(coss.y);

                    var targetAngle = angle * symbol;

                }
                if (GUILayout.Button("Test Crad Init", GUILayout.Width(150), GUILayout.Height(50)))
                {


                }

                if (GUILayout.Button("Test Crad Get", GUILayout.Width(150), GUILayout.Height(50)))
                {

                }
                if (GUILayout.Button("Test Crad Recycle", GUILayout.Width(150), GUILayout.Height(50)))
                {
                    
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Test Terrain", GUILayout.Width(150), GUILayout.Height(50)))
                {
                    TerrainMgr.Ins.SetColorTest();
                }

            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

}

public class DebugerWindow : EditorWindow
{
    private void OnGUI()
    {
        //自定义按钮加在此处
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Asset Manager", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<AssetManagerWindow>())
            {
                var window = EditorWindow.CreateWindow<DebugerWindow>();
                window.Show();
            }
        }
        if (GUILayout.Button(new GUIContent("UI Window Manager", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<UIWindowManagerWindow>())
            {
                var window = EditorWindow.CreateWindow<UIWindowManagerWindow>();
                window.Show();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Property Window", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<PropertyWindow>())
            {
                var window = EditorWindow.CreateWindow<PropertyWindow>();
                window.Show();
            }
        }
        if (GUILayout.Button(new GUIContent("Message System", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<MessagingSystemWindow>())
            {
                var window = EditorWindow.CreateWindow<MessagingSystemWindow>();
                window.Show();
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Quad Tree", EditorGUIUtility.FindTexture("PlayButton"))))
        {
            if (!EditorWindow.GetWindow<QuadTreeWindow>())
            {
                var window = EditorWindow.GetWindow<QuadTreeWindow>();
                window.Show();
            }
        }

        GUILayout.EndHorizontal();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}

public class AssetManagerWindow : EditorWindow
{
    Dictionary<string, (Type type, bool isIns, object assets, Dictionary<int, GameObject> objs)> m_DicAssets = null;

    private void OnEnable()
    {
        var type = typeof(AssetsMgr)?.GetField("m_DicAssets", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);

        if (type != null)
        {
            m_DicAssets = type.GetValue(AssetsMgr.Ins) as Dictionary<string, (Type type, bool isIns, object assets, Dictionary<int, GameObject> objs)>;
        }
    }
    private void OnDisable()
    {
        m_DicAssets = null;
    }
    private void OnInspectorUpdate()
    {
        Repaint();
    }


    Vector2 m_ScrollV2Pos = Vector2.zero;
    Vector2 m_ScrollV2Pos2 = Vector2.zero;
    int m_LastCount = 0;
    string m_DebugStr = "";
    private void OnGUI()
    {
        if (m_DicAssets == null) return;

        EditorGUILayout.BeginHorizontal();
        m_ScrollV2Pos = EditorGUILayout.BeginScrollView(m_ScrollV2Pos, GUILayout.Width(position.width * 0.8f), GUILayout.Height(position.height));


        EditorGUILayout.LabelField($"Count: {m_DicAssets.Count}");
        EditorGUILayout.Space(20);


        EditorGUILayout.BeginHorizontal();

        foreach (var item in m_DicAssets)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField($"key: {item.Key}");
            EditorGUILayout.LabelField($"Type: {item.Value.type}");
            EditorGUILayout.LabelField($"Is Ins: {item.Value.isIns}");
            EditorGUILayout.LabelField($"Asset: {item.Value.assets}");
            EditorGUILayout.LabelField($"List: {item.Value.objs.Count}");

            foreach (var obj in item.Value.objs)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"\t[{obj.Key}]:", GUILayout.Width(100));
                EditorGUILayout.ObjectField(obj.Value, typeof(GameObject));

                EditorGUILayout.EndHorizontal();
            }



            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();






        m_ScrollV2Pos2 = EditorGUILayout.BeginScrollView(m_ScrollV2Pos2, GUILayout.Width(position.width * 0.2f), GUILayout.Height(position.height));


        if (m_DicAssets.Count != m_LastCount)
        {

            m_LastCount = m_DicAssets.Count;

            AddLog($"{System.DateTime.Now}: {m_LastCount}");

        }
        EditorGUILayout.TextArea(m_DebugStr);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }


    void AddLog(object f_Message)
    {
        m_DebugStr += $"{f_Message}\n";
    }

}


public class UIWindowManagerWindow : EditorWindow
{
    DicStack<Type, UIWindowPage> m_PageStack = null;

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
        m_PageStack = null;
    }
    private void OnInspectorUpdate()
    {
        Repaint();
    }


    Vector2 m_ScrollV2Pos = Vector2.zero;
    Vector2 m_ScrollV2Pos2 = Vector2.zero;
    int m_LastCount = 0;
    EUIWindowPage m_search = EUIWindowPage.None;
    private void OnGUI()
    {
        if (UIWindowManager.Ins != null && m_PageStack == null)
        {
            var type = typeof(UIWindowManager)?.GetField("m_PageStack", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);
            m_PageStack = type.GetValue(UIWindowManager.Ins) as DicStack<Type, UIWindowPage>;
        }

        if (m_PageStack == null) return;

        EditorGUILayout.BeginScrollView(m_ScrollV2Pos, GUILayout.Width(position.width), GUILayout.Height(position.height));




        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField($"Count = {m_PageStack.Count}");

        EditorGUILayout.LabelField($"搜索：", GUILayout.Width(100));
        m_search = (EUIWindowPage)EditorGUILayout.EnumPopup(m_search);


        EditorGUILayout.EndHorizontal();







        EditorGUILayout.BeginHorizontal();
        var fieldInfo = typeof(UIWindowPage).GetField("m_WindowStack", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);
        var getWindow = typeof(UIWindowPage).GetMethod("GetWindowAsync", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default);
        var index = 0;
        foreach (var item in m_PageStack)
        {
            if (m_search != EUIWindowPage.None && m_search != item.Value.CurPage)
            {
                continue;
            }
            var windowStack = fieldInfo.GetValue(item.Value) as ListStack<EAssetName>;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"[{index++}]Page: {item.Value.CurPage}");


            EditorGUILayout.BeginVertical();
            foreach (var element in windowStack.GetEnumerator())
            {
                var window = getWindow.Invoke(item.Value, new object[] { element.Value }) as UIWindow;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"\t[{element.Key}] = ", GUILayout.Width(100));
                EditorGUILayout.ObjectField(window, window?.GetType());

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

        }

        EditorGUILayout.EndHorizontal();



        EditorGUILayout.EndScrollView();
    }
}






public class PropertyWindow : EditorWindow
{
    enum EClass
    {
        None,
        UIWindow,
        UILobby,

    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }


    Vector2 m_ScrollV2Pos = Vector2.zero;
    Vector2 m_ScrollV2Pos2 = Vector2.zero;



    EClass m_ClassType = EClass.None;
    Object m_Class = null;
    Type m_Type = null;
    private void OnGUI()
    {
        m_ScrollV2Pos = EditorGUILayout.BeginScrollView(m_ScrollV2Pos, GUILayout.Width(position.width), GUILayout.Height(position.height));

        EditorGUILayout.BeginHorizontal();
        var typeStr = EditorGUILayout.TextField($"{m_Type}");
        m_ClassType = (EClass)EditorGUILayout.EnumPopup(m_ClassType);
        EditorGUILayout.EndHorizontal();

        m_Type = Type.GetType(m_ClassType.ToString());
        if (m_Type == null)
        {
            m_Type = Type.GetType(typeStr);
        }
        if (m_Type != null)
        {
            m_Class = EditorGUILayout.ObjectField(m_Class, m_Type);

            if (m_Class != null)
            {
                var curType = m_Type;
                do
                {
                    EditorGUILayout.Space(20);
                    EditorGUILayout.LabelField($" ---- {curType} ----- ");

                    var fields = curType.GetFields(
                            BindingFlags.IgnoreCase
                            | BindingFlags.DeclaredOnly
                            | BindingFlags.Instance
                            | BindingFlags.Static
                            | BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.FlattenHierarchy
                            | BindingFlags.InvokeMethod
                            | BindingFlags.CreateInstance
                            | BindingFlags.GetField
                            | BindingFlags.SetField
                            | BindingFlags.GetProperty
                            | BindingFlags.SetProperty
                            | BindingFlags.PutDispProperty
                            | BindingFlags.PutRefDispProperty
                            | BindingFlags.ExactBinding
                            | BindingFlags.SuppressChangeType
                            | BindingFlags.OptionalParamBinding
                            | BindingFlags.IgnoreReturn
                            | BindingFlags.DoNotWrapExceptions
                        );

                    foreach (var field in fields)
                    {
                        var value = field.GetValue(m_Class);

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField(field.Name, GUILayout.Width(100));
                        EditorGUILayout.LabelField($"{value}", GUILayout.Width(100));


                        EditorGUILayout.EndHorizontal();
                    }

                    curType = curType.BaseType;

                } while (curType != null);


            }
        }




        EditorGUILayout.EndScrollView();
    }
}

public class MessagingSystemWindow : EditorWindow
{
    EventSystemMgr m_Target = null;


    private void OnInspectorUpdate()
    {
        Repaint();
    }




    Vector2 m_ScrollPoint;
    Vector2 ViewSize => position.size;



    private void OnGUI()
    {
        var target = EventSystemMgr.Ins;
        if (target == null) return;

        var field = typeof(EventSystemMgr).GetField("m_DicEvent", BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null) return;
        var m_DicEvent = field.GetValue(target) as Dictionary<EEventSystemType, Dictionary<IEventSystem, (object tUserdata, string tDesc)>>;


        m_ScrollPoint = EditorGUILayout.BeginScrollView(m_ScrollPoint, GUILayout.Width(ViewSize.x), GUILayout.Height(ViewSize.y)); // 1 - 1



        EditorGUILayout.BeginHorizontal();
        foreach (var item in m_DicEvent)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.EnumPopup(item.Key);

            foreach (var obj in item.Value)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"\tType:", GUILayout.Width(150));
                EditorGUILayout.TextField($"{obj.Key}");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"\tUserData:", GUILayout.Width(150));
                EditorGUILayout.TextField($"{obj.Value.tUserdata}");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"\tDesc:", GUILayout.Width(150));
                EditorGUILayout.TextField($"{obj.Value.tDesc}");
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.EndHorizontal();




        EditorGUILayout.EndScrollView(); // 1
    }
}




public class QuadTreeWindow : EditorWindow
{
    public string AssetPath => QuadTreeMgr.Instance.AssetPath;

    public QuadTreeMgr m_Target = QuadTreeMgr.Instance;


    public QuadTreeData m_Data = null;
    Dictionary<int, QuadTreeNode> TreeNodeList => m_Data?.GetData();
    List<int> ShowTreeNodeList = new();
    Dictionary<int, RectTransform> ObjList = new();
    Dictionary<int, TempData> AreaData = new();
    QuadTreeNode RootNode => m_Data?.RootNode;


    Texture2D m_DefaultImg = null;
    Texture2D m_ActiveImg = null;

    RectTransform m_RootTarget = null;
    bool m_IsDebug = false;


    RawImage m_TestImg = null;
    private void OnEnable()
    {
        m_DefaultImg = (Texture2D)AssetDatabase.LoadMainAssetAtPath(@"Assets\Resources\Art\UIMap\newworld/newworld_0.png");
        m_ActiveImg = (Texture2D)AssetDatabase.LoadMainAssetAtPath(@"Assets\Resources\Art\UIMap\battle/battle_0.png");


    }

    private void OnDisable()
    {
        StopDebug();

        QuadTreeMgr.Instance.StopRender();

        SaveEditorData();

    }
    private void OnInspectorUpdate()
    {
        Repaint();
    }

    string m_SaveName = "";
    int m_NodeIndex = 0;
    int m_NodeIndex2 = 0;
    bool m_Statue = false;



    Vector2 m_ScrollPos1;
    Vector2 m_ScrollPos2;
    Vector2 m_ScrollPos3;


    private void OnGUI()
    {
        #region 顶部按钮部分

        EditorGUILayout.BeginHorizontal();
        {
            m_SaveName = EditorGUILayout.TextField(m_SaveName, GUILayout.Width(200));
            if (GUILayout.Button("Save", GUILayout.Width(150)))
            {
                StopDebug();
                if (m_Data == null)
                {
                    if (!Directory.Exists(AssetPath))
                    {
                        Directory.CreateDirectory(AssetPath);
                    }
                    var name = object.Equals(m_SaveName, "") ? typeof(QuadTreeData).ToString() : m_SaveName;
                    var path = $"{AssetPath}{name}.asset";
                    if (!File.Exists(path))
                    {
                        m_Data = ScriptableObject.CreateInstance<QuadTreeData>();
                        AssetDatabase.CreateAsset(m_Data, path);
                    }
                    else
                    {
                        Debug.LogError($"当前路径下文件已经存在   file name = {name}");
                    }
                }
                m_Data?.SaveData(TreeNodeList);
                SaveEditorData();
            }
            m_Data = EditorGUILayout.ObjectField(m_Data, typeof(QuadTreeData), true, GUILayout.Width(200)) as QuadTreeData;

            m_NodeIndex = EditorGUILayout.IntField(m_NodeIndex, GUILayout.Width(50));
            m_NodeIndex2 = EditorGUILayout.IntField(m_NodeIndex2, GUILayout.Width(50));
            if (GUILayout.Button("Add Node", GUILayout.Width(150)))
            {
                StopDebug();
                for (int i = m_NodeIndex; i < Mathf.Max(m_NodeIndex2, m_NodeIndex + 1); i++)
                {
                    m_Data?.AddTreeNode(i);
                }

                SaveEditorData();
            }

            if (GUILayout.Button("Remove Node", GUILayout.Width(150)))
            {
                StopDebug();
                m_Data?.RemoveTreeNode(m_NodeIndex);
                SaveEditorData();
            }

            m_RootTarget = EditorGUILayout.ObjectField(m_RootTarget, typeof(RectTransform), true, GUILayout.Width(200)) as RectTransform;
            if (GUILayout.Button($"Debug - {m_IsDebug}", GUILayout.Width(150)) && m_RootTarget != null)
            {
                if (m_IsDebug)
                {
                    StopDebug();
                    QuadTreeMgr.Instance.StopRender();
                }
                else
                {
                    StartDebug();
                }
            }
        }
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal();
        {
            bool curQuadTreeMgr = QuadTreeMgr.Instance != null;
            EditorGUILayout.ObjectField(QuadTreeMgr.Instance.AssetData, typeof(QuadTreeData), true, GUILayout.Width(200));
            EditorGUILayout.ObjectField(QuadTreeMgr.Instance.Mat_Range, typeof(Material), true, GUILayout.Width(200));
            if (GUILayout.Button("Init QuadTreeMgr", GUILayout.Width(150))
                && m_Data != null
                && curQuadTreeMgr)
            {
                m_Target.StartExecute($"{m_Data.name}");
                UpdateAreaData();
            }
            if (GUILayout.Button("Update Node", GUILayout.Width(150)) && curQuadTreeMgr)
            {
                foreach (var item in AreaData)
                {
                    foreach (var data in item.Value.Data)
                    {
                        m_Target.SetNodeStatus(data.Index, data.IsActive);
                    }
                }
            }

            if (GUILayout.Button("Update Effect", GUILayout.Width(150)) && curQuadTreeMgr)
            {
                m_Target.UpdateAllNodeEffect();
            }

            if (GUILayout.Button("Rt Test", GUILayout.Width(150)) && curQuadTreeMgr && m_RootTarget != null)
            {
                var size = m_RootTarget.parent.GetComponent<RectTransform>().rect.size;
                m_Target.StartRender((int)size.x, (int)size.y);
            }

            //m_TestImg = EditorGUILayout.ObjectField(m_TestImg, typeof(RawImage), true, GUILayout.Width(150)) as RawImage;
            //if (m_TestImg != null)
            //{
            //    var rtTexture = m_Target.IsShow();
            //    m_TestImg.texture = rtTexture;
            //}

        }
        EditorGUILayout.EndHorizontal();

        #endregion

        if (m_Data == null || QuadTreeMgr.Instance.TreeNodeList == null) return;



        #region 下方节点部分
        EditorGUILayout.BeginHorizontal();
        {
            #region 区域部分

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    void SetAllIsUnFold(bool f_Status)
                    {
                        foreach (var item in AreaData)
                        {
                            item.Value.IsUnFold = f_Status;
                        }
                    }
                    EditorGUILayout.LabelField("Area:", GUILayout.Width(50));
                    if (GUILayout.Button("All", GUILayout.Width(70)))
                    {
                        SetAllIsUnFold(true);
                    }
                    if (GUILayout.Button("Clear", GUILayout.Width(70)))
                    {
                        SetAllIsUnFold(false);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    void SetAllIsUnFold(bool f_Status)
                    {
                        foreach (var item in AreaData)
                        {
                            foreach (var data in item.Value.Data)
                            {
                                data.IsActive = f_Status;
                            }
                        }
                    }
                    EditorGUILayout.LabelField("Active:", GUILayout.Width(50));
                    if (GUILayout.Button("All", GUILayout.Width(70)))
                    {
                        SetAllIsUnFold(true);
                    }
                    if (GUILayout.Button("Clear", GUILayout.Width(70)))
                    {
                        SetAllIsUnFold(false);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    void SetAllIsShowParent(bool f_Status)
                    {
                        foreach (var item in AreaData)
                        {
                            foreach (var data in item.Value.Data)
                            {
                                data.IsShowParent = f_Status;
                            }
                        }
                    }
                    EditorGUILayout.LabelField("Parnet:", GUILayout.Width(50));
                    if (GUILayout.Button("All", GUILayout.Width(70)))
                    {
                        SetAllIsShowParent(true);
                    }
                    if (GUILayout.Button("Clear", GUILayout.Width(70)))
                    {
                        SetAllIsShowParent(false);
                    }
                }
                EditorGUILayout.EndHorizontal();

                m_ScrollPos3 = EditorGUILayout.BeginScrollView(m_ScrollPos3, GUILayout.Width(200), GUILayout.Height(position.height * 0.8f));
                {
                    EditorGUILayout.BeginVertical();
                    {
                        foreach (var item in AreaData)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField($"区域: [{item.Key}]", GUILayout.Width(50));
                                item.Value.IsUnFold = EditorGUILayout.Toggle(item.Value.IsUnFold, GUILayout.Width(20));
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            {
                                if (item.Value.IsUnFold)
                                {
                                    EditorGUILayout.LabelField("", GUILayout.Width(20));
                                    EditorGUILayout.BeginVertical();
                                    {
                                        foreach (var data in item.Value.Data)
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            {
                                                EditorGUILayout.LabelField($"ID:", GUILayout.Width(20));
                                                EditorGUILayout.LabelField($"{data.Index}", GUILayout.Width(30));

                                                EditorGUILayout.LabelField($"Act:", GUILayout.Width(25));
                                                data.IsActive = EditorGUILayout.Toggle(data.IsActive, GUILayout.Width(30));

                                                EditorGUILayout.LabelField($"Par:", GUILayout.Width(25));
                                                data.IsShowParent = EditorGUILayout.Toggle(data.IsShowParent, GUILayout.Width(25));
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space(20);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();

            }
            EditorGUILayout.EndVertical();

            #endregion

            ShowTreeNodeList.Clear();
            foreach (var item in AreaData)
            {
                if (item.Value.IsUnFold)
                {
                    foreach (var data in item.Value.Data)
                    {
                        if (!ShowTreeNodeList.Contains(data.Index))
                        {
                            ShowTreeNodeList.Add(data.Index);
                        }
                        if (data.IsShowParent)
                        {
                            var parNode = QuadTreeMgr.Instance.GetParentNode(data.Index);

                            while (!object.ReferenceEquals(parNode, null))
                            {
                                if (!ShowTreeNodeList.Contains(parNode.Index))
                                {
                                    ShowTreeNodeList.Add(parNode.Index);
                                }
                                parNode = QuadTreeMgr.Instance.GetParentNode(parNode.Index);
                            }
                        }
                    }
                }
            }





            #region 节点部分

            EditorGUILayout.BeginVertical();
            {

                m_ScrollPos2 = EditorGUILayout.BeginScrollView(m_ScrollPos2, GUILayout.Width(position.width * 0.85f), GUILayout.Height(position.height * 0.95f));
                {
                    NodeBox(RootNode);
                    ShowTreeNodes(new() { RootNode });
                }
                EditorGUILayout.EndScrollView();

            }
            EditorGUILayout.EndVertical();
            #endregion
        }
        EditorGUILayout.EndHorizontal();
        #endregion



        UpdateDebug();
    }

    // 显示节点
    void ShowTreeNodes(List<QuadTreeNode> f_ParentNode)
    {
        List<QuadTreeNode> nodeList = new();

        if (f_ParentNode.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            foreach (var nodes in f_ParentNode)
            {
                var list = nodes.GetLeafNodes();
                foreach (var item in list)
                {
                    var tempItem = item;
                    var value = tempItem.Value;

                    var node = QuadTreeMgr.Instance.GetNode(value);

                    if (ShowTreeNodeList.Contains(node.Index))
                    {
                        NodeBox(node);
                    }


                    EditorGUILayout.Space(2);
                    nodeList.Add(node);
                }
                EditorGUILayout.Space(20);
            }
            EditorGUILayout.EndHorizontal();

            ShowTreeNodes(nodeList);
        }
    }

    // 节点
    void NodeBox(QuadTreeNode f_Node)
    {

        var leafNodes = QuadTreeMgr.Instance.GetLeafNodeIndex(f_Node.Index);
        var parent = QuadTreeMgr.Instance.GetParentNode(f_Node.Index);


        GUIContent guiContect = new();
        guiContect.image = f_Node.IsEffect ? m_ActiveImg : f_Node.IsActive ? m_DefaultImg : null;

        guiContect.text = $"{f_Node.Index} " +
            $"\n {parent?.Index} ({leafNodes[0]} - {leafNodes[m_Data.TreeNum - 1]})" +
            $"\n {f_Node.IsActive}" +
            $"\n {f_Node.IsEffect}" +
            $"\n {f_Node.Position}" +
            $"\n {QuadTreeMgr.Instance.IsShow(new Vector2Int((int)f_Node.Position.x, (int)f_Node.Position.y))}";


        GUIStyle style = GUIStyle.none;

        style.normal.textColor = Color.red;


        EditorGUILayout.BeginVertical();
        {
            GUILayout.Box(guiContect, GUILayout.Width(300), GUILayout.Height(150));
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label($"Area ID: ", GUILayout.Width(50));
                f_Node.AreaID = EditorGUILayout.IntField(f_Node.AreaID, GUILayout.Width(150));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label($"Radius: ", GUILayout.Width(50));
                f_Node.Radius = EditorGUILayout.Slider(f_Node.Radius, 0, 1, GUILayout.Width(150));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label($"Area: ", GUILayout.Width(50));
                f_Node.AreaBlend = EditorGUILayout.Slider(f_Node.AreaBlend * 10, 0, 1, GUILayout.Width(150)) / 10;
            }
            EditorGUILayout.EndHorizontal();


            if (ObjList.TryGetValue(f_Node.Index, out var value))
            {
                EditorGUILayout.ObjectField(value, typeof(RectTransform), true, GUILayout.Width(200));
            }
        }
        EditorGUILayout.EndVertical();


    }


    // 保存数据
    private void SaveEditorData()
    {
        if (m_Data == null) return;
        EditorUtility.SetDirty(m_Data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // 开始调试
    private void StartDebug()
    {
        m_IsDebug = true;
        foreach (var item in TreeNodeList)
        {
            var rect = GameObject.Instantiate(m_RootTarget, m_RootTarget.parent);
            rect.name = $"{item.Key}";
            rect.anchoredPosition3D = item.Value.Position;
            ObjList.Add(item.Key, rect);
        }

        EStartDebug();
    }
    private async void EStartDebug()
    {
        var canvasSizeDelta = m_RootTarget.parent.GetComponent<RectTransform>().rect.size;
        while (m_IsDebug)
        {
            await UniTask.Delay(300);

            Dictionary<int, QuadTreeNode> f_NodeList = new();
            foreach (var item in TreeNodeList)
            {
                if (ShowTreeNodeList.Contains(item.Value.Index))
                {
                    f_NodeList.Add(item.Key, item.Value);
                }
            }
            QuadTreeMgr.Instance.UpdateRangeData(f_NodeList, 4096, 4096);
        }
    }
    // 结束调试
    private void StopDebug()
    {
        m_IsDebug = false;
        foreach (var item in ObjList)
        {
            GameObject.DestroyImmediate(item.Value.gameObject);
        }

        ObjList.Clear();
    }
    // 更新调试
    private void UpdateDebug()
    {
        if (!m_IsDebug) return;

        foreach (var item in ObjList)
        {
            TreeNodeList[item.Key].Position = item.Value.anchoredPosition3D;
        }
    }

    private void UpdateAreaData()
    {
        AreaData.Clear();
        // 初始化区域数据
        foreach (var item in TreeNodeList)
        {
            if (item.Value.GetLeafNodes().Count > 0) continue;
            if (!AreaData.ContainsKey(item.Value.AreaID))
            {
                AreaData.Add(item.Value.AreaID, new()
                {
                    IsUnFold = true,
                    Data = new(),
                });
            }

            AreaData[item.Value.AreaID].Data.Add(new()
            {
                Index = item.Key,
                IsShowParent = true,
                IsActive = item.Value.IsActive,
            });
        }
    }
}
class TempData
{
    public bool IsUnFold = false;
    public List<NodeData> Data = new();
}
class NodeData
{
    public int Index;
    public bool IsShowParent;
    public bool IsActive;
}