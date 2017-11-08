using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    public GameObject bulletPrefab;
    public GameObject bulletPrefabBig;
    public GameObject explosionPrefab;
    public int bulletsInstantiated;
    public int bulletsBigInstantiated;
    public int explosionsInstantiated;
    public List<GameObject> bulletList;
    public List<GameObject> bulletListBig;
    public List<GameObject> explosionsList;

    private GameObject parent;
    private GameObject bulletParent;
    private GameObject bulletBigParent;
    private GameObject explosionParent;

	void Awake () {
        bulletList = new List<GameObject>();
        bulletListBig = new List<GameObject>();
        explosionsList = new List<GameObject>();
        parent = new GameObject();
        bulletParent = new GameObject();
        bulletBigParent = new GameObject();
        explosionParent = new GameObject();
        SpawnObjects();
	}

    void SpawnObjects() {
        parent.name = "PooledObjects";
        parent.AddComponent<DontDestroyOnLoad>();
        bulletParent.name = "Bullets";
        bulletParent.transform.parent = parent.transform;
        bulletBigParent.name = "BulletsBig";
        bulletBigParent.transform.parent = parent.transform;
        explosionParent.name = "Explosions";
        explosionParent.transform.parent = parent.transform;

        for (int i = 0; i < bulletsInstantiated; i++) {
            GameObject g = Instantiate<GameObject>(bulletPrefab);
            g.transform.parent = bulletParent.transform;
            g.SetActive(false);
            bulletList.Add(g);
        }

        for (int i = 0; i < bulletsBigInstantiated; i++) {
            GameObject g = Instantiate<GameObject>(bulletPrefabBig);
            g.transform.parent = bulletBigParent.transform;
            g.SetActive(false);
            bulletListBig.Add(g);
        }

        for (int i = 0; i < explosionsInstantiated; i++) {
            GameObject g = Instantiate<GameObject>(explosionPrefab);
            g.transform.parent = explosionParent.transform;
            g.SetActive(false);
            explosionsList.Add(g);
        }

    }

    public GameObject GetPooledBullet() {
        for (int i = 0; i < bulletList.Count; i++) {
            if (!bulletList[i].activeInHierarchy) {
                return bulletList[i];
            }
        }

        GameObject g = Instantiate<GameObject>(bulletPrefab);
        g.transform.parent = bulletParent.transform;
        bulletList.Add(g);
        g.SetActive(false);
        return g;
    }

    public GameObject GetPooledBigBullet() {
        for (int i = 0; i < bulletListBig.Count; i++) {
            if (!bulletListBig[i].activeInHierarchy) {
                return bulletListBig[i];
            }
        }

        GameObject g = Instantiate<GameObject>(bulletPrefabBig);
        g.transform.parent = bulletBigParent.transform;
        bulletListBig.Add(g);
        g.SetActive(false);
        return g;
    }

    public GameObject GetPooledExplosion() {
        for (int i = 0; i < explosionsList.Count; i++) {
            if (!explosionsList[i].activeInHierarchy) {
                return explosionsList[i];
            }
        }

        GameObject g = Instantiate<GameObject>(explosionPrefab);
        g.transform.parent = explosionParent.transform;
        explosionsList.Add(g);
        g.SetActive(false);
        return g;
    }

}
