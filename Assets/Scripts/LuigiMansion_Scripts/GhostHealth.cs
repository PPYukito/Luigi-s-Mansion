using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GhostHealth : MonoBehaviour
{
    public TMP_Text hpText;

    public void SetHPText(string hp)
    {
        hpText.text = hp;
    }

    public void ResetColor()
    {
        hpText.color = Color.white;
    }

    public void FadeHP()
    {
        hpText.DOColor(new Color(255, 255, 255, 0), 1.0f);
    }
}
