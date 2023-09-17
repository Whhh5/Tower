using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TestAnimtor : MonoBehaviour
{
    Animator Anima => GetComponent<Animator>();
    [SerializeField]
    AnimatorOverrideController m_CurAnimaOver = null;
    [Button]
    public void Test1()
    {
        if (m_CurAnimaOver == null)
        {
            m_CurAnimaOver = new AnimatorOverrideController();
            m_CurAnimaOver.runtimeAnimatorController = Anima.runtimeAnimatorController;
            Anima.runtimeAnimatorController = m_CurAnimaOver;
        }
        var count = m_CurAnimaOver.overridesCount;
        var list = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        m_CurAnimaOver.GetOverrides(list);
        string logStr = $"animator override count: {count} \n\t\t";
        foreach (var item in list)
        {
            logStr += $"{item.Key.name}->{item.Value?.name}  ----  ";
        }
        logStr += "\n\t\t";
        foreach (var item in m_CurAnimaOver.animationClips)
        {
            logStr += $"{item.name}  ----  ";
        }
        logStr += "\n\t\t";

        var animaClip = Resources.Load<AnimationClip>("Test/Test_Bottom");
        list.Add(new(animaClip, null));
        m_CurAnimaOver["Test_Left"] = animaClip;


        m_CurAnimaOver.GetOverrides(list);
        foreach (var item in list)
        {
            logStr += $"{item.Key.name}->{item.Value?.name}  ----  ";
        }
        logStr += "\n\t\t";
        foreach (var item in m_CurAnimaOver.animationClips)
        {
            logStr += $"{item.name}  ----  ";
        }
        logStr += "\n\t\t";


        Debug.Log(logStr);
    }
    [Button]
    public void Test_Left()
    {
        var leftClip = m_CurAnimaOver["Test_Left"];
        var rightClip = m_CurAnimaOver["Test_Right"];
        var upClip = m_CurAnimaOver["Test_Up"];
        var bottomClip = m_CurAnimaOver["Test_Botton"];
        Debug.Log($"animator override count = {m_CurAnimaOver.overridesCount}, left:{leftClip.name}, right{rightClip.name}, up:{upClip.name}, buttom:{bottomClip?.name}");
    }
}
