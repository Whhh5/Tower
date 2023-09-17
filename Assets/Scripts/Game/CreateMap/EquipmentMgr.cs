using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;


public class EquipmentMgr : Singleton<EquipmentMgr>
{

}




public abstract class EquipmentBaseData
{
    public abstract EEquipmentType EquipmentType { get; }
    public WorldObjectBaseData TargetWorldObject = null;
    public void Initialization(WorldObjectBaseData f_WorldObj)
    {
        TargetWorldObject = f_WorldObj;
    }
    public virtual void StartExecute()
    {

    }
    public virtual void StopExecute()
    {

    }
}
public class EquipmentData_Default1 : EquipmentBaseData
{
    public override EEquipmentType EquipmentType => EEquipmentType.EquipDefault1;
}
public class EquipmentData_Default2 : EquipmentBaseData
{
    public override EEquipmentType EquipmentType => EEquipmentType.EquipDefault2;
}
public class EquipmentData_Default3 : EquipmentBaseData
{
    public override EEquipmentType EquipmentType => EEquipmentType.EquipDefault3;
}
public class EquipmentData_Default4 : EquipmentBaseData
{
    public override EEquipmentType EquipmentType => EEquipmentType.EquipDefault4;
}