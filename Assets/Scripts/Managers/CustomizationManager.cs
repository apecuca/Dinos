using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationManager : MonoBehaviour
{
    [SerializeField] private Image img_preview;
    [SerializeField] private Text lb_select;
    [SerializeField] private SO_Cosmetics cosmetics;

    int previewSkin = 0;
    int selectedSkin = 0;

    private void Start()
    {
        selectedSkin = SaveInfo.GetInstance().GetSelectedSkin();
    }

    public void SelectSkin()
    {
        selectedSkin = previewSkin;
        SaveInfo.GetInstance().SetSelectedSkin(selectedSkin);
    }


    public void UpdatePreview(bool _force = false)
    {
        if (previewSkin >= cosmetics.GetSkinsLength())
            return;

        if (_force)
        {
            selectedSkin = SaveInfo.GetInstance().GetSelectedSkin();
            previewSkin = selectedSkin;
        }

        SkinInfo _skin = cosmetics.GetSkinInfo(previewSkin);
        img_preview.sprite = _skin.preview;
        lb_select.text = $"{_skin.cost} COINS\n{_skin.name}";
    }

    public void ChangePreviewSkin(int _dir)
    {
        // 1 >
        // -1 <

        previewSkin += _dir;

        int _length = cosmetics.GetSkinsLength();
        if (previewSkin < 0) previewSkin = _length - 1;
        if (previewSkin >= _length) previewSkin = 0;

        UpdatePreview();
    }


}
