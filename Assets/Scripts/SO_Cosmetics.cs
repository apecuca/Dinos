using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkinInfo
{
    public string name;
    public string description;
    public RuntimeAnimatorController anim;
    public Sprite preview;
    public int cost;
}

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Cosmetics data", order = 1)]
public class SO_Cosmetics : ScriptableObject
{
    [SerializeField] private SkinInfo[] skins;

    public int GetSkinsLength()
    {
        return skins.Length;
    }


    public SkinInfo GetSkinInfo(int _id)
    {
        if (_id >= skins.Length)
            return null;

        return skins[_id];
    }

}
