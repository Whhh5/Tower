using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using Cysharp.Threading.Tasks;

public interface ISkillUtil
{

}
public class SkillMgr : Singleton<SkillMgr>
{

}




public abstract class PersonSkillBaseData
{
    protected Entity_HeroBaseData HeroBaseData = null;
    public abstract EPersonSkillType PersonType { get; }
    public void Initialization(Entity_HeroBaseData f_HeroData)
    {
        HeroBaseData = f_HeroData;
    }
    public virtual void StartExecute()
    {

    }
    public virtual void StopExecute()
    {

    }
}
public class PersonSkillData_Hero1_Stage1_Default1 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage1_Default1;
    public Entity_Player_Hero1Data Hero1Data => HeroBaseData as Entity_Player_Hero1Data;
    public override void StartExecute()
    {
        base.StartExecute();

        Hero1Data.SwordLow.SetPenrtrate(true);
    }
}
public class PersonSkillData_Hero1_Stage1_Default2 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage1_Default2;
}
public class PersonSkillData_Hero1_Stage1_Default3 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage1_Default3;
}
public class PersonSkillData_Hero1_Stage1_Default4 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage1_Default4;
}
public class PersonSkillData_Hero1_Stage2_Default1_Loss1 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage2_Default1_Loss1;
    public Entity_Player_Hero1Data Hero1Data => HeroBaseData as Entity_Player_Hero1Data;
    public override void StartExecute()
    {
        base.StartExecute();
        Hero1Data.SwordLow.SetTargetStop(true);
    }
}
public class PersonSkillData_Hero1_Stage2_Default1_Loss2 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage2_Default1_Loss2;
}
public class PersonSkillData_Hero1_Stage2_Default1_Loss3 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage2_Default1_Loss3;
}
public class PersonSkillData_Hero1_Stage2_Default2_Loss1 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage2_Default2_Loss1;
}
public class PersonSkillData_Hero1_Stage2_Default2_Loss2 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage2_Default2_Loss2;
}
public class PersonSkillData_Hero1_Stage2_Default3_Loss1 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage2_Default3_Loss1;
}
public class PersonSkillData_Hero1_Stage3_Default1_Loss1_Height1 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage3_Default1_Loss1_Height1;
    public Entity_Player_Hero1Data Hero1Data => HeroBaseData as Entity_Player_Hero1Data;
    public override void StartExecute()
    {
        base.StartExecute();
        Hero1Data.SwordLow.SetPenrtrate(false);
        Hero1Data.SwordLow.SetTargetStop(false);
        Hero1Data.StopExecuteSword();
    }
}
public class PersonSkillData_Hero1_Stage3_Default1_Loss1_Height2 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage3_Default1_Loss1_Height2;
}
public class PersonSkillData_Hero1_Stage3_Default1_Loss1_Height3 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage3_Default1_Loss1_Height3;
}
public class PersonSkillData_Hero1_Stage3_Default1_Loss2_Height1 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage3_Default1_Loss2_Height1;
}
public class PersonSkillData_Hero1_Stage3_Default1_Loss2_Height2 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage3_Default1_Loss2_Height2;
}
public class PersonSkillData_Hero1_Stage3_Default1_Loss3_Height1 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage3_Default1_Loss3_Height1;
}
public class PersonSkillData_Hero1_Stage3_Default2_Loss1_Height1 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage3_Default2_Loss1_Height1;
}
public class PersonSkillData_Hero1_Stage3_Default2_Loss1_Height2 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage3_Default2_Loss1_Height2;
}
public class PersonSkillData_Hero1_Stage3_Default2_Loss2_Height1 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage3_Default2_Loss2_Height1;
}
public class PersonSkillData_Hero1_Stage3_Default3_Loss1_Height1 : PersonSkillBaseData
{
    public override EPersonSkillType PersonType => EPersonSkillType.Stage3_Default3_Loss1_Height1;
}