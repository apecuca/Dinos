using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveGameInfo
{
    public int highscore;
    public string nickname;
    public bool sfxOn;
    public bool hideNickname;
}
public class SaveGame
{
    public static void Salvar(SaveGameInfo save)
    {
        string saveString = JsonUtility.ToJson(save);
        PlayerPrefs.SetString("save", saveString);
        PlayerPrefs.Save();
    }

    public static bool TemSave()
    {
        return PlayerPrefs.HasKey("save");
    }

    public static SaveGameInfo Carregar()
    {
        string saveString = PlayerPrefs.GetString("save");
        SaveGameInfo save = JsonUtility.FromJson<SaveGameInfo>(saveString);

        return save;
    }
}
