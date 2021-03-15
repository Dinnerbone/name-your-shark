using HarmonyLib;

[HarmonyPatch(typeof(PlayerStats), "Damage")]
public static class PlayerStats_Damage_Patch
{
    static void Postfix(Network_Player ___playerNetwork, PlayerStats __instance)
    {
        if (Semih_Network.IsHost && __instance.IsDead)
        {
            if (NameYourSharkMod.Instance.SharkCurrentlyAttacking != null)
            {
                RAPI.BroadcastChatMessage($"{___playerNetwork.characterSettings.Name} was eaten by {NameYourSharkMod.Instance.SharkCurrentlyAttacking}");
            }
        }
    }
}