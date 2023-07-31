using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Player_Default2Data : Person_EnemyData
{
    public Entity_Player_Default2Data(int f_Index, int f_TargetIndex, Entity_SpawnPointPlayerData f_TargetSpawnPoint) : base(f_Index, f_TargetIndex, f_TargetSpawnPoint)
    {
    }
    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Default2;

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;

    protected override int AtkRange => base.AtkRange + 1;

    private Entity_Player_Default2 Target => GetCom<Entity_Player_Default2>();

    private int m_EffectDataIndex = int.MinValue;
    private ListStack<EffectMoveData> m_DicEffect = new("", 20);
    private float m_speed = 5.0f;

    public override void AfterLoad()
    {
        base.AfterLoad();

    }
    public override void AnimatorCallback070()
    {
        base.AnimatorCallback070();

        switch (CurStatus)
        {
            case EPersonStatusType.Attack:
                {
                    var curEffKey = m_EffectDataIndex++;
                    var startPos = Target != null && Target.AttackPoint != null ? Target.AttackPoint.position : WorldPosition;
                    var effData = new TestTimeLineData(curEffKey, this, UnityEngine.Playables.DirectorWrapMode.Loop);

                    var target = m_CurTarget;

                    var moveData = new EffectMoveData(effData, startPos, m_speed, () =>
                    {
                        Vector3? value = target != null && target.CurStatus != EPersonStatusType.Die
                            ? target.CentralPoint
                            : null;
                        return value;
                    }, (targetPos) =>
                    {
                        if (Vector3.Magnitude(effData.WorldPosition - targetPos) > 0.001f)
                        {
                            return true;
                        }

                        effData.EntityDamage(target, -HarmBase);
                        return false;
                    });
                    m_DicEffect.Push(moveData);
                }
                break;
            default:
                break;
        }
    }
    public override string GetCurrentAnimationName()
    {
        var name = base.GetCurrentAnimationName();

        switch (CurStatus)
        {
            case EPersonStatusType.Attack:
                break;
            case EPersonStatusType.Skill:
                break;
            default:
                break;
        }
        return name;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        var loopIndex = 0;
        while (loopIndex < m_DicEffect.Count)
        {
            var item = m_DicEffect[loopIndex];
            item.UpdateOnEnable();
            if (item.OnEnable == false)
            {
                m_DicEffect.Remove(item);
                item.UnLoad();
                return;
            }
            loopIndex++;
            item.UpdatePosition();
        }
    }
    class EffectMoveData
    {
        public EffectMoveData(EntityEffectBaseData f_EffectData, Vector3 f_StartPosition, float f_UnitSpeed = 1, Func<Vector3?> f_GetPosition = null, Func<Vector3, bool> f_GetOnEnable = null)
        {
            Index = f_EffectData.Index;
            OnEnable = true;
            EffectData = f_EffectData;
            StartPosition = f_StartPosition;
            GetPosition = f_GetPosition;
            GetOnEnable = f_GetOnEnable;
            UnitSpeed = f_UnitSpeed;
            f_EffectData.SetPosition(f_StartPosition);
            LoadAsync();
        }
        public int Index;
        public bool OnEnable = true;
        public EntityEffectBaseData EffectData = null;
        public Vector3 StartPosition = Vector3.zero;
        public Vector3 TargetPosition = Vector3.zero;
        public float UnitSpeed = 1.0f;
        public Func<Vector3?> GetPosition = null;
        public Func<Vector3, bool> GetOnEnable = null;

        public void UpdatePosition()
        {
            var value = GetPosition?.Invoke();
            TargetPosition = value != null ? value ?? Vector3.zero : StartPosition;


            Vector3 targetPos = Vector3.MoveTowards(EffectData.WorldPosition, TargetPosition, Time.deltaTime * UnitSpeed);
            EffectData.SetForward(targetPos - EffectData.WorldPosition);
            EffectData.SetPosition(targetPos);
        }
        public void UpdateOnEnable()
        {
            if (GetOnEnable == null)
            {
                OnEnable = Vector3.Magnitude(TargetPosition - EffectData.WorldPosition) > 0.001f;
            }
            else
            {
                OnEnable = GetOnEnable.Invoke(TargetPosition);
            }
        }
        public async void LoadAsync()
        {
             await ILoadPrefabAsync.LoadAsync(EffectData);
        }
        public void UnLoad()
        {
            ILoadPrefabAsync.UnLoad(EffectData);
        }
    }
}
public class Entity_Player_Default2 : Person_Enemy
{
    public Entity_Monster_Default1Data ObjectData => TryGetData<Entity_Monster_Default1Data>(out var data) ? data : null;
    public Transform AttackPoint = null;
}
