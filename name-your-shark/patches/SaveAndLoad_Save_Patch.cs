using HarmonyLib;
using System.IO;

[HarmonyPatch(typeof(SaveAndLoad), "Save")]
public static class SaveAndLoad_Save_Patch
{
    static void Postfix(string filename, RGD raftgamedata)
    {
        if (raftgamedata.Type == RGDType.World)
        {
            var directory = Directory.GetParent(filename);
            NameYourSharkMod.Instance.Names.SaveSeen(directory + "/seen_shark_names.txt");
        }
    }
}