using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ShuffleBag
{
    private HashSet<string> all = new HashSet<string>();
    private HashSet<string> seen = new HashSet<string>();

    public void LoadPossibilities(string path)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }
        all = new HashSet<string>(File.ReadAllLines(path).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public void LoadSeen(string path)
    {
        if (File.Exists(path))
        {
            seen = new HashSet<string>(File.ReadAllLines(path).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)));
        }
        else
        {
            seen = new HashSet<string>();
        }
    }

    public void SaveSeen(string path)
    {
        File.WriteAllLines(path, seen);
    }

    public void ClearSeen()
    {
        seen.Clear();
    }

    public HashSet<string> Unseen
    {
        get
        {
            var unseen = new HashSet<string>(all);
            unseen.ExceptWith(seen);
            return unseen;
        }
    }

    public string Take()
    {
        if (all.Count == 0)
        {
            return "Gracie";
        }

        var unseen = Unseen;
        if (unseen.Count == 0)
        {
            Debug.Log("Clearing seen names...");
            seen.Clear();
            unseen = Unseen;
        }

        var random = new System.Random();
        var array = unseen.ToArray();
        var result = array[random.Next(array.Length)];

        seen.Add(result);
        return result;
    }

    public int Count
    {
        get
        {
            return all.Count;
        }
    }
}
