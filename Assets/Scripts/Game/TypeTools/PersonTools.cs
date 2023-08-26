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
    Incubation, // 孵化
    Entrance, // 入场动画
    Idle,
    Walk,
    Attack,
    Skill,
    Die,
    Control,
}

