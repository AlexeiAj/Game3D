using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public GameObject player;
    public Camera playerCam;
    public Transform head;
    public Animator animations;
    public Keys keys { get; set; }
    public string username = "";
    public int id = -1;
    public bool enemy;

    private bool isGrounded;
    private bool isRunning = false;
    private bool readyToJump = true;
    private float jumpDelay = 0.6f;

    public ParticleSystem shootPS;
    private float fireRate = 2f;
    private float nextTimeToFire = 0f;

    private Quaternion camRotation;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        keys = new Keys();
    }

    void FixedUpdate() {
        running();
        jump();
        shoot();

        if(!enemy) sendKeys();
    }
    
    void LateUpdate() {
        head.transform.localRotation = camRotation;
    }

    private void shoot() {
        if(keys.mouseLeft && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 2f / fireRate;
            shootPS.Play();
        }
    }

    private void running() {
        bool aux = isRunning;
        isRunning = keys.x != 0 || keys.y != 0;
        if(!isGrounded) isRunning = false;
        if(aux == isRunning) return;
        
        animations.SetBool("Running", isRunning);
    }

    private void jump() {
        if(keys.jumping && isGrounded && readyToJump){
            readyToJump = false;
            animations.SetBool("Jumping", true);
            Invoke("restartJump", jumpDelay);
        }
    }

    private void restartJump() {
        readyToJump = true;
        animations.SetBool("Jumping", false);
    }

    private void sendKeys(){
        keys.updateKeys();
        sendPlayerKeys();
    }

    public void sendPlayerKeys() {
        Packet packet = new Packet();
        packet.Write("playerKeysTS");
        packet.Write(id);
        packet.Write(keys);
        Client.instance.sendUdpData(packet);
    }

    public void playerPosition(Packet packet) {
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        camRotation = packet.ReadQuaternion();
        isGrounded = packet.ReadBool();
        if(enemy) keys = packet.ReadKeys();

        ThreadManager.ExecuteOnMainThread(() => {
            playerPositionUpdate(id, position, rotation, camRotation);
        });
    }

    public void playerPositionUpdate(int id, Vector3 position, Quaternion rotation, Quaternion camRotation) {
        if(player == null) return;
        
        player.transform.position = position;
        player.transform.rotation = rotation;
        playerCam.transform.localRotation = camRotation;
    }

    public void removePlayer() {
        ThreadManager.ExecuteOnMainThread(() => {
            Client.instance.removeFromPlayers(id);
            Debug.Log("Player [id: "+id+"] has disconnect!");
            Destroy(player);
        });
    }

    public void setId(int id) {
        this.id = id;
    }

    public void setUsername(string username) {
        this.username = username;
    }
}
