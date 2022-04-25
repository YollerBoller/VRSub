﻿//This file modifies the wallphone when unattaching/reattaching to base.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneAttached : MonoBehaviour
{

    public bool collidingWithBase = true;
    private Vector3 defaultPosition;
    private Vector3 defaultRotation;
    private Rigidbody rb;
    private GameObject phone;
    public AudioSource audio;
    

    private void Awake(){        
        defaultPosition = GameObject.FindGameObjectWithTag("Base").transform.position;
        defaultRotation = new Vector3(-90f, 90f, 0f);
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        phone = this.gameObject;
        
                                      
    }

    public void playAudio() {
        audio.Play(0);                
    }
    public void stopAudio() {
        audio.Stop();       
    }    

    public void OnHandAttach() { //unattach phone from base
        rb.useGravity = true;        
        rb.constraints = RigidbodyConstraints.None;    
        
        playAudio();     
    }

    public void OnHandDetach() { //reattach phone to base
        if (collidingWithBase) {        
            rb.useGravity = false;            
            transform.position = defaultPosition;
            transform.eulerAngles = defaultRotation;            
            rb.constraints = RigidbodyConstraints.FreezePosition;           
            stopAudio(); 
        }
    }

    private void OnCollisionEnter(Collision other) { //check for collision to trigger OnHandDetach
        if(other.transform.tag == ("Base")) {       
            collidingWithBase = true; 
            OnHandDetach();
        }    
        if(other.transform.tag == ("Button")) {
            Physics.IgnoreCollision(phone.GetComponent<Collider>(), other.collider);
        }
    }

    private void OnCollisionExit(Collision other) { //check for collision to trigger OnHandAttach
        if(other.transform.tag == ("Base")) {
            collidingWithBase = false; 
            OnHandAttach();
        }
    }    
}
