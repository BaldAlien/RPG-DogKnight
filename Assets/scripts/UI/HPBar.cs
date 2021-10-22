using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public GameObject HPUIPrefab;
    public Transform HPBarPoint;
    public bool alwaysVisible;
    private float visibleTime = 5f;
    private float lastVisTime;

    private Image HPSlider;
    private Transform UIBar;
    Transform Camera;

    CharacterStats Stats;

    void Awake()
    {
        Stats = GetComponent<CharacterStats>();
        Stats.UpdateHPBarOnAttack += UpdateHPBar;
    }

    void OnEnable()
    {
        Camera = UnityEngine.Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIBar = Instantiate(HPUIPrefab, canvas.transform).transform;
                HPSlider = UIBar.GetChild(0).GetComponent<Image>();
                UIBar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    void UpdateHPBar(int currentHP, int maxHP)
    {
        if (currentHP <= 0)
            Destroy(UIBar.gameObject);
        UIBar.gameObject.SetActive(true);
        lastVisTime = visibleTime;
        float sliderPercent = (float)currentHP / maxHP;
        HPSlider.fillAmount = sliderPercent;
    }

    void LateUpdate()
    {
        if (UIBar != null)
        {
            UIBar.position = HPBarPoint.position;
            UIBar.forward = -Camera.forward;

            if (lastVisTime <= 0 && !alwaysVisible)
            {
                UIBar.gameObject.SetActive(false);
            }
            else
            {
                lastVisTime -= Time.deltaTime;
            }
        }
    }
}
