using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Il2CppSystem;
using Reactor;

namespace VentPlugin
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]

    public class VentPlugin : BasePlugin
    {
        public const string Id = "com.tomatan515.ventPlugin";
        public Harmony Harmony { get; } = new Harmony(Id);
        public ConfigEntry<string> Name { get; private set; }

        public override void Load()
        {
            Name = Config.Bind("Fake", "Name", ":>");

            Harmony.PatchAll();
        }
    }

    public static class PlayerVentTimeExtension
    {
        //byte -> ID 
        //DateTime -> 世界共通時
        public static IDictionary<byte, DateTime> allVentTimes = new Dictionary<byte, DateTime>() { };

        public static void SetLastVent(byte player)
        {
            if (allVentTimes.ContainsKey(player))
                allVentTimes[player] = DateTime.UtcNow;
            else
                allVentTimes.Add(player, DateTime.UtcNow);
        }

        public static DateTime GetLastVent(byte player)
        {
            if (allVentTimes.ContainsKey(player))
                return allVentTimes[player];
            else
                return new DateTime(0);
        }
    }
    
    /**
     *     [HarmonyPatch(typeof(Vent), "CanUse")]
    public static class VentPatch
    {
        public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
        {
            float num = float.MaxValue;
            PlayerControl localPlayer = pc.Object;
            
            couldUse = !localPlayer.Data.IsDead;
            
            canUse = couldUse;
            if ((DateTime.UtcNow - PlayerVentTimeExtension.GetLastVent(pc.Object.PlayerId)).TotalMilliseconds > 1000)
            {
                num = Vector2.Distance(localPlayer.GetTruePosition(), __instance.transform.position);
                canUse &= num <= __instance.UsableDistance;
            }
            __result = num;
            return false;
        }
    }
     */
}












/**public static IDictionary<byte, DateTime> allVentTimes = new Dictionary<byte, DateTime>() { };
/**public static void SetLastVent(byte player)
{
    if (allVentTimes.ContainsKey(player))
        allVentTimes[player] = DateTime.UtcNow;
    else
        allVentTimes.Add(player, DateTime.UtcNow);
}

public static DateTime GetLastVent(byte player)
{
    if (allVentTimes.ContainsKey(player))
        return allVentTimes[player];
    else
        return new DateTime(0);
}

/**
*         [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
private static class VentPatch
{
    public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo pc,
        [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)

    {
        float num = float.MaxValue;
        PlayerControl localPlayer = pc.Object;

        couldUse = !localPlayer.Data.IsDead;

        canUse = couldUse;
        if ((DateTime.UtcNow - GetLastVent(pc.Object.PlayerId)).TotalMilliseconds > 1000)
        {
            num = Vector2.Distance(localPlayer.GetTruePosition(), __instance.transform.position);
            canUse &= num <= __instance.UsableDistance;
        }

        __result = num;
        
        return false;
    }
}
*/