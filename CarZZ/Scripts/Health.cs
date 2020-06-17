using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public Slider slider;
    public float maxhealth = 1f;
    public float currenthealth;
    // Start is called before the first frame update
    void Start()
    {
        //print(slider.value);
        currenthealth = maxhealth;
        //slider.value = currenthealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        //print(currenthealth);
        slider.value = currenthealth;
        if(currenthealth <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
