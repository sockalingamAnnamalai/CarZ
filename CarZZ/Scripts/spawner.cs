using System.Collections;
using System.Collections.Generic;
//using System.ComponentModel;
//using UnityEngine.Component;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class spawner : MonoBehaviour
{
    private void Update()
    {
        //print("hi");
        void OnTriggerEnter(Collider col)
        {
            print("hi");
            if (col.gameObject.tag == "P1")
            {
                print("hi");
                Destroy(this.gameObject);
            }
        }
    }

}
