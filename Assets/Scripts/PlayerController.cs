using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public GameObject playerModel;
    public Weapon playerGun;
    public GameObject player;
    public Camera playerCam;
    public Transform head;
    public Animator animations;
    public Keys keys { get; set; }
    public TextMesh displayName;
    public string username = "";
    public int id = -1;
    public bool enemy = false;

    private bool alive = true;
    private bool isGrounded;
    private bool readyToJump = true;
    private float jumpDelay = 0.6f;
    private bool finishDashDelay = true;
    private float dashDelay = 0.8f;

    public ParticleSystem dashPS;
    public GameObject dashGO;
    public GameObject impactPS;
    public GameObject bloodPS;
    public GameObject jumpPS;
    public GameObject landPS;

    private float fireRate = 2f;
    private float nextTimeToFire = 0f;

    private Quaternion camRotation;

    public float health = 100f;
    public float block = 100f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        keys = new Keys();
    }

    void FixedUpdate() {
        if(alive) {
            running();
            jump();
            dash();
            shoot();
            
            if(!enemy) updateHelth();
            if(!enemy) sendKeys();
        }

        displayName.transform.LookAt(Camera.main.transform);
        displayName.transform.Rotate(0, 180, 0);
    }
    
    void LateUpdate() {
        if(alive) head.transform.localRotation = camRotation;
    }

    private void updateHelth() {
        GameController.instance.setHealth(health);
        GameController.instance.setBlock(block);
    }

    private void shoot() {
        if(keys.mouseLeft && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 2f / fireRate;
            playerGun.shoot();
            SoundManager.PlaySound("shoot");
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
            SoundManager.PlaySound("land");
        }

        if(!isGrounded && !animations.GetBool("Jumping") && !animations.GetBool("Running")) {
            animations.SetBool("Jumping", true);
        }

        if(keys.jumping && isGrounded && readyToJump && !animations.GetBool("Jumping")){
            animations.SetBool("Jumping", true);
            Destroy(Instantiate(jumpPS, new Vector3(player.transform.position.x, player.transform.position.y-1, player.transform.position.z), Quaternion.identity), 1);
            SoundManager.PlaySound("jump");
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
                SoundManager.PlaySound("dash");
            } else if (keys.x != 0) {
                dashGO.transform.localPosition = new Vector3(keys.x * 1.2f, 0, 0);
                dashGO.transform.localRotation = Quaternion.Euler(0, keys.x * -90, 0);
                dashPS.Play();
                SoundManager.PlaySound("dash");
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
            GameController.instance.setLog("Player [id: "+id+"] has disconnect!");
            Destroy(player);
        });
    }

    public void creatShootImpact(Packet packet) {
        ThreadManager.ExecuteOnMainThread(() => {
            Vector3 hitPoint = packet.ReadVector3();
            Quaternion rotationPoint = packet.ReadQuaternion();
            bool isPlayer = packet.ReadBool();
            Destroy(Instantiate(impactPS, hitPoint, rotationPoint), 1);
            if(isPlayer) {
                Destroy(Instantiate(bloodPS, hitPoint, rotationPoint), 1);
                SoundManager.PlaySound("hit");
                if(enemy) GameController.instance.enableDamage();
            }
        });
    }

    public void killPlayer(Packet packet) {
        alive = false;
        playerModel.SetActive(false);
        playerGun.setVisible(false);
    }

    public void respawnPlayer(Packet packet) {
        alive = true;
        playerModel.SetActive(true);
        playerGun.setVisible(true);
    }

    public void setId(int id) {
        this.id = id;
    }

    public void setUsername(string username) {
        this.username = username;
    }

    public void makeItEnemy() {
        enemy = true;
        playerCam.enabled = false;
        playerCam.tag = "Enemy";
        displayName.text = username;
        setLayerRecursively(playerModel, LayerMask.NameToLayer("Enemy"));
    }

    private void setLayerRecursively(GameObject obj, LayerMask newLayer) {
        if(null == obj) return;
       
        obj.layer = newLayer;
       
        foreach(Transform child in obj.transform) {
            if (null == child) continue;
            setLayerRecursively(child.gameObject, newLayer);
        }
    }
}
