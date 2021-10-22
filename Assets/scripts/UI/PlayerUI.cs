using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    TMP_Text CurrentHP_Text, MaxHP_Text, Level_Text;
    Image HPSlider;

    void Awake()
    {
        Level_Text = transform.GetChild(1).GetComponent<TMP_Text>();
        CurrentHP_Text = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        MaxHP_Text = transform.GetChild(0).GetChild(3).GetComponent<TMP_Text>();
        HPSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    public void UpdatePlayerHPBar()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.currentHP / GameManager.Instance.playerStats.maxHP;
        HPSlider.fillAmount = sliderPercent;
        CurrentHP_Text.text = GameManager.Instance.playerStats.currentHP.ToString();
        MaxHP_Text.text = GameManager.Instance.playerStats.maxHP.ToString();
    }

    public void UpdateLevel()
    {
        Level_Text.text = "Lv." + GameManager.Instance.playerStats.characterData.currentLevel;
        UpdatePlayerHPBar();
    }
}
