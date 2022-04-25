﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(CharacterController))]
public class ControllerMovement : MonoBehaviour
{
    CharacterController characterController;
    [SerializeField] Collider BodyCollider;
    [SerializeField] Transform VRCameraTransform;
    [SerializeField] GameObject VRRightHand;
    [SerializeField] GameObject VRLeftHand;

    [SerializeField] SteamVR_Input_Sources InputSourceLeft;
    [SerializeField] SteamVR_Input_Sources InputSourceRight;

    [SerializeField] SteamVR_Action_Vector2 SteamVRMovement;
    
    [SerializeField] float MovementSpeed;

    void Awake(){
        // Ignore collisions with character controller and body collider from steamvrobjects
        characterController = GetComponent<CharacterController>();
        Physics.IgnoreCollision(characterController, BodyCollider);

        // Setup event listeners for movement system
        // Left controller for moving
        // Right controller for rotating
        SteamVRMovement.AddOnAxisListener(Move, InputSourceLeft);
        SteamVRMovement.AddOnAxisListener(Rotate, InputSourceRight);
    }

    [SerializeField] GameObject LeftHandColliderController;
    [SerializeField] GameObject RightHandColliderController;

    void Start(){
        // Have VR hand colliders ignore collisions witht the character controller
        // Find HandColliderLeft and Right that gets spawned at the start of runtime
        LeftHandColliderController = GameObject.Find("HandColliderLeft(Clone)").gameObject;
        RightHandColliderController = GameObject.Find("HandColliderRight(Clone)").gameObject;

        // Fetch the HandCollider components off both hand collider gameObjects
        Valve.VR.InteractionSystem.HandCollider leftHandColliders = LeftHandColliderController.GetComponent<Valve.VR.InteractionSystem.HandCollider>();
        Valve.VR.InteractionSystem.HandCollider rightHandColliders = RightHandColliderController.GetComponent<Valve.VR.InteractionSystem.HandCollider>();

        // Set the Collision layermask for the colliders to only collide with Colliders with the tag "Interactable"
        LayerMask newMask = LayerMask.GetMask("Interactable");
        leftHandColliders.collisionMask = newMask;
        rightHandColliders.collisionMask = newMask; 

        // Get all the colliders in both hands, iterate through setting them to ignore collisions with CharacterController
        Collider[] leftColliders = leftHandColliders.colliders;
        Collider[] rightColliders = rightHandColliders.colliders;

        foreach(Collider collider in leftColliders){
            Physics.IgnoreCollision(characterController, collider);
            Physics.IgnoreCollision(BodyCollider, collider);
        }

        foreach(Collider collider in rightColliders){
            Physics.IgnoreCollision(characterController, collider);
            Physics.IgnoreCollision(BodyCollider, collider);
        }
    }

    void ManageController(){
        characterController.radius = .2f;
    }

    void LateUpdate(){
        // Set CharacterController transform.position.x and y to VRCamera.position.x and y
        Vector3 cameraPos = VRCameraTransform.localPosition;
        cameraPos.y = transform.position.y;
        //characterController.transform.position = cameraPos;
        characterController.center = cameraPos;
    }

    private void Move(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta){
        Vector3 movement = new Vector3(axis.x, 0f, axis.y);
        movement = VRCameraTransform.forward * movement.z + VRCameraTransform.right * movement.x;
        if(!characterController.isGrounded){
            movement.y = -9.8f;
            characterController.Move(movement * MovementSpeed * Time.deltaTime);
        }else{
            characterController.Move(movement * MovementSpeed * Time.deltaTime);
        }
    }

    private void Rotate(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta){
        transform.Rotate(0, axis.x * 90 * Time.deltaTime, 0);
    }

    void OnColliderEnter(Collision other){
        Debug.Log(other.transform.tag);
    }
}
