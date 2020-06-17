using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class cars : MonoBehaviour
{
    public WheelDrive carOriginal;
    private float timeBtwnSpawn;
    public float startTimeBtwnSpwn;
    System.Random rn = new System.Random();
    private int num;
    private int speedprob;
    public Rigidbody rb;
    WheelDrive carClonefamily;
    //public Health sethealth;





    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        //sethealth = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Health>();
        if (timeBtwnSpawn <= 0)
        {
            num = rn.Next(1, 3);
            speedprob = rn.Next(1, 10);
            //print(num);
            WheelDrive carClonefamily = Instantiate(carOriginal);
            Transform trans = carClonefamily.transform;
            Transform sign = trans.Find("Canvas");
            Transform msign = trans.Find("Canvas-maint");
            msign.gameObject.SetActive(false);
            if (num == 1)
            {
                Transform Pathhb = GameObject.Find("Path").transform;
                carClonefamily.path = Pathhb; 
                //carClonefamily.transform.GetChild(6).gameObject.SetActive(false);
                sign.gameObject.SetActive(false);
                //carClonefamily.yourSprite.SetActive(false);
                //print(yourSprite);
            }
            else
            {
                Transform Pathhb = GameObject.Find("Path2").transform;
                //yourSprite = GameObject.Find("Canvas");
                //carClonefamily.yourSprite.SetActive(false);
                carClonefamily.path = Pathhb;
                sign.gameObject.SetActive(false);
                //carClonefamily.transform.GetChild("Canvas").gameObject.SetActive(false);

            }
            if (speedprob == 4 || speedprob == 7)
            {
                carClonefamily.maxSpeed = 200;
                carClonefamily.maxTorque = 170;
                sign.gameObject.SetActive(true);
                //carClonefamily.transform.GetChild("Canvas").gameObject.SetActive(true);
                //yourSprite = GameObject.Find("Canvas");
                // carClonefamily.yourSprite.SetActive(true);
                //SGameObject yourSprite = GameObject.Find("Canvas");
                //YourSprite.SetActive(False);
                //rb = carClonefamily.GetComponent<Rigidbody>();
                //rb.velocity = transform.forward * 100;
            }
            

            //UnityEngine.Debug.Log(carClonefamily.transform.position);
            //Path.Path = Path;

            timeBtwnSpawn = startTimeBtwnSpwn;
        }
        else
        {
            timeBtwnSpawn -= Time.deltaTime;
        }
    }
}
