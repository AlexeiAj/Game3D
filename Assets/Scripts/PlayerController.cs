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
    private bool readyToJump = true;
    private float jumpDelay = 0.6f;
    private bool finishDashDelay = true;
    private float dashDelay = 0.8f;

    public ParticleSystem shootPS;
    public ParticleSystem dashPS;
    public GameObject dashGO;
    public GameObject impactPS;
    public GameObject jumpPS;
    public GameObject landPS;

    private float fireRate = 2f;
    private float nextTimeToFire = 0f;

    private Quaternion camRotation;

    public float health = 100f;
    public float block = 100f;

    void Start() {
        if(!MenuController.instance.isAndroid()) Cursor.lockState = CursorLockMode.Locked;
        keys = new Keys();
    }

    void FixedUpdate() {
        running();
        jump();
        dash();
        shoot();
        
        if(!enemy) updateHelth();
        if(!enemy) sendKeys();
    }
    
    void LateUpdate() {
        head.transform.localRotation = camRotation;
    }

    private void updateHelth() {
        MenuController.instance.setHealth(health);
        MenuController.instance.setBlock(block);
    }

    private void shoot() {
        if(keys.mouseLeft && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 2f / fireRate;
            shootPS.Play();
        }
    }

    private void running() {
        if(!isGrounded && animations.GetBool("Running")){
            animations.SetBool("Running", false);
        } 

        if((keys.x == 0 && keys.y == 0) && isGrounded && animations.GetBool("Running")){
            animations.SetBool("Running", false);
        } 

        if((keys.x != 0 || keys.y != 0) && isGrounded && !animations.GetBool("Running")) {
            animations.SetBool("Running", true);
        }
    }

    private void jump() {
        if(isGrounded && animations.GetBool("Jumping") && readyToJump){
            animations.SetBool("Jumping", false);
            Destroy(Instantiate(landPS, new Vector3(player.transform.position.x, player.transform.position.y-1, player.transform.position.z), Quaternion.identity), 1);
        }

        if(!isGrounded && !animations.GetBool("Jumping") && !animations.GetBool("Running")) {
            animations.SetBool("Jumping", true);
        }

        if(keys.jumping && isGrounded && readyToJump && !animations.GetBool("Jumping")){
            animations.SetBool("Jumping", true);
            Destroy(Instantiate(jumpPS, new Vector3(player.transform.position.x, player.transform.position.y-1, player.transform.position.z), Quaternion.identity), 1);
            Invoke("restartJump", jumpDelay);
            readyToJump = false;
        }
    }

    private void dash() {
        if(keys.mouseRight && !isGrounded && finishDashDelay){
            finishDashDelay = false;

            if (keys.y != 0) {
                dashGO.transform.localPosition = new Vector3(0, 0, keys.y * 1.2f);
                dashGO.transform.localRotation = Quaternion.Euler(0, keys.y >= 0 ? 180 : 0, 0);
                dashPS.Play();
            } else if (keys.x != 0) {
                dashGO.transform.localPosition = new Vector3(keys.x * 1.2f, 0, 0);
                dashGO.transform.localRotation = Quaternion.Euler(0, keys.x * -90, 0);
                dashPS.Play();
            }

            Invoke("resetFinishDashDelay", dashDelay);
        }
    }

    private void restartJump() {
        readyToJump = true;
    }

    private void resetFinishDashDelay() {
        finishDashDelay = true;
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
        health = packet.ReadFloat();
        block = packet.ReadFloat();
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
            MenuController.instance.setLog("Player [id: "+id+"] has disconnect!");
            Destroy(player);
        });
    }

    public void creatShootImpact(Packet packet) {
        ThreadManager.ExecuteOnMainThread(() => {
            Vector3 hitPoint = packet.ReadVector3();
            Quaternion rotationPoint = packet.ReadQuaternion();
            Destroy(Instantiate(impactPS, hitPoint, rotationPoint), 1);
        });
    }

    public void setId(int id) {
        this.id = id;
    }

    public void setUsername(string username) {
        this.username = username;
    }
}
