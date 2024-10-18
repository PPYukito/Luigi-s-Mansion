using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Cinemachine;

public class GeneralInstance : MonoBehaviour
{
    public static GeneralInstance instance;

    [Header("Display Settings")]
    public RectTransform canvas;
    public GameObject ghostHPGameObj;

    [Header("Camera Settings")]
    public Camera cam;
    public CinemachineVirtualCamera cinemachineCam;
    public float shakeIntensity;
    public float shakeTime;

    [Header("Ghosts")]
    public List<Ghost> listOfGhost;

    //private List<Ghost>
    private Dictionary<Ghost, GhostHealth> ghostDisplays;
    private IDisposable dispose;
    private CinemachineBasicMultiChannelPerlin cineBasMulChanlPerl;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        instance = this;

        ghostDisplays = new Dictionary<Ghost, GhostHealth>();
        GenerateGhostHPComponent();

        cineBasMulChanlPerl = cinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        foreach (KeyValuePair<Ghost, GhostHealth> ghostData in ghostDisplays)
        {
            ghostData.Value.transform.position = cam.WorldToScreenPoint(ghostData.Key.transform.position);
        }
    }

    public void GenerateGhostHPComponent()
    {
        foreach (Ghost ghost in listOfGhost)
        {
            GhostHealth ghostHPComp = Instantiate(ghostHPGameObj, canvas).GetComponent<GhostHealth>();
            ghostDisplays.Add(ghost, ghostHPComp);
        }
    }

    public void ShakeCamera()
    {
        cineBasMulChanlPerl.m_AmplitudeGain = shakeIntensity;
        StartCoroutine(StopShake());
    }

    IEnumerator StopShake()
    {
        yield return new WaitForSeconds(shakeTime);
        cineBasMulChanlPerl.m_AmplitudeGain = 0;
    }

    public void ShowHP(Ghost ghost)
    {
        if (ghostDisplays.ContainsKey(ghost))
        {
            int hp = (int)ghost.GetHP();

            if (hp < 100)
                ghostDisplays[ghost].SetHPText(hp.ToString());

            if (hp <= 0)
            {
                GameObject hpObj = ghostDisplays[ghost].gameObject;
                ghostDisplays.Remove(ghost);
                Destroy(hpObj);
            }
        }
    }

    public void ResetGhostColor(Ghost ghost)
    {
        if (ghostDisplays.ContainsKey(ghost))
            ghostDisplays[ghost].ResetColor();
    }

    public void FadeGhostHP(Ghost ghost)
    {
        if (ghostDisplays.ContainsKey(ghost))
            ghostDisplays[ghost].FadeHP();
    }
}
