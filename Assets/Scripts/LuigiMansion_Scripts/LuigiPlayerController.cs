using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuigiPlayerController : MonoBehaviour
{
    [Header("Controllers")]
    public CharacterController CharacterController;
    public WeaponControl WeaponControl;
    public RandomMovement RandomMovement;
    public MovementInput MovementInput;

    [Header("Movement Settings")]
    public float capturingSpeed = 3.0f;

    private bool attackingGhost = false;
    private Vector3 escapeAxis;

    private void Start()
    {
        attackingGhost = false;
        WeaponControl.Init(CapturingGhost, ActivePlayerRotation);
    }

    private void Update()
    {
        if (attackingGhost)
        {
            CharacterController.Move(transform.forward * capturingSpeed * Time.deltaTime);

            // angle calculation
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            //Debug.Log($"H: {x}, V: {z}");

            escapeAxis = new Vector3(x, 0, z);
            float escapeAngle = Vector3.Angle(transform.forward, escapeAxis);

            // tell weapon to do damage to ghost;
            WeaponControl.DoDamageToGhosts(escapeAngle);

            if (escapeAngle >= 130)
            {
                CharacterController.Move(escapeAxis * (capturingSpeed / 2) * Time.deltaTime);
            }
        }
    }

    private void CapturingGhost(bool attacking)
    {
        if (attackingGhost != attacking)
            attackingGhost = attacking;

        if (attacking)
            RandomMovement.StartRandomMovement();
        else
            RandomMovement.StopRandomMovement();
    }

    private void ActivePlayerRotation(bool activeRotate)
    {
        MovementInput.blockRotationPlayer = activeRotate;
    }
}
