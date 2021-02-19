using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace VentPlugin
{
    public class Patches
    {

        public static IDictionary<byte , Vent> playerVent = new Dictionary<byte , Vent>();

        public static IDictionary<byte, Button> playerButton = new Dictionary<byte, Button>();
        
        public static Il2CppArrayBase<Vent> vents;
        
        public static void onPressed()
        {
            System.Console.WriteLine("よばれ");
            
            if (!playerVent.ContainsKey(PlayerControl.LocalPlayer.PlayerId) /*|| !playerButton.ContainsKey(PlayerControl.LocalPlayer.PlayerId*/)
            {
                return;
            }
            Button.Use(playerVent[PlayerControl.LocalPlayer.PlayerId]);
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        static class ButtonPatch
        {
            static void Postfix(HudManager __instance)
            {
                Button button = new Button(__instance);
                button.AddListener(onPressed);
            }
        }
        
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
        static class ShipstatusOnEnablePatch
        {
            static void Prefix(ShipStatus __instance)
            {
                var vents = GameObject.FindObjectsOfType<Vent>();

                Patches.vents = vents;
            }
        }
        
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        class HandleThePLaceButton
        {
            static void Prefix(PlayerControl __instance)
            {
                if (PlayerControl.LocalPlayer != __instance) return;
                
            }
        }
        
    }
}