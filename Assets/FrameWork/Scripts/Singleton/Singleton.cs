using B1.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace B1
{
    public abstract class Singleton<T> : Base, IInitialization
        where T : new()
    {
        public static T Ins = new();

        public virtual void Awake()
        {
            
        }

        public virtual void Start()
        {

        }

        public virtual void Destroy()
        {

        }
    }
}