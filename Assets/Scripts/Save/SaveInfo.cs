using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInfo
{
    private int highscore;
    private string nickname;
    private bool sfxOn;

    private static SaveInfo instance;

    public static SaveInfo GetInstance()
    {
        if (instance == null)
        {
            instance = new SaveInfo();
        }

        return instance;
    }

    public SaveInfo()
    {
        if (SaveGame.TemSave())
        {
            SaveGameInfo _sgi = SaveGame.Carregar();
            highscore = _sgi.highscore;
            nickname = _sgi.nickname;
            sfxOn = _sgi.sfxOn;
        }
        else
        {
            highscore = 0;
            nickname = "";
            sfxOn = true;
        }
    }

    public void Salvar()
    {
        SaveGameInfo _sgi = new SaveGameInfo();
        _sgi.highscore = highscore;
        _sgi.nickname = nickname;
        _sgi.sfxOn = sfxOn;

        SaveGame.Salvar(_sgi);
    }

    public void ResetarSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }


    #region GETTERS E SETTERS

    public void SetHighscore(int _vl)
    {
        highscore = _vl;
    }

    public int GetHighscore()
    {
        return highscore;
    }

    public void SetNickname(string _vl)
    {
        nickname = _vl;
    }

    public string GetNickname()
    {
        return nickname;
    }

    public void SetSfxOn(bool _vl)
    {
        sfxOn = _vl;
    }

    public bool GetSfxOn()
    {
        return sfxOn;
    }

    #endregion

    #region MISC

    public bool IsNicknameValid()
    {
        if (nickname != null &&
            nickname != "" &&
            nickname != " ")
            return true;

        return false;
    }


    #endregion
}
