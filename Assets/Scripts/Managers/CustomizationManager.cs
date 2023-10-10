using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image img_preview;
    [SerializeField] private Text lb_select;
    [SerializeField] private Button btn_select;
    [SerializeField] private Text lb_skinName;
    [SerializeField] private Text lb_skinDesc;
    [SerializeField] private Text lb_skinCount;
    [SerializeField] private Text lb_coinsCount;

    [Header("Misc assignables")]
    [SerializeField] private SO_Cosmetics cosmetics;

    int previewSkin = 0;
    int selectedSkin = 0;

    private void Start()
    {
        selectedSkin = SaveInfo.GetInstance().GetSelectedSkin();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.C)) return;

        SaveInfo _saveInstance = SaveInfo.GetInstance();
        _saveInstance.AddCoins(100);
        _saveInstance.Salvar();
        UpdatePreview();
    }

    public void SelectSkin()
    {
        SaveInfo _saveInfo = SaveInfo.GetInstance();

        // fazer a parte dos dinheiros aqui
        if (!_saveInfo.GetBoughtSkins().Contains(previewSkin))
        {
            int _skinPrice = cosmetics.GetSkinInfo(previewSkin).cost;
            if (_skinPrice > _saveInfo.GetCoins())
                return;

            _saveInfo.AddToBoughtSkins(previewSkin);
            _saveInfo.SpendCoins(_skinPrice);
        }

        selectedSkin = previewSkin;
        _saveInfo.SetSelectedSkin(selectedSkin);
        UpdatePreview();
    }


    public void UpdatePreview(bool _force = false)
    {
        if (previewSkin >= cosmetics.GetSkinsLength())
            return;

        SaveInfo _saveInfo = SaveInfo.GetInstance();

        if (_force)
        {
            selectedSkin = _saveInfo.GetSelectedSkin();
            previewSkin = selectedSkin;
        }

        SkinInfo _skin = cosmetics.GetSkinInfo(previewSkin);
        int _coins = _saveInfo.GetCoins();

        img_preview.sprite = _skin.preview;
        lb_skinName.text = $"{_skin.name}";
        lb_skinDesc.text = $"{_skin.description}";
        lb_skinCount.text = $"{previewSkin + 1}/{cosmetics.GetSkinsLength()}";
        lb_coinsCount.text = $"{_coins} c";

        if (_saveInfo.GetBoughtSkins().Contains(previewSkin))
        {
            if (selectedSkin == previewSkin)
            {
                lb_select.text = "SELECIONADO";
                btn_select.interactable = false;
                lb_select.color = Color.black;
                return;
            }

            // caso não seja
            lb_select.text = "SELECIONAR";
            btn_select.interactable = true;
            lb_select.color = Color.black;
        }
        else
        {
            lb_select.text = $"COMPRAR\n{_skin.cost} c";

            if (_skin.cost > _coins)
            {
                btn_select.interactable = false;
                lb_select.color = Color.white;

                return;
            }

            // caso não seja
            btn_select.interactable = true;
            lb_select.color = Color.black;
        }
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
