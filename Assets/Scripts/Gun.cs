using UnityEngine;

public class Gun : MonoBehaviour {

    // private float damage = 10f;
    // private float range = 100f;
    // private float fireRate = 2f;
    // private float nextTimeToFire = 0f;
    //public Camera playerCam;
    // public ParticleSystem shootPS;
    // public GameObject impactEffectPS;

    // void Update() {
    //     if(Input.GetMouseButton(0) && Time.time >= nextTimeToFire) {
    //         nextTimeToFire = Time.time + 2f / fireRate;

    //         shootPS.Play();
    //         shoot();
    //     }
    // }

    // private void shoot() {
    //     RaycastHit hit;
    //     if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range)) {
    //         Debug.Log(hit.transform.name);

    //         // hit.rigidbody.AddForce(-hit.normal * force);

    //         GameObject impactGo = Instantiate(impactEffectPS, hit.point, Quaternion.LookRotation(hit.normal));
    //         Destroy(impactGo, 1f);
    //     }
    // }
}
