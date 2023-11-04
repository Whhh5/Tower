using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_EnergyCrystalBaseData : WorldObjectBaseData
{
    public override EWorldObjectType ObjectType => EWorldObjectType.Resource;
    public virtual EQualityType Quality => EQualityType.Quality1;

    public override EEntityType EntityType => EEntityType.EnergyCrystal;

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;
    public new Entity_EnergyCrystalBase EntityTarget => GetCom<Entity_EnergyCrystalBase>();

    public override void InitData(int f_ChunkIndex = -1)
    {
        base.InitData(f_ChunkIndex);
        SetActivateStatus(true);
    }
    public override void AfterLoad()
    {
        base.AfterLoad();
    }

    public override void Death()
    {
        MonsterEnter();
        base.Death();
    }

    public void MonsterEnter()
    {
        // 产生爆炸
        var effect = new Entity_EnergyCrystal1_Effect1Data();
        effect.SetPosition(WorldPosition);
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(effect));


        if (this.TryGetRandomEnemy(out var list))
        {
            foreach (var item in list)
            {
                this.EntityDamage(item);
            }
        }


        // 标记为不激活
        SetActivateStatus(false);
    }
    public void SetActivateStatus(bool f_Status)
    {
        SetMainColor(f_Status ? Color.white : new Color(1, 0, 0, 0.5f));

    }

    public Color MainColor = Color.white;
    public void SetMainColor(Color? f_Color = null)
    {
        MainColor = f_Color ?? Color.white;
        if (EntityTarget != null)
        {
            EntityTarget.SetMainColor();
        }
    }

}
public abstract class Entity_EnergyCrystalBase : WorldObjectBase
{
    public Entity_EnergyCrystalBaseData Data => GetData<Entity_EnergyCrystalBaseData>();
    [SerializeField]
    private SpriteRenderer m_MainSprite = null;
    public void SetMainColor()
    {
        m_MainSprite.color = Data.MainColor;
    }

}
