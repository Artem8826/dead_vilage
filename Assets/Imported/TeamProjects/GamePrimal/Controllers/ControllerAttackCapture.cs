﻿using Assets.GamePrimal.Controllers;
using Assets.GamePrimal.Mono;
using Assets.TeamProjects.GamePrimal.Helpers.InterfaceHold;
using Assets.TeamProjects.GamePrimal.Proxies;
using Assets.TeamProjects.GamePrimal.SeparateComponents.MiscClasses;
using UnityEngine;

namespace Assets.TeamProjects.GamePrimal.Controllers
{
    public struct AttackCaptureParams
    {
        public Transform Source;
        public Transform Target;
        public bool HasHit;
        public bool HasDied;
        public Vector3 Touch;
    }

    public class ControllerAttackCapture
    {
        public bool HitFixated { get; private set; } = false;
        public bool HasHit { get; private set; } = false;

        private MainScene.MainScene _theMainScene;
        private ControllerFocusSubject _cFocusSubject;

        public void Start()
        {
            _theMainScene = Object.FindObjectOfType<MainScene.MainScene>();
            _cFocusSubject = StaticProxyRouter.GetControllerFocusSubject();
        }

        public void ReleaseFixated() => HasHit = false;
        public void LockTarget() => HasHit = true;

        public void Update()
        {
            HitFixated = false;
            Transform focused = _cFocusSubject.GetHardFocus();
            Transform captured = _cFocusSubject.GetSoftFocus();
            Vector3 touchPoint = _cFocusSubject.GetTouchPoint();

            if (!StaticProxyStateHolder.UserOnUi)
                if (Input.GetKeyDown(KeyCode.Mouse0) && focused && captured && captured.GetComponent<MonoMechanicus>())
                    if (focused.GetInstanceID() != captured.GetInstanceID())
                    {
                        MonoMechanicus focusedMonomech = focused.GetComponent<MonoMechanicus>();
                        MonoMechanicus capturedMonomech = captured.GetComponent<MonoMechanicus>();

                        if (focusedMonomech.IsBlueTeam == capturedMonomech.IsBlueTeam)
                            return;

                        AttackCaptureParams acp = new AttackCaptureParams()
                        {
                            Source = captured, Target = focused, HasHit = HasHit, Touch = touchPoint
                        };

                        StaticProxyRouter.GetControllerEvent().HitDetectedInvoke(acp);
    //                    Debug.Log("Locked " + Time.time);
                        HitFixated = true;

                        StaticProxyStateHolder.LockModeOn = false;
                    }
        }
    }
}
