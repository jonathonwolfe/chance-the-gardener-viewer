using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainPage : MonoBehaviour
{
    public Dropdown UserID;

    DirectoryInfo dir;
    DirectoryInfo[] info;
    private void Awake()
    {
        dir = new DirectoryInfo(SaveSystem.Renders);
        info = dir.GetDirectories();
        PopulateDropdown();
        UserID.onValueChanged.AddListener((UnityEngine.Events.UnityAction<int>)delegate { ChangeUserID(); });
        ChangeUserID();
    }

    void PopulateDropdown()
    {
        List<string> options = new List<string>();
        foreach (DirectoryInfo f in info)
        {
            options.Add(f.Name);
        }
        UserID.ClearOptions();
        UserID.AddOptions(options);
        options.Clear();
    }

    public void ChangeUserID()
    {
        SaveSystem.UserID = UserID.options[UserID.value].text;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
