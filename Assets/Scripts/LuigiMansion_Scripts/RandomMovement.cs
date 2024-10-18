using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RandomMovement : MonoBehaviour
{
    //[Header("Controllers")]
    //public CharacterController CharacterController;

    //[Header("Movement Settings")]
    //public float capturingSpeed = 3.0f;

    [Header("Random Rotate Settings")]
    public float rotateSpeed = 10;
    public AnimationCurve lerpEase = default;
    public float yRot;

    private bool right;
    private bool capturing = false;
    //private Vector3 escapeAxis;
    //private MyPlayer actionInput;

    //private void Start()
    //{
    //    actionInput = new MyPlayer();
    //    actionInput.Enable();
    //}

    //private void OnEnable()
    //{
    //    if (actionInput == null)
    //        actionInput = new MyPlayer();
    //    actionInput.Enable();
    //}

    //private void OnDisable()
    //{
    //    actionInput.Disable();
    //}

    //private void OnDrawGizmos()
    //{
    //    Debug.DrawRay(transform.position, escapeAxis, Color.yellow);
    //}

    public void StartRandomMovement()
    {
        capturing = true;
        StartCoroutine(RotateTo());
        StartCoroutine(ChooseDir());
    }

    private void Update()
    {
        //if (capturing)
        //{
        //    CharacterController.Move(transform.forward * capturingSpeed * Time.deltaTime);

        //    angle calculation
        //    float x = Input.GetAxis("Horizontal");
        //    float z = Input.GetAxis("Vertical");
        //    Debug.Log($"H: {x}, V: {z}");
        //}

        // BOTH ways can get -1 to 1 value but the result is too Digital, I can not get the value between them. SO GetAxis is the BEST way for now;
        //float horizontal = actionInput.Player.Horizontal.ReadValue<float>();
        //float vertical = actionInput.Player.Vertical.ReadValue<float>();
        //Debug.Log($"Horizontal: {horizontal}, Vertical: {vertical}");

        //Vector2 move = actionInput.Player.Move.ReadValue<Vector2>();
        //Debug.Log($"Vector X: {move.x}, Vector Y: {move.y}");
    }

    public void StopRandomMovement()
    {
        capturing = false;
        StopCoroutine("RotateTo");
        StopCoroutine("ChooseDir");
        StopAllCoroutines();
    }

    IEnumerator RotateTo()
    {
        yRot += Random.Range(15, 45) * (right ? 1 : -1);
        float distance = Mathf.Abs(Mathf.DeltaAngle(transform.localEulerAngles.y, yRot));

        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(0, yRot, 0);

        float animateTime = 0;
        float animationLength = distance / rotateSpeed;

        while (animateTime < animationLength)
        {
            animateTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, endRot, lerpEase.Evaluate(animateTime / (animationLength)));
            yield return null;
        }

        StartCoroutine(RotateTo());
    }

    IEnumerator ChooseDir()
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        right = (Random.value > 0.5f);
        StartCoroutine(ChooseDir());
    }
}
