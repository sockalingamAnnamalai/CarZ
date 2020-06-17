using UnityEngine;
using System;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics;

[Serializable]
public enum DriveType
{
	RearWheelDrive,
	FrontWheelDrive,
	AllWheelDrive
}

public class WheelDrive : MonoBehaviour
{
	public Transform path;
	
	private List<Transform> nodes;
	private int currentNode = 0;
	public bool isBreak = false;
	public float currentSpeed;
	public float maxSpeed = 145f;
	public WheelCollider a1l;
	public WheelCollider a1r;
	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	private float maxBreakTorque = 150f;
	//[Tooltip("Maximum steering angle of the wheels")]
	private float maxAngle = 90f;
	//[Tooltip("Maximum torque applied to the driving wheels")]
	public float maxTorque = 120f;
	//[Tooltip("Maximum brake torque applied to the driving wheels")]
	//public float brakeTorque = 30000f;
	[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
	public GameObject wheelShape;
	[Header("Sensors")]
	private int sensorLength = 4;
	private Vector3 frontSensorPos = new Vector3(0f, 0.1f, 1.2f);
	private float frontSideSensorPos = .8f;
	private float frontSensorAngle = 30f;

	private bool objectFront;
	private bool objectFrontRight;
	private bool objectFrontRightAngle;
	private bool objectFrontLeft;
	private bool objectFrontLeftAngle;
	private bool objectFrontLeft90;
	private bool objectFrontRight90;



	//[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	//public float criticalSpeed = 5f;
	//[Tooltip("Simulation sub-steps when the speed is above critical.")]
	//public int stepsBelow = 5;
	//[Tooltip("Simulation sub-steps when the speed is below critical.")]
	//public int stepsAbove = 1;

	[Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
	public DriveType driveType;

    private WheelCollider[] m_Wheels;
	


	// Find all the WheelColliders down in the hierarchy.
	void Start()
	{
		
		m_Wheels = GetComponentsInChildren<WheelCollider>();

		for (int i = 0; i < m_Wheels.Length; ++i) 
		{
			var wheel = m_Wheels [i];

			// Create wheel shapes only when needed.
			if (wheelShape != null)
			{
				var ws = Instantiate (wheelShape);
				ws.transform.parent = wheel.transform;
			}
		}
		Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
		nodes = new List<Transform>();
		//UnityEngine.Debug.Log(pathTransforms[0].position);
		for (int i = 1; i < pathTransforms.Length; i++)
		{

			if (pathTransforms[i] != transform)
			{
				nodes.Add(pathTransforms[i]);
			}
		}
	}

	// This is a really simple approach to updating wheels.
	// We simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero.
	// This helps us to figure our which wheels are front ones and which are rear.
	void Update()
	{
		//print(maxTorque);
		//m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

		//float angle = maxAngle * Input.GetAxis("Horizontal");
		//float torque = maxTorque * Input.GetAxis("Vertical");
		//sUnityEngine.Debug.Log(nodes[currentNode].position);
		Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
		float newangle = (relativeVector.x / relativeVector.magnitude) * maxAngle;

		//float handBrake = Input.GetKey(KeyCode.X) ? brakeTorque : 0;
		

		foreach (WheelCollider wheel in m_Wheels)
		{
			// A simple car where front wheels steer while rear ones drive.
			if (wheel.transform.localPosition.z > 0)
			{
				wheel.steerAngle = newangle;
			}
			/*
			if (wheel.transform.localPosition.z < 0)
			{
				wheel.brakeTorque = handBrake;
			}

			if (wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheelDrive)
			{
				wheel.motorTorque = torque;
			}

			if (wheel.transform.localPosition.z >= 0 && driveType != DriveType.RearWheelDrive)
			{
				wheel.motorTorque = torque;
			}*/
			
			// Update visual wheels if any.
			if (wheelShape) 
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				// Assume that the only child of the wheelcollider is the wheel shape.
				Transform shapeTransform = wheel.transform.GetChild (0);

                if (wheel.name == "a0l" || wheel.name == "a1l" || wheel.name == "a2l")
                {
                    shapeTransform.rotation = q * Quaternion.Euler(0, 180, 0);
                    shapeTransform.position = p;
                }
                else
                {
                    shapeTransform.position = p;
                    shapeTransform.rotation = q;
                }
			}
		}
		//currentSpeed = 2 * Mathf.PI * a1l.radius * a1l.rpm * 60 / 1000;
		
		//if (currentSpeed < maxSpeed && isBreak == false)
		//{
			a1l.motorTorque = maxTorque;
			a1r.motorTorque = maxTorque;
       // }
        //else
        //{
		//	a1l.motorTorque = 0;
		//	a1r.motorTorque = 0;
		//}

		if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5)
		{
			if (currentNode < nodes.Count - 1)
			{
				currentNode++;
			}
			else
			{
				//currentNode = nodes[1].position;
				Destroy(gameObject);
			}
		}

        if (isBreak)
        {
			wheelFL.brakeTorque = maxBreakTorque;
			wheelFR.brakeTorque = maxBreakTorque;
		}
        else
        {
			wheelFL.brakeTorque = 0;
			wheelFR.brakeTorque = 0;
		}

		if (isBreak)
		{
			//print("hii");
			transform.Find("Canvas-maint").gameObject.SetActive(true);
		}

		RaycastHit hit;
		Vector3 sensorStarPos = transform.position;
		sensorStarPos += transform.forward * frontSensorPos.z;
		sensorStarPos += transform.up * frontSensorPos.y;

		objectFront = Physics.Raycast(sensorStarPos, transform.forward, out hit, sensorLength);
		sensorStarPos += transform.right * frontSideSensorPos;
		objectFrontRight = Physics.Raycast(sensorStarPos, transform.forward, out hit, sensorLength);
		objectFrontRightAngle = Physics.Raycast(sensorStarPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength);
		sensorStarPos -= transform.right * frontSideSensorPos * 2;
		objectFrontLeft = Physics.Raycast(sensorStarPos, transform.forward, out hit, sensorLength);
		objectFrontLeftAngle = Physics.Raycast(sensorStarPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength);

		sensorStarPos = transform.position;
		sensorStarPos += transform.up * frontSensorPos.y;
		objectFrontLeft90 = Physics.Raycast(sensorStarPos, Quaternion.AngleAxis(-90, transform.up) * transform.forward, out hit, 1);
		//UnityEngine.Debug.DrawLine(sensorStarPos, hit.point);
		objectFrontRight90 = Physics.Raycast(sensorStarPos, Quaternion.AngleAxis(90, transform.up) * transform.forward, out hit, 1);
		//UnityEngine.Debug.DrawLine(sensorStarPos, hit.point);
		//front center

		if (objectFrontRight && (!objectFrontLeftAngle || !objectFrontLeft))
        {
			wheelFL.steerAngle = wheelFL.steerAngle - 60;
			wheelFR.steerAngle = wheelFR.steerAngle - 60;
		}
		else if (objectFrontRight && !objectFrontRightAngle)
		{
			wheelFL.steerAngle = wheelFL.steerAngle + 75;
			wheelFR.steerAngle = wheelFR.steerAngle + 75;
		}
		else if (objectFrontRightAngle && !objectFrontLeft)
		{
			wheelFL.steerAngle = wheelFL.steerAngle - 45;
			wheelFR.steerAngle = wheelFR.steerAngle - 45;
		}
		else if (objectFrontLeft && (!objectFrontRightAngle || !objectFrontRight))
		{
			wheelFL.steerAngle = wheelFL.steerAngle + 60;
			wheelFR.steerAngle = wheelFR.steerAngle + 60;
		}
		else if (objectFrontLeft && !objectFrontLeftAngle)
		{
			wheelFL.steerAngle = wheelFL.steerAngle - 75;
			wheelFR.steerAngle = wheelFR.steerAngle - 75;
        }
		else if (objectFrontLeftAngle && !objectFrontRight)
		{
			wheelFL.steerAngle = wheelFL.steerAngle + 45;
			wheelFR.steerAngle = wheelFR.steerAngle + 45;
		}
		else if (objectFrontLeft90 && !objectFrontRightAngle)
		{
			wheelFL.steerAngle = wheelFL.steerAngle + 20;
			wheelFR.steerAngle = wheelFR.steerAngle + 20;
		}
		else if (objectFrontRight90 && !objectFrontLeftAngle)
		{
			wheelFL.steerAngle = wheelFL.steerAngle - 20;
			wheelFR.steerAngle = wheelFR.steerAngle - 20;
		}
		else if(!objectFront)
        {
			wheelFL.steerAngle = wheelFL.steerAngle;
			wheelFR.steerAngle = wheelFR.steerAngle;
        }
        else
        {
			a1l.motorTorque = 0;
			a1r.motorTorque = 0;
		}

		/*
		//front right

		sensorStarPos += transform.right * frontSideSensorPos;
		if (Physics.Raycast(sensorStarPos, transform.forward, out hit, sensorLength))
		{
			
			UnityEngine.Debug.DrawLine(sensorStarPos, hit.point);
			wheelFL.steerAngle = wheelFL.steerAngle - 60;
			wheelFR.steerAngle = wheelFR.steerAngle - 60;
		}
		
		// front right angle
		if (Physics.Raycast(sensorStarPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
		{
			UnityEngine.Debug.DrawLine(sensorStarPos, hit.point);
			wheelFL.steerAngle = wheelFL.steerAngle - 30;
			wheelFR.steerAngle = wheelFR.steerAngle - 30;
		}
		// front left
		sensorStarPos -= transform.right * frontSideSensorPos * 2;
		if (Physics.Raycast(sensorStarPos, transform.forward, out hit, sensorLength))
		{
			UnityEngine.Debug.DrawLine(sensorStarPos, hit.point);
			//wheelFL.steerAngle = wheelFL.steerAngle + 40;
			//wheelFR.steerAngle = wheelFR.steerAngle + 40;

		}
		//front left angle
		if (Physics.Raycast(sensorStarPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up)*transform.forward, out hit, sensorLength))
		{
			UnityEngine.Debug.DrawLine(sensorStarPos, hit.point);
			//wheelFL.steerAngle = wheelFL.steerAngle + 40;
			//wheelFR.steerAngle = wheelFR.steerAngle + 40;

		}*/
		

	}

}
