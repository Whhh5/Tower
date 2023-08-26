using System;
using B1;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public enum EAnimatorStatus
{
    None,
    One,
    Loop,
    Stop,
}
public abstract class TransformBase : MonoBase
{
    public Vector3 Position => transform.position;

    public Vector3 LocalPostion => transform.localPosition;

    public Vector3 Forward => transform.forward;

    public Vector3 Up => transform.up;

    public Vector3 Right => transform.right;

    public Vector3 LocalRotation => transform.localRotation.eulerAngles;
    public Quaternion Rotation => transform.rotation;


    public virtual void SetWorldPos(Vector3 f_WorldPos)
    {
        transform.position = f_WorldPos;
    }

    public virtual void SetLocalPos(Vector3 f_LocalPos)
    {
        transform.localPosition = f_LocalPos;
    }

    public virtual void SetForward(Vector3 f_Direction)
    {
        transform.forward = f_Direction;
    }

    public virtual void SetUp(Vector3 f_Direction)
    {
        transform.up = f_Direction;
    }

    public virtual void SetRight(Vector3 f_Direction)
    {
        transform.right = f_Direction;
    }

    public virtual void SetLocalRotation(Vector3 f_Angle)
    {
        transform.localRotation = Quaternion.Euler(f_Angle);
    }

}
