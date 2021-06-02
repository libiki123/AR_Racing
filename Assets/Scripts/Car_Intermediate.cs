using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class Car_Intermediate: MonoBehaviour
{   
    [SerializeField] private Transform frontWheels, backWheels;
    [SerializeField] private Rigidbody sphereRb;
    [SerializeField] private Transform kartModel,kartNormal;
    [SerializeField] private LayerMask Road;

    [Header("Movement")]
    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;
    public float airDrag;
    public float groundDrag;
    public float maxWheelTurn;

    //////////////////////////////
    
    private bool isGrounded;
    private bool isDrifting;
    private bool turnleft;
    private bool turnRight;
    private bool accelerate;

    private float vertialInput;
    private float horizontalInput;

    private float speed, currentSpeed;
    private float rotate, currentRotate;

    private int driftDirection;


    void Start()
    {
        sphereRb.transform.parent = null;
    }


    public void TurnLeftDown()
	{
        turnleft = true;
    }

    public void TurnLeftup()
    {
        turnleft = false;
    }

    public void TurnRightDown()
    {
        turnRight = true;
    }

    public void TurnRightUp()
    {
        turnRight = false;
    }

    public void AccelerateDown()
	{
        accelerate = true;
	}

    public void AccelerateUp()
    {
        accelerate = false;
    }

    void Update()
    {
        //horizontalInput = Input.GetAxisRaw("Horizontal");
        if (turnRight && turnleft) // both directions
            horizontalInput = 0;
        else if (turnRight)
            horizontalInput = 1;
        else if (turnleft)
            horizontalInput = -1;
        else
            horizontalInput = 0;

        transform.position = sphereRb.transform.position;       // update car to follow sphere

        //if (Input.GetButton("Fire1"))   //Accelerate
        if(accelerate)
        {              
            speed = fwdSpeed;
        }

        if (horizontalInput != 0)
        {
            int dir = horizontalInput > 0 ? 1 : -1;
            float amount = Mathf.Abs(horizontalInput);
            Steer(dir, amount);
        }

        

		//Drift
		if (Input.GetButtonDown("Jump") && !isDrifting && horizontalInput != 0)
		{
			isDrifting = true;
			driftDirection = horizontalInput > 0 ? 1 : -1;

            kartModel.parent.DOComplete();
            kartModel.parent.DOPunchPosition(Vector3.up * 0.2f, 0.3f, 5, 1);      // do a small hop
		}

		if (isDrifting)
		{
			float control = (driftDirection == 1) ? ExtensionMethods.Remap(horizontalInput, -1, 1, 0, 2) : ExtensionMethods.Remap(horizontalInput, -1, 1, 2, 0);
			float powerControl = (driftDirection == 1) ? ExtensionMethods.Remap(horizontalInput, -1, 1, .2f, 1) : ExtensionMethods.Remap(horizontalInput, -1, 1, 1, .2f);
			Steer(driftDirection, control);
			//driftPower += powerControl;

			//ColorDrift();
		}

		if (Input.GetButtonUp("Jump") && isDrifting)
		{
			Boost();
		}

		currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
		currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;


		if(!isDrifting)
        {
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 180 + (horizontalInput * 15), kartModel.localEulerAngles.z), .2f);
        }
        else
        {
			float control = (driftDirection == 1) ? ExtensionMethods.Remap(horizontalInput, -1, 1, .5f, 2) : ExtensionMethods.Remap(horizontalInput, -1, 1, 2, .5f);
            kartModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(kartModel.parent.localEulerAngles.y, (control * 15) * driftDirection, .2f), 0);
		}

        frontWheels.localEulerAngles = new Vector3(frontWheels.localEulerAngles.x, (horizontalInput * maxWheelTurn), 0);    // wheels turn
        frontWheels.localEulerAngles += new Vector3(sphereRb.velocity.magnitude / 2, 0, 0);                                 // wheels roll
        backWheels.localEulerAngles += new Vector3(sphereRb.velocity.magnitude / 2, 0, 0);

    }

	private void FixedUpdate()
	{
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, Road);

        sphereRb.drag = isGrounded ? groundDrag : airDrag;      // change drag if airbone

        if (isGrounded)
		{
			sphereRb.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
			transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 4f);   // smooth rotate
		}
		else
		{
			sphereRb.AddForce(transform.up * -20f);     // add gravity;
			Quaternion newRotation = Quaternion.Euler(20, transform.eulerAngles.y, transform.eulerAngles.z);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, 50 * Time.deltaTime);
		}

        kartNormal.up = Vector3.Lerp(kartNormal.up, hit.normal, Time.deltaTime * 8.0f); // car tilt base on ground
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    public void Steer(int direction, float amount)
    {
        rotate = (turnSpeed * direction) * amount;
    }

    private void Boost()
    {
        isDrifting = false;

        DOVirtual.Float(currentSpeed * 3, currentSpeed, .3f, Speed);
        //DOVirtual.Float(0, 1, .5f, ChromaticAmount).OnComplete(() => DOVirtual.Float(1, 0, .5f, ChromaticAmount));

        kartModel.parent.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);
    }

    private void Speed(float x)
	{
        currentSpeed = x;
	}

    private void ChromaticAmount(float x)
    {
        //postProfile.GetSetting<ChromaticAberration>().intensity.value = x;
    }

    private void OnDrawGizmos()
	{
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z));
	}












    private void AlterUpdate()
	{
        transform.position = sphereRb.transform.position;       // update car to follow sphere

        vertialInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        speed = vertialInput > 0 ? vertialInput * fwdSpeed : vertialInput * revSpeed;       // adjust speed

        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, Road);

        if (isGrounded)
        {
            float newRotation = horizontalInput * turnSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");      // set car rotation
            transform.Rotate(0, newRotation, 0, Space.World);
        }

        transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;      // car tilt base on ground

        sphereRb.drag = isGrounded ? groundDrag : airDrag;      // change drag if airbone

        frontWheels.localEulerAngles = new Vector3(0, (horizontalInput * maxWheelTurn), frontWheels.localEulerAngles.z);    // wheels turn
        frontWheels.localEulerAngles += new Vector3(0, 0, sphereRb.velocity.magnitude / 2);                                             // wheels roll
        backWheels.localEulerAngles += new Vector3(0, 0, sphereRb.velocity.magnitude / 2);
    }

    private void AlterFixedUpdate()
	{
        if (isGrounded)
        {
            sphereRb.AddForce(transform.forward * speed, ForceMode.Acceleration);
            
        }
        else
        {
            sphereRb.AddForce(transform.up * -20f);     // add gravity;
            Quaternion newRotation = Quaternion.Euler(20, transform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, 50 * Time.deltaTime);
        }
    }
}
