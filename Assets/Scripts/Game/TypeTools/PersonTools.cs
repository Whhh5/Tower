using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using B1;
using UnityEngine;
using UnityEngine.AI;

public enum EMoveDirection
{
    None = 0,
    Left = 1 << 0,
    Right = 1 << 1,
    Forward = 1 << 2,
    Back = 1 << 3,
}


public enum EPersonStatusType
{
    None,
    Idle,
    Walk,
    Attack,
    Skill,
    Die,
    Control,
}

public abstract class PersonData : EntityData
{
    public abstract ELayer LayerMask { get; }
    public abstract ELayer AttackLayerMask { get; }
    public int HarmBase { get; protected set; } = 12;
    public WeaponBaseData<WeaponBase> CurWeaponData { get; protected set; }

    protected PersonData(int f_index) : base(f_index)
    {
    }
    protected void PlayAnimation(bool f_IsForce = false)
    {

    }
    public void PlayerAnimation(EPersonStatusType f_StatusType)
    {
        var animaName = $"{AssetPrefabID}_{f_StatusType}";
    }
}


public abstract class Person : Entity, IButton3DClick
{
    [SerializeField]
    protected ELayer m_LayerMask = ELayer.Default;
    public ELayer LayerMask => m_LayerMask;
    [SerializeField]
    protected ELayer m_AttackLayerMask = ELayer.Default;
    public ELayer AttackLayerMask => m_AttackLayerMask;
    [SerializeField]
    protected uint m_HarmBase = 0;
    [SerializeField]
    protected WeaponBase m_CurWeapon = null;
    public NavMeshAgent NavMeshAgent => GetComponent<NavMeshAgent>();
    public Animator Animator => GetComponent<Animator>();


    // Gain
    protected Dictionary<EGainType, Dictionary<ushort, GainBase>> m_CurGainList = new();

    protected override void Awake()
    {
        base.Awake();

        var button3D = GetComponent<Button3D>();

        button3D?.AddListener(this);
    }

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
        await GTools.ParallelTaskAsync((ushort)EGainType.EnumCount, async (index) =>
        {
            m_CurGainList.Add((EGainType)index, new());
        });
    }
    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();

        m_CurGainList.Clear();
    }
    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);
    }

    public override async void OnUpdate()
    {
        base.OnUpdate();
        if (GTools.RefIsNull(m_CurWeapon)) return;
    }

    public override async UniTask StartExecute()
    {

    }

    public override async UniTask StopExecute()
    {

    }


    #region 状态回调

    public async UniTask OnClickAsync()
    {
        GTools.PlayerController.SetController(this);
    }

    public async UniTask OnClick2Async()
    {

    }

    #endregion

    public async UniTask SetWeapon(WeaponBase f_Weapon)
    {
        if (!GTools.RefIsNull(m_CurWeapon))
        {
            await GTools.WeaponMgr.DestroyWeaponAsync(m_CurWeapon);
        }
        m_CurWeapon = f_Weapon;
    }
    public async void OnClickDownKeyCodeD_Launch()
    {
        if (GTools.RefIsNull(m_CurWeapon)) return;
        await m_CurWeapon.StartExecute();

    }
    public async void OnClickDownKeyCodeF_Collect()
    {
        if (GTools.RefIsNull(m_CurWeapon)) return;
        await m_CurWeapon.StopExecute();
    }



    // ===============------      ------===============
    //                       Gain 增益
    // ===============------      ------===============
    // 触发增益
    public async UniTask ExecuteGainAsync(EGainType f_GainType)
    {
        await GTools.ParallelTaskAsync(m_CurGainList[f_GainType], async (key, value) =>
        {
            await value.StartExecute();
        });
    }

    public async UniTask AddGainAsync(ushort f_ID)
    {
        var gainType = GTools.GainMgr.GetGainType(f_ID);
        if (!m_CurGainList[gainType].ContainsKey(f_ID))
        {
            var ins = GTools.GainMgr.GetGain(f_ID, this);
            m_CurGainList[gainType].Add(f_ID, ins);
        }
    }
    public async UniTask RemoveGainAsync(ushort f_ID)
    {
        var gainType = GTools.GainMgr.GetGainType(f_ID);
        if (m_CurGainList[gainType].ContainsKey(f_ID))
        {
            m_CurGainList[gainType].Remove(f_ID);
        }
    }
    public async UniTask SetGainLevelAsync(ushort f_ID, ushort f_Level)
    {

    }
    public async UniTask AddGainTierAsync(ushort f_ID)
    {

    }
    public async UniTask RediusGainTierAsync(ushort f_ID)
    {

    }
}
