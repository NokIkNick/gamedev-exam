using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    [SerializeField] private GameObject objectToPool;
    [SerializeField] private int poolSize = 10;
    private Queue<GameObject> pool;

    private void Awake() {
        //InitializePool();
    }
    private void InitializePool() {
          for (int i = 0; i < poolSize; i++) {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    public GameObject GetPooledObject() {
        if(pool == null) {
            pool = new Queue<GameObject>();
            InitializePool();
        }
        if (pool.Count > 0) {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        } else {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(true);
            return obj;
        }
    }

    public void ReturnObjectToPool(GameObject obj) {
        obj.SetActive(false);
        if (!pool.Contains(obj)) {
            pool.Enqueue(obj);
        }
        else {
            Debug.LogWarning("Object is already in the pool.");
        }
    }
}
