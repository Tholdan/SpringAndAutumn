using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeamController : MonoBehaviour {

    //PUBLIC
    public GameObject lightBeamPrefab;
    public GameObject lightGenerator;

    [Space]

    [Header("Initial Light Beam Configuration")]
    public Vector3 lightBeamLocalPosition;
    public Vector3 lightBeamDirection;

    [Space]

    [Header("Light Beam Configuration")]
    public int maxReflectionBeams = 10;
    public float maxLightBeamDistance = 100;


    //PRIVATE
    private GameObject[] reflectionVector;

    private int currentReflection;

	// Use this for initialization
	void Start () {
        if(maxReflectionBeams <= 0)
        {
            Debug.LogError("Max Relfections Beams have to be a value bigger than 0");
        }

        Debug.Log("Remember that the gameobject with this object have to be always placed in (0, 0, 0).");
        Debug.Log("Current GameObject is " + transform.name);

        //Light beam vector initialized.
        reflectionVector = new GameObject[maxReflectionBeams];
        currentReflection = 0;

        //Initial light beam configuration.
        Debug.Log(lightGenerator.transform.name);
        reflectionVector[0] = Instantiate(lightBeamPrefab);
        reflectionVector[0].transform.parent = lightGenerator.transform;
        reflectionVector[0].transform.localPosition = lightBeamLocalPosition;
        reflectionVector[0].GetComponent<LineRenderer>().SetPosition(0, lightBeamLocalPosition);
        reflectionVector[0].GetComponent<LineRenderer>().SetPosition(1, lightBeamLocalPosition + lightBeamDirection * maxLightBeamDistance);
	}
	
	// Update is called once per frame
	void Update () {
        bool end = true;
        currentReflection = 0;

        //First beam treatment
        RaycastHit hit;
        LineRenderer currentLightRenderer = reflectionVector[currentReflection].GetComponent<LineRenderer>();
        if (currentLightRenderer == null)
        {
            Debug.LogError("Not line renderer found in the lightBeam prefab");
        }

        if (Physics.Raycast(lightGenerator.transform.position + currentLightRenderer.GetPosition(0), lightGenerator.transform.rotation * (currentLightRenderer.GetPosition(1) - currentLightRenderer.GetPosition(0)), out hit, maxLightBeamDistance))
        {
            currentLightRenderer.SetPosition(1, Vector3.forward * hit.distance);
            if (hit.transform.CompareTag("Mirror") && maxReflectionBeams > 1)
            {
                end = false;
                currentReflection = NextCurrentReflection();
            }
        }
        else
        {
            currentLightRenderer.SetPosition(1, Vector3.forward * maxLightBeamDistance);
            currentReflection = NextCurrentReflection();
        }

        int antiInfiniteLoop = 0;
        while (!end && currentReflection < maxReflectionBeams)
        {
            //If the ray isn't instantiated it is created.
            if(reflectionVector[currentReflection] == null)
            {
                reflectionVector[currentReflection] = Instantiate(lightBeamPrefab);
            }

            //Calculates and initializes the new vector using reflection physics.
            reflectionVector[currentReflection].transform.parent = transform;
            currentLightRenderer = reflectionVector[currentReflection].GetComponent<LineRenderer>();
            currentLightRenderer.SetPosition(0, hit.point);
            Vector3 mirrorVector = Vector3.Reflect(GetBeamDirection(reflectionVector[currentReflection - 1], true), hit.normal);
            currentLightRenderer.SetPosition(1, currentLightRenderer.GetPosition(0) + (mirrorVector).normalized * maxLightBeamDistance);

            //Checks if the ray has collided with another mirror.
            if (Physics.Raycast(currentLightRenderer.GetPosition(0), currentLightRenderer.GetPosition(1) - currentLightRenderer.GetPosition(0), out hit, maxLightBeamDistance))
            {
                currentLightRenderer.SetPosition(1, hit.point);
                if (hit.transform.CompareTag("Mirror") && currentReflection < maxReflectionBeams)
                {
                    currentReflection = NextCurrentReflection();
                }
                else
                {
                    end = true;
                }
            }
            else
            {
                end = true;
                currentLightRenderer.SetPosition(1, (currentLightRenderer.GetPosition(1)).normalized * maxLightBeamDistance);
            }

            antiInfiniteLoop++;
            if(antiInfiniteLoop >= 1000)
            {
                break;
            }
        }

        bool nullFound = false;
        currentReflection = NextCurrentReflection();
        while(!nullFound && currentReflection < maxReflectionBeams)
        {
            if (reflectionVector[currentReflection] != null)
            {
                Destroy(reflectionVector[currentReflection]);
                currentReflection = NextCurrentReflection();
            }
            else
            {
                nullFound = true;
            }
        }
	}

    private int NextCurrentReflection()
    {
        return (currentReflection + 1) % reflectionVector.Length;
    }

    private Vector3 GetBeamDirection(GameObject beam, bool first)
    {
        Vector3 aux;
        if (first)
        {
            aux = this.lightGenerator.transform.rotation * (beam.GetComponent<LineRenderer>().GetPosition(1) - beam.GetComponent<LineRenderer>().GetPosition(0));
        }
        else
        {
            aux = (beam.GetComponent<LineRenderer>().GetPosition(1) - beam.GetComponent<LineRenderer>().GetPosition(0));
        }
        return aux.normalized;
        
    }
}