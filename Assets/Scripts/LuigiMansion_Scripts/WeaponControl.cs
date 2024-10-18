using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class WeaponControl : MonoBehaviour
{
    public delegate void AttackingGhost(bool attacking);
    public AttackingGhost attackingGhost;

    public delegate void SetPlayerRotation(bool activeRotate);
    public SetPlayerRotation setPlayerRotate;

    [Header("Transform Settings")]
    public Transform playerTransform;
    public Transform suckingPoint;

    [Header("Light Settings")]
    public GameObject flashLight;
    public Renderer lightMeshRenderer;
    public float minLight;
    public float maxLight;

    [Header("Tornado Settings")]
    public GameObject tornado;

    [Header("Damage to Ghost Settings")]
    public float doDamageEverySecond = 0.35f;

    private MyPlayer controls;
    private List<Ghost> listOfGhost;

    private bool isSucking = false;
    //private bool isBlowing = false;

    private Material lightMat;

    private void Start()
    {
        SwitchWeapon(false);
        listOfGhost = new List<Ghost>();
        listOfGhost.Clear();
    }

    public void Init(AttackingGhost attackingGhostCallback, SetPlayerRotation setPlayerRotateCallback)
    {
        attackingGhost = attackingGhostCallback;
        setPlayerRotate = setPlayerRotateCallback;
    }

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new MyPlayer();
            controls.Player.Transform.performed += OnFlash; //able to use OnTransform by PlayerInout component for reading input instead of subscibe;
            controls.Player.Fire.performed += OnSucking;
            controls.Player.Fire.canceled += OnStopSucking;
        }

        controls.Player.Enable();
        lightMat = lightMeshRenderer.material;
    }

    private void OnDisable()
    {
        listOfGhost.Clear();
        controls.Player.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSucking)
        {
            Ghost enteredGhost = other.GetComponent<Ghost>();
            if (enteredGhost)
            {
                if (enteredGhost.GetHP() > 0 && !listOfGhost.Contains(enteredGhost))
                {
                    listOfGhost.Add(enteredGhost);
                    enteredGhost.ghostJustGotScuked += GhostGotSucked;
                    enteredGhost.ghostDead += DestroyGhost;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Ghost exitedGhost = other.GetComponent<Ghost>();
        if (exitedGhost)
        {
            if (listOfGhost.Contains(exitedGhost))
            {
                exitedGhost.ghostDead -= DestroyGhost;
                exitedGhost.ghostJustGotScuked -= GhostGotSucked;
                listOfGhost.Remove(exitedGhost);
            }
        }
    }

    // You can use both this method or OnFlash method, same result;
    //private void OnTransform(InputValue value)
    //{
    //    foreach (Ghost ghost in listOfGhost)
    //    {
    //        ghost.SetBeingAttack(true);
    //    }
    //}

    private void OnFlash(InputAction.CallbackContext callback)
    {
        DOVirtual.Float(minLight, maxLight, 0.2f, SetLightflash)
            .OnComplete(ResetLight);

        GeneralInstance.instance.ShakeCamera();

        foreach (Ghost ghost in listOfGhost)
        {
            ghost.Stunned(true);
        }
    }

    private void SetLightflash(float lightValue)
    {
        lightMat.SetFloat("Opacity", lightValue);
    }

    private void ResetLight()
    {
        DOVirtual.Float(maxLight, minLight, 1.2f, SetLightflash);
    }

    private void OnSucking(InputAction.CallbackContext callback)
    {
        isSucking = true;
        setPlayerRotate?.Invoke(true);
        SwitchWeapon(true);

        foreach (Ghost ghost in listOfGhost)
        {
            ghost.SetBeingSuck(true, suckingPoint);
            if (ghost.isStunned)
            {
                ghost.transform.SetParent(playerTransform);
            }
        }
    }

    private void SwitchWeapon(bool suckingMode)
    {
        tornado.SetActive(suckingMode);
        flashLight.SetActive(!suckingMode);
    }

    private void OnStopSucking(InputAction.CallbackContext callback)
    {
        StopSucking();

        foreach (Ghost ghost in listOfGhost)
        {
            ghost.transform.SetParent(null);
            ghost.SetBeingSuck(false);
        }
    }

    private void StopSucking()
    {
        GhostGotSucked(false);
        isSucking = false;
        setPlayerRotate?.Invoke(false);
        SwitchWeapon(false);
    }

    private void DestroyGhost(Ghost ghost)
    {
        StopSucking();
        listOfGhost.Remove(ghost);
    }

    public void DoDamageToGhosts(float angle)
    {
        for (int i = 0; i < listOfGhost.Count; i++)
        {
            if (!listOfGhost[i].IsDead)
                listOfGhost[i].TakeDamage(angle);
        }
    }

    private void GhostGotSucked(bool setAttack)
    {
        attackingGhost?.Invoke(setAttack);
    }
}
