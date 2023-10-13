using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{

    Rigidbody myRigidbody;

    // Start is called before the first frame update
    void Start()
    {

        myRigidbody = gameObject.AddComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Keypad8)) {
            myRigidbody.AddForce(Vector3.forward * 20f);
        }
        if (Input.GetKey(KeyCode.Keypad4)) {
            myRigidbody.AddForce(Vector3.left * 20f);
        }
        if (Input.GetKey(KeyCode.Keypad5)) {
            myRigidbody.AddForce(Vector3.back * 20f);
        }
        if (Input.GetKey(KeyCode.Keypad6)) {
            myRigidbody.AddForce(Vector3.right * 20f);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            myRigidbody.AddForce(Vector3.up * 20f, ForceMode.Impulse);
        }
    }
}
