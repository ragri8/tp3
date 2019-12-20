﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private AudioSource sound;
    private float tdebut=0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - tdebut > 0.1)
        {
            light.enabled = false;
        }

        if (Time.time - tdebut > 5)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (transform.position.y<0.5)
        {
            sound.Play();
        }
    }

    private void activate(Vector3 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        gameObject.GetComponent<Rigidbody>().velocity=Vector3.zero;
        light.enabled = true;
        tdebut = Time.time;
    }
}
