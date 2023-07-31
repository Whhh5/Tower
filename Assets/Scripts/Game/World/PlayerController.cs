using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using B1;
using Cysharp.Threading.Tasks;

public class PlayerController : MonoSingleton<PlayerController>
{
    public Person CurController { get; private set; }
    public NavMeshAgent CurNavMeshAgent => CurController?.NavMeshAgent;
    public Animator CurAnimator => CurController?.Animator;




    public KeyCode LastDownKeyCode = KeyCode.None;
    public KeyCode CurDownKeyCode = KeyCode.None;
    public float LastDownKeyCodeTime = 0;
    public float MoveInterval = 1;
    public float SpeedBase = 3.5f;
    public bool IsRun;
    private void Update()
    {
        if (GTools.RefIsNull(CurController)) return;


        var curWorldPos = CurController.Position;
        var toWorldPos = curWorldPos;
        #region ÒÆ¶¯²¿·Ö
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = GTools.MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, (int)ELayer.Terrain))
            {
                CurNavMeshAgent.SetDestination(hit.point);
            }
        }

        //CurAnimator.getik

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            KeyCodeDown(KeyCode.LeftArrow);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            KeyCodeDown(KeyCode.RightArrow);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            KeyCodeDown(KeyCode.UpArrow);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            KeyCodeDown(KeyCode.DownArrow);
        }


        CurDownKeyCode = KeyCode.None;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            toWorldPos += new Vector3(-MoveInterval, 0, 0);
            CurDownKeyCode = KeyCode.LeftArrow;
            CurNavMeshAgent.SetDestination(toWorldPos);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            toWorldPos += new Vector3(MoveInterval, 0, 0);
            CurDownKeyCode = KeyCode.RightArrow;
            CurNavMeshAgent.SetDestination(toWorldPos);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            toWorldPos += new Vector3(0, 0, MoveInterval);
            CurDownKeyCode = KeyCode.UpArrow;
            CurNavMeshAgent.SetDestination(toWorldPos);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            toWorldPos += new Vector3(0, 0, -MoveInterval);
            CurDownKeyCode = KeyCode.DownArrow;
            CurNavMeshAgent.SetDestination(toWorldPos);
        }



        void KeyCodeDown(KeyCode f_CurKeyCode)
        {
            CurNavMeshAgent.speed = SpeedBase;
            
            if (Mathf.Abs(GTools.CurTime - LastDownKeyCodeTime) < 0.5f)
            {
                CurNavMeshAgent.speed = SpeedBase * 2;
            }
            LastDownKeyCode = f_CurKeyCode;
            LastDownKeyCodeTime = GTools.CurTime;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            CurNavMeshAgent?.ResetPath();
        }
        #endregion


        if (Input.GetKeyDown(KeyCode.D))
        {
            OnButtonLaunch();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnButtonColltion();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CurController?.AddGainAsync(1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            CurController?.RemoveGainAsync(1);
        }
    }



    public void SetController(Person f_Person)
    {
        CurController = f_Person;
    }
    public async UniTask SetWeapon(WeaponBase f_Weapon)
    {
        await CurController.SetWeapon(f_Weapon);
    }

    public void OnButtonLaunch()
    {
        CurController?.OnClickDownKeyCodeD_Launch();
    }
    public void OnButtonColltion()
    {
        CurController?.OnClickDownKeyCodeF_Collect();
    }

}
