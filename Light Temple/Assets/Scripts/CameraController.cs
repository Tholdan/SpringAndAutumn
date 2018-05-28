using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionRotations
{
    public Quaternion up = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    public Quaternion down = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    public Quaternion left = Quaternion.Euler(0.0f, 270.0f, 0.0f);
    public Quaternion right = Quaternion.Euler(0.0f, 90.0f, 0.0f);

    public Quaternion[] rotationsVector = new Quaternion[4] {
        Quaternion.Euler(0.0f, 0.0f, 0.0f),
        Quaternion.Euler(0.0f, 90.0f, 0.0f),
        Quaternion.Euler(0.0f, 180.0f, 0.0f),
        Quaternion.Euler(0.0f, 270.0f, 0.0f) };
}

public class CameraController : MonoBehaviour {

    //PUBLIC
    public Transform focus;
    public Transform mainCamera;
    public Vector3 cameraPosition;
    public Vector3 lookAtPosition;

    public float rotationSpeed = 1;


    //PRIVATE
    private Quaternion desiredRotation;
    private QuaternionRotations quaternionRotations;
    private int currentRotation;

	// Use this for initialization
	void Start () {
        quaternionRotations = new QuaternionRotations();

        transform.position = focus.position;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        
        mainCamera.localPosition = cameraPosition;
        mainCamera.LookAt(transform.position + lookAtPosition);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = focus.position;

		if(Input.GetButtonDown("Rotate Camera Left"))
        {
            currentRotation = (currentRotation + 1) % quaternionRotations.rotationsVector.Length;
            desiredRotation = quaternionRotations.rotationsVector[currentRotation];
        }
        if(Input.GetButtonDown("Rotate Camera Right"))
        {
            currentRotation = (currentRotation - 1) % quaternionRotations.rotationsVector.Length;
            if(currentRotation < 0)
            {
                currentRotation = quaternionRotations.rotationsVector.Length - 1;
            }
            desiredRotation = quaternionRotations.rotationsVector[currentRotation];
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

    public Quaternion GetCurrentRotation()
    {
        return quaternionRotations.rotationsVector[currentRotation];
    }
}
