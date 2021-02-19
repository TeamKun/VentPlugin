using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;

namespace VentPlugin
{
    public class Button
    {
        private static List<Button> AllButtons = new List<Button>();
        
        private readonly HudManager _hudManager;
        private Action _onClick;
        public KillButtonManager buttonManager;
        public float MaxTimer = 1;

        private float timer;

        public Vent _vent;

        public Button(HudManager hudManager)
        {
            _hudManager = hudManager;

            AllButtons.Add(this);
            OnStart();
            
            //Patches.playerButton.Add(PlayerControl.LocalPlayer.PlayerId , this);
            
            buttonManager.renderer.material.SetFloat("_Desat", 0f);
            buttonManager.gameObject.SetActive(true);
        }
        
        void OnUpdate()
        {
            if (PlayerControl.LocalPlayer.Data.IsImpostor)
            {
                buttonManager.gameObject.SetActive(false);
            }
            else
            {
                buttonManager.gameObject.SetActive(true);
            }

            if (timer > 0)
            {
                timer -= Time.deltaTime;
                buttonManager.SetCoolDown(timer, MaxTimer);
            }
            else
            {
                SetTarget();
            }

            buttonManager.transform.localPosition = new Vector3((_hudManager.UseButton.transform.localPosition.x) * -1, _hudManager.UseButton.transform.localPosition.y, _hudManager.KillButton.transform.localPosition.z) + new Vector3(0.2f, 0.2f);

            foreach (Vent vent in Patches.vents)
            {
                if (vent.UsableDistance  >=
                    Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), vent.transform.position))
                {
                    _vent = vent;
                }
                else if (vent.UsableDistance < Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(),
                    vent.transform.position) && _vent == vent)
                {
                    _vent = null;
                }

                if (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), vent.transform.position) < vent.UsableDistance * 2)
                {
                    vent.SetOutline(true , true);
                }
                else
                {
                    vent.SetOutline(false , false);
                }
            }

            if (_vent != null)
            {
                buttonManager.renderer.color = new Color(1f, 1f, 1f, 1f);
                buttonManager.renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                buttonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                buttonManager.renderer.material.SetFloat("_Desat", 1f);
            }
            
            float num = 0;
            //buttonManager.renderer.color = new Color(1f, 1f, 1f, 1f);

            if (_vent == null) return;

            if (!CanUse(_vent))
            {
                Patches.playerVent.Remove(PlayerControl.LocalPlayer.PlayerId);
            }
            else if (!Patches.playerVent.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
            {
                Patches.playerVent.Add(PlayerControl.LocalPlayer.PlayerId, _vent);
            }
            else
            {
                Patches.playerVent.Remove(PlayerControl.LocalPlayer.PlayerId);
                Patches.playerVent.Add(PlayerControl.LocalPlayer.PlayerId, _vent);
            }

            if (CanUse(_vent) && !PlayerControl.LocalPlayer.Data.IsImpostor)
            {
                buttonManager.renderer.color = Palette.EnabledColor;
                buttonManager.renderer.material.SetFloat("_Desat", 0f);
                buttonManager.enabled = true;
            }
            else
            {
                buttonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                buttonManager.renderer.material.SetFloat("_Desat", 1f);
                buttonManager.enabled = false;
            }
            
        }

        void SetTarget()
        {
            
        }

        void OnStart()
        {
            buttonManager = GameObject.Instantiate(_hudManager.KillButton, _hudManager.transform);
            buttonManager.renderer.sprite = _hudManager.UseButton.VentImage;
            
            buttonManager.SetCoolDown(timer , MaxTimer);
            timer = MaxTimer;

            buttonManager.renderer.SetCooldownNormalizedUvs();

            buttonManager.transform.localPosition = new Vector3((buttonManager.transform.localPosition.x + 1.3f) * -1, buttonManager.transform.localPosition.y, buttonManager.transform.localPosition.z);
        }

        bool CanUse(Vent vent)
        {
            if (vent == null) return false;
            
            float num = float.MaxValue;

            bool canUse = !PlayerControl.LocalPlayer.Data.IsDead;
            num = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), vent.transform.position);
            canUse &= num <= vent.UsableDistance;

            return canUse && (timer < 0);
        }

        public void AddListener(Action action)
        {
            _onClick += action;
            buttonManager.GetComponent<PassiveButton>().OnClick.AddListener((UnityAction) OnClick);
            void OnClick()
            {
                if (CanUse(_vent))
                {

                    if (!Patches.playerVent.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
                    {
                        return;
                    }
                    Use(Patches.playerVent[PlayerControl.LocalPlayer.PlayerId]);
                    
                    timer = MaxTimer; 
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        class HudManagerPatch
        {
            static void Prefix()
            {
                try
                {

                    AllButtons.RemoveAll(item => item.buttonManager == null);

                    foreach (var button in AllButtons)
                    {
                        button.OnUpdate();
                    }
                }
                catch
                {
                    
                }
            }
        }

        public static void Use(Vent vent)
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer.inVent)
            {
                localPlayer.MyPhysics.RpcExitVent(vent.Id);
                vent.SetButtons(false);
                return;
            }

            localPlayer.MyPhysics.RpcEnterVent(vent.Id);
            vent.SetButtons(true);
        }
    }
}