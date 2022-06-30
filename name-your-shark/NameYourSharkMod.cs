using UnityEngine;
using HarmonyLib;
using System.Collections;
using System.IO;
using TMPro;

public class NameYourSharkMod : Mod
{
    public static int CHANNEL_ID = 588;
    public static Messages MESSAGE_TYPE_SET_NAME = (Messages)524;
    public static NameYourSharkMod Instance;

    public string SharkCurrentlyAttacking;
    public ShuffleBag Names;

    private Harmony harmonyInstance;
    private AssetBundle assets;
    private bool inWorld;

    public IEnumerator Start()
    {
        Instance = this;

        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(GetEmbeddedFileBytes("name-your-shark.assets"));
        yield return request;
        assets = request.assetBundle;

        harmonyInstance = new Harmony("com.dinnerbone.nameyourshark");
        harmonyInstance.PatchAll();

        Names = new ShuffleBag();

        var namePath = SaveAndLoad.AppPath + "shark_names.txt";
        Names.LoadPossibilities(namePath);
        Debug.Log($"Loaded {Names.Count} possible shark names...");

        if (Names.Count == 0)
        {
            Debug.LogWarning($"Please put your possible shark names, one per line, in {namePath}");
        }
    }

    public void OnModUnload()
    {
        harmonyInstance.UnpatchAll("com.dinnerbone.nameyourshark");
        assets.Unload(true);
        Instance = null;
    }

    private string GetLatestSaveDirectoryPath(string folderPath)
    {
        if (!Directory.Exists(folderPath)) return null;

        var directories = Directory.GetDirectories(folderPath);
        for (var i = 0; i < directories.Length; i++)
        {
            if (directories[i].EndsWith("-Latest"))
            {
                return directories[i];
            }
        }
        return null;
    }

    private void EnsureWorldLoaded()
    {
        if (inWorld) return;

        var worldDir = SaveAndLoad.WorldPath + SaveAndLoad.CurrentGameFileName;
        var latest = GetLatestSaveDirectoryPath(worldDir);
        Debug.Log("worldDir: " + worldDir);
        Debug.Log("latest: " + latest);
        if (latest != null)
        {
            Debug.Log("Loaded already-seen shark names");
            Names.LoadSeen(latest + "/seen_shark_names.txt");
        }
        else
        {
            Debug.Log("Cleared already-seen shark names");
            Names.ClearSeen();
        }
        inWorld = true;
    }

    public override void WorldEvent_WorldLoaded()
    {
        EnsureWorldLoaded();
    }

    public override void WorldEvent_WorldUnloaded()
    {
        inWorld = false;
    }

    public void FixedUpdate()
    {
        var message = RAPI.ListenForNetworkMessagesOnChannel(CHANNEL_ID);
        if (message != null)
        {
            if (message.message.Type == MESSAGE_TYPE_SET_NAME && !Raft_Network.IsHost)
            {
                if (message.message is UpdateSharkNameMessage msg)
                {
                    var maybeShark = NetworkIDManager.GetNetworkIDFromObjectIndex<AI_NetworkBehaviour>(msg.sharkId);
                    if (maybeShark is AI_NetworkBehavior_Shark shark)
                    {
                        var nameTag = shark.stateMachineShark.GetComponentInChildren<TextMeshPro>();
                        nameTag.text = msg.name;
                    }
                }
            }
        }
    }

    public TextMeshPro AddNametag(AI_StateMachine_Shark shark)
    {
        var nameTag = Instantiate(assets.LoadAsset<GameObject>("Name Tag"));
        nameTag.AddComponent<Billboard>();

        nameTag.transform.SetParent(shark.transform);
        nameTag.transform.localPosition = new Vector3(0, 2f, 0);
        nameTag.transform.localRotation = Quaternion.identity;

        var text = nameTag.GetComponentInChildren<TextMeshPro>();

        if (Raft_Network.IsHost)
        {
            EnsureWorldLoaded();
            text.text = Names.Take();
        }

        return text;
    }
}