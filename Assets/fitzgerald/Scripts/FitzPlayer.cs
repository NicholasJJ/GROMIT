using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitzPlayer : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    public float attackDistance = 1.0f;
    public float attackHalfAngle = 45.0f;
    public float attackDamage = 10;

    private Animator animator;
    private ActiveItem inventory;

    public List<FitzRoomVolume> currentRooms;

    void Awake() {
        // Unload all rooms
        foreach (var room in FindObjectsOfType<FitzRoomVolume>()) {
            room.Unload();
        }
    }

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        if (!controller) {
            controller = gameObject.AddComponent<CharacterController>();
        }

        if (!animator) {
            animator = gameObject.GetComponentInChildren<Animator>();
        }

        if (!inventory) {
            inventory = gameObject.GetComponent<ActiveItem>();
        }
        if (inventory) {
            inventory.OnGrabItem.AddListener(item => {
                foreach (var room in currentRooms) {
                    room.RemoveRoomObject(item.gameObject);
                }
            });
            inventory.OnDropItem.AddListener(item => {
                foreach (var room in currentRooms) {
                    room.AddRoomObject(item.gameObject);
                }
            });
        }

        var health = gameObject.GetComponent<FitzHealth>();
        if (health) {
            health.OnTakeDamage.AddListener((damage, causer) => {
                PlayerUI.inst.healthbar.SetHealth(health.GetHealthPct());
            });
            PlayerUI.inst.healthbar.SetHealth(health.GetHealthPct());
        }
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (move.magnitude > 1) move /= move.magnitude;
        Physics.SyncTransforms();
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        animator.SetFloat("Speed", move.magnitude);

        // Don't let them jump, for now
        // Changes the height position of the player..
        // if (Input.GetButtonDown("Jump") && groundedPlayer)
        // {
        //     playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        // }

        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     // Attack
        //     // This needs to be much more highly parameterized. Putting this as a simple placeholder

        //     var hits = Physics.OverlapSphere(transform.position, attackDistance);
        //     foreach (var hit in hits) {
        //         if(Mathf.Acos(Vector3.Dot((hit.transform.position - transform.position).normalized, transform.forward)) <= attackHalfAngle * Mathf.Deg2Rad) {
        //             Debug.Log("Hit this object: " + hit.gameObject.name);

        //             var health = hit.gameObject.GetComponent<FitzHealth>();
        //             if (health) {
        //                 health.CauseDamage(attackDamage, gameObject);
        //             } 
        //         }
        //     }

        //     animator.SetTrigger("Attack");
        // }

        playerVelocity.y += gravityValue * Time.deltaTime;
        Physics.SyncTransforms();
        controller.Move(playerVelocity * Time.deltaTime);

        // Check if the player is holding an object
        if (inventory) {
            animator.SetBool("grabbing", inventory.GrabbingObject());
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        // Debug.Log($"OnCheckColliderHit {hit.gameObject.name}");

        var door = hit.gameObject.GetComponent<FitzDoor>();
        if (door) {
            door.CheckDoorCollision(gameObject);
        }

        var inventoryItem = hit.gameObject.GetComponent<FitzInventoryItem>();
        if (inventoryItem) {
            inventoryItem.CheckPickup(gameObject);
        }

        var damage = hit.gameObject.GetComponent<DamageOnCollision>();
        if (damage) {
            Debug.Log($"Damage on collision {damage}, {GetComponent<FitzHealth>()}");
            damage.CauseHealthDamage(GetComponent<FitzHealth>());
        }
    }

    private void OnTriggerEnter(Collider other) {
        var room = other.GetComponent<FitzRoomVolume>();
        if (room) {
            if (!InRoom(room.roomId)) {
                room.RoomEntered(this);
            }
            currentRooms.Add(room);
        }
    }

    private void OnTriggerExit(Collider other) {
        var room = other.GetComponent<FitzRoomVolume>();
        if (room) {
            currentRooms.Remove(room);
            if (!InRoom(room.roomId)) {
                room.RoomExited(this);
            }
        }
    }

    private bool InRoom(string roomId) {
        foreach (var room in currentRooms) {
            if (room.roomId == roomId) {
                return true;
            }
        }
        return false;
    }
}
