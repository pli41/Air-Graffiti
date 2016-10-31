using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

    Vector3 oldPos;
    [SerializeField]
    GameObject marker;

    Vector3 currentVelocity;
    Vector3 currentAcceleration;

    Vector3 oldVelocity;
    Vector3 oldAcceleration;
    [SerializeField]
    float interval;
    [SerializeField]
    float noiseThreshold;

    Vector3 deviceAccleration;
    Vector3 calculatedTranslation;


	Rigidbody rigid;


	// Use this for initialization
	void Start () {
		rigid = GetComponent<Rigidbody> ();
		Input.gyro.enabled = true;
		oldAcceleration = FilterNoise(Input.gyro.userAcceleration, noiseThreshold);
        //StartCoroutine("MarkDevicePos");
        //StartCoroutine("MovePointer");

    }
	
	// Update is called once per frame
	void Update () {
		deviceAccleration = Input.gyro.userAcceleration;
	}


	void FixedUpdate(){
		currentAcceleration = FilterNoise(Input.gyro.userAcceleration, noiseThreshold);
		currentVelocity = oldVelocity + oldAcceleration * interval;

		Vector3 jerk = (currentAcceleration - oldAcceleration) / Time.fixedDeltaTime;
		if (jerk.magnitude <= 0) {
			rigid.velocity = Vector3.zero;
			rigid.angularVelocity = Vector3.zero; 
		}
		else {
			rigid.AddForce (oldVelocity);
		}


		oldAcceleration = currentAcceleration;
		oldVelocity = currentVelocity;
	}


    void OnDrawGizmos()
    {
		/*
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(currentAcceleration.x, 0, 0));
        Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, currentAcceleration.x, 0));
        Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 0, currentAcceleration.x));
        */

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + new Vector3(Input.gyro.userAcceleration.x, 0, 0));
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, Input.gyro.userAcceleration.y, 0));
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 0, Input.gyro.userAcceleration.z));
    }

    IEnumerator MarkDevicePos()
    {
        while (true)
        {
            GameObject mark = Instantiate(marker) as GameObject;
            mark.transform.position = transform.position;
            Destroy(mark, 10f);
            yield return new WaitForSeconds(interval);
        }
    }
    
    IEnumerator MovePointer()
    {
        while (true)
        {
			Debug.Log ("moving");
			currentAcceleration = FilterNoise(Input.gyro.userAcceleration, noiseThreshold);
            currentVelocity = oldVelocity + oldAcceleration * interval;

            transform.position += GetTranslation();

            oldAcceleration = currentAcceleration;
            oldVelocity = currentVelocity;

            yield return new WaitForSeconds(interval);
        }
    }

    Vector3 GetTranslation()
	{
        Vector3 jerk = (currentAcceleration - oldAcceleration) / interval;
        //Vector3 jerk = Vector3.zero;
        if (jerk.magnitude == 0)
        {
            return Vector3.zero;
        }

        Vector3 translation = oldVelocity * interval + 0.5f * oldAcceleration * Mathf.Pow(interval, 2f) + (1f/6f) * jerk * Mathf.Pow(interval, 3f);
        calculatedTranslation = translation;
		return translation;
    }

    Vector3 FilterNoise(Vector3 vec, float threshold)
    {
        vec.x = Mathf.Abs(vec.x) > threshold ? vec.x : 0;
        vec.y = Mathf.Abs(vec.y) > threshold ? vec.y : 0;
        vec.z = Mathf.Abs(vec.z) > threshold ? vec.z : 0;
        return vec;
    }

}
