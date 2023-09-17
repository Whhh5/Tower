using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;

namespace B1
{
    public class MonoBase : MonoBehaviour, ILog
    {
        public void Log<T>(T message) => Debug.Log($" 【 {GetType()} 】\n{message}");
        public void LogWarning<T>(T message) => Debug.LogWarning($" 【 {GetType()} 】\n{message}");
        public void LogError<T>(T message) => Debug.LogError($" 【 {GetType()} 】\n{message}");

        public async UniTask DelayAsync(int f_DelayTime = 0)
        {
            await UniTask.Delay(f_DelayTime);
        }


        private Dictionary<EEventSystemType, string> m_MsgDic = null;
        protected virtual void Awake()
        {
            Awake_Message();
        }
        protected virtual void Update()
        {

        }
        protected virtual void Start()
        {

        }
        protected virtual void OnDestroy()
        {
            Destroy_Message();
        }

        #region 消息系统处理
        private void Awake_Message()
        {
            //消息接口处理
            var eventSystem = this as IEventSystem;
            if (eventSystem != null)
            {
                m_MsgDic = SubscribeList();
                foreach (var item in m_MsgDic)
                {
                    var tempItem = item;

                        EventSystemMgr.Ins.Subscribe(tempItem.Key, eventSystem);
                    
                }
            }
        }
        private void Destroy_Message()
        {
            //消息接口处理
            if (!object.ReferenceEquals(m_MsgDic, null))
            {
                var eventSystem = this as IEventSystem;
                foreach (var item in m_MsgDic)
                {
                    var tempItem = item;
                    EventSystemMgr.Ins.Unsubscribe(tempItem.Key, eventSystem);
                }
            }
        } 
        public virtual Dictionary<EEventSystemType, string> SubscribeList()
        {
            return m_MsgDic;
        }
        #endregion
    }
}
