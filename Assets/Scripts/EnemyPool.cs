using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField]
    GameObject[] enemy;
    public static int enemyCount = 1, enemyType = 0;
    int randomEnemy = 0;
    Queue<GameObject> availableObjects = new Queue<GameObject>();
    public static EnemyPool Instance { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        GrowPool();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void GrowPool()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            var instanceToAdd = Instantiate(enemy[enemyType]);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);            
        }
        for (int i = 0; i < EnemySpawner.numOfRandomEnemies; i++)
        {
            randomEnemy = Random.Range(0, 4);
            var instanceToAdd = Instantiate(enemy[randomEnemy]);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }
    public GameObject GetFromPool()
    {
        if(availableObjects.Count == 0)
        {
            GrowPool();
        }
        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }
}
