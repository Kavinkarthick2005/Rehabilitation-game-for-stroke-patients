using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : MonoBehaviour
{
    public GameObject foodsphere;
    public Vector3 desiredFoodSize = new Vector3(0.2f, 0.2f, 0.2f);

    public void OnTriggerEnter(Collider other){
        if(other.name == "Spatula"){
            
            foodsphere.SetActive(true);
            foodsphere.transform.localScale = desiredFoodSize;
        }
    }
}
 


