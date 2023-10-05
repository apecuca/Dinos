using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image img_preview;
    [SerializeField] private Text lb_select;
    [SerializeField] private Text lb_skinName;
    [SerializeField] private Text lb_skinDesc;
    [SerializeField] private Text lb_skinCount;

    [Header("Misc assignables")]
    [SerializeField] private SO_Cosmetics cosmetics;

    int previewSkin = 0;
    int selectedSkin = 0;

    private void Start()
    {
        selectedSkin = SaveInfo.GetInstance().GetSelectedSkin();
    }

    public void SelectSkin()
    {
        // fazer a parte dos dinheiros aqui

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
        // mudar texto caso já tiver a skin
        lb_select.text = $"BUY\n{_skin.cost} c";
        lb_skinName.text = $"{_skin.name}";
        lb_skinDesc.text = $"{_skin.description}";
        lb_skinCount.text = $"{previewSkin + 1}/{cosmetics.GetSkinsLength()}";
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
