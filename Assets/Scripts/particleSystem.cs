using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleSystem : MonoBehaviour
{
    private float tdebut = 0;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (Time.time - tdebut > 10) {
            gameObject.SetActive(false);
        }
    }
    public void activate() {
        gameObject.SetActive(true);
        gameObject.GetComponent<Rigidbody>().velocity=Vector3.zero;
        tdebut = Time.time;
    }
}
