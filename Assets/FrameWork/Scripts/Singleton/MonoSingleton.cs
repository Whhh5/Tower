using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace B1
{
    public abstract class MonoSingleton<T> : MonoBase
        where T : MonoSingleton<T>
    {
        public static T Ins = null;
        protected override void Awake()
        {
            Ins = Ins == null ? (T)this : Ins;
        }
    }
}