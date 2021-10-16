using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static readonly string Renders = Application.dataPath + "/FarmBotData" + "/renders/";

    public static string UserID;

    public static void Save(string saveString, string gardenID)
    {
        File.WriteAllText(Renders + "/" + UserID + "/" + gardenID + "/" + "save.txt", saveString);   
    }

    public static string Load(string gardenID)
    {
        if(File.Exists(Renders + "/" + UserID + "/" + gardenID + "/" + "save.txt"))
        {
            string saveString = File.ReadAllText(Renders + "/" + UserID + "/" + gardenID + "/" + "save.txt");
            return saveString;
        }
        else
        {
            return null;
        }
    }

    public static string Delete(string gardenID)
    {
        if (File.Exists(Renders + "/" + UserID + "/" + gardenID + "/" + "save.txt"))
        {
            File.Delete(Renders + "/" + UserID + "/" + gardenID + "/" + "save.txt");
            return "Deleted Save File";
        }
        return "No Save File Found";
    }
}
