using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public static MapGenerator instance = null;

    public GameObject map1Prefab;

    private void Awake() {
        if (instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public void newMap() {
        ThreadManager.ExecuteOnMainThread(() => {
            MapGenerator.instance.createMap();
        });
    }

    public void createMap() {
        Instantiate(map1Prefab, new Vector3(-2.315202f,-1.984119f,-9.144831f), Quaternion.identity);
    }
}
