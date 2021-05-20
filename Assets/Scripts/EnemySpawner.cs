using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EnemySpawner : MonoBehaviour
{
    public int numObjects = 5;
    public GameObject player, pointer;
    public GameObject[] enemies;
    public static int enemyCount = 0, waveNumber = 1, numOfRandomEnemies = 0;
    int enemyIndex = 0;
    public TextMeshProUGUI nextWaveText;
    bool nextWave = false;
    public int enemyId = 0;
    bool stopCounting = false;
    Vector3 playerPosition;
    void Start()
    {
        StartCoroutine("EnemySpawn");
    }
    void Update()
    {
        playerPosition = player.transform.position;
        while (nextWave==true)
        {
            StartCoroutine("EnemySpawn");
        }
        switch (waveNumber)
        {
            case 5:
                stopCounting = true;
                break;
            case 15:
                stopCounting = false;
                break;
            default:
                break;
        }
    }
    IEnumerator EnemySpawn()
    {
        enemyId = 0;
        nextWave = false;        
        
        for (int i = 0; i < numObjects; i++)
        {
            enemyIndex = 0;
            enemySpawn(enemyIndex);
            yield return new WaitForSeconds(3f);            
        }
        for (int i = 0; i < numOfRandomEnemies; i++)
        {
            enemyIndex = Random.Range(1, 5);
            enemySpawn(enemyIndex);
            yield return new WaitForSeconds(3f);
        }
        while(enemyCount>0)
        {
            yield return null;
        }
        while(nextWaveText.alpha<1)
        {
            nextWaveText.alpha += Time.deltaTime / 2;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(3f);
        while (nextWaveText.alpha > 0)
        {
            nextWaveText.alpha -= Time.deltaTime / 2;
            yield return new WaitForEndOfFrame();
        }
        if (stopCounting == true && waveNumber < 15)
        {
            numObjects -= 1;
            numOfRandomEnemies += 1;
            Debug.Log(numObjects);
            Debug.Log(numOfRandomEnemies);

        }
        else if(stopCounting == true && waveNumber >= 15)
        {
            numObjects += 1;
            numOfRandomEnemies += 1;
        }
        else
        {
            numObjects += 1;
        }
        nextWave = true;
        waveNumber += 1;
    }
    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }
    void enemySpawn(int index)
    {
        Vector3 pos = RandomCircle(playerPosition, 40f);
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, playerPosition - pos);
        //var enemyClone = EnemyPool.Instance.GetFromPool();        
        var enemyClone = Instantiate(enemies[index]);
        enemyClone.transform.position = pos;
        enemyClone.name = "enemy" + enemyId;
        GameObject pointerClone = Instantiate(pointer, player.transform);
        EnemyPointer pointerId = pointerClone.GetComponent<EnemyPointer>();
        pointerId.getId = enemyId;
        enemyId++;
        enemyCount++;
    }
}
