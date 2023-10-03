using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInfo
{
    private int highscore;
    private string nickname;
    private bool sfxOn;
    private bool hideNickname;

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
            hideNickname = _sgi.hideNickname;
        }
        else
        {
            highscore = 0;
            nickname = "";
            sfxOn = true;
            hideNickname = false;
        }
    }

    public void Salvar()
    {
        SaveGameInfo _sgi = new SaveGameInfo();
        _sgi.highscore = highscore;
        _sgi.nickname = nickname;
        _sgi.sfxOn = sfxOn;
        _sgi.hideNickname = hideNickname;

        SaveGame.Salvar(_sgi);
    }

    public void ResetarSave()
    {
        PlayerPrefs.DeleteAll();

        highscore = 0;
        nickname = "";
        sfxOn = true;
        hideNickname = false;

        Salvar();
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

    public void SetHideNick(bool _vl)
    {
        hideNickname = _vl;
    }

    public bool GetHideNick()
    {
        return hideNickname;
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
