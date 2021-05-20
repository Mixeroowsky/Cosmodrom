using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    GameObject playerPosition;
    public float speed;
    public Transform shotSpawn;
    public GameObject shot, missile;
    int cloneCount = 0;
    Rigidbody2D rb;
    AudioManager audioManager;
    public ParticleSystem enemyBoom;
    int tankHp = 3;
    float changeShotZone = .3f;
    public Slider slider;
    public Vector3 hpBarOffset;
    private void Start()
    {
        audioManager = AudioManager.instance;
        rb = this.GetComponent<Rigidbody2D>();     
        if(gameObject.tag != "kamikaze" && gameObject.tag != "rocket")
        {
            StartCoroutine("Shooting");
        }     
        if(gameObject.tag == "rocket")
        {
            StartCoroutine("GuidedMissile");
        }
        if (audioManager == null)
        {
            Debug.LogError("No audio");
        }
    }

    private void FixedUpdate()
    {
        if(gameObject.tag == "tank")
        {

            slider.value = tankHp;
            slider.gameObject.SetActive(tankHp < 3);
            slider.transform.position = Camera.main.WorldToScreenPoint(transform.position + hpBarOffset);
        }
        playerPosition = GameObject.FindGameObjectWithTag("Player");
        if(playerPosition == null || GameManager.playerDead == true)
        {           
            Debug.Log("No player");
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, playerPosition.transform.position, Time.deltaTime * speed);
            Vector3 direction = playerPosition.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Shot")
        {
            switch (gameObject.tag)
            {
                case "tank":
                    tankHp--;
                    Destroy(collision.gameObject);
                    if (tankHp == 0)
                    {
                        enemyDead(300, collision.gameObject);
                    }
                    Debug.Log(tankHp);
                    break;
                case "rocket":
                    enemyDead(200, collision.gameObject);
                    break;
                case "enemy":
                    enemyDead(100, collision.gameObject);
                    break;
                case "smol":
                    enemyDead(200, collision.gameObject);
                    break;
                case "kamikaze":
                    enemyDead(200, collision.gameObject);
                    break;
            }
        }
    }

    IEnumerator Shooting()
    {
        while(true)
        {
            if (cloneCount < 3)
            {
                GameObject clone = Instantiate(shot, shotSpawn);
                if(gameObject.tag == "tank")
                {
                    changeShotZone = -changeShotZone;
                    Vector3 localShotZone = transform.TransformPoint(changeShotZone, .2f, 0);
                    shotSpawn.position = localShotZone;
                }
                audioManager.PlaySound("enemy_shot");
                cloneCount++;
                yield return new WaitForSeconds(Random.Range(2f, 5f));
                Destroy(clone, 10f);
                cloneCount--;
            }
        }
    }
    IEnumerator GuidedMissile()
    {
        while(true)
        {
            if (cloneCount < 3)
            {
                GameObject clone = Instantiate(missile, shotSpawn.position,Quaternion.identity);                
                audioManager.PlaySound("enemy_missile");
                cloneCount++;
                yield return new WaitForSeconds(Random.Range(4f, 9f));
                Destroy(clone, 10f);
                cloneCount--;
            }
        }
    }
    void enemyDead(int points, GameObject collision)
    {
        GameManager.score += points;
        ParticleSystem boom = Instantiate(enemyBoom) as ParticleSystem;
        boom.transform.position = transform.position;
        boom.Play();
        Destroy(boom.gameObject, 2f);
        EnemySpawner.enemyCount--;
        audioManager.PlaySound("enemy_dead");
        Destroy(collision.gameObject);
        Destroy(this.gameObject);
        //EnemyPool.Instance.AddToPool(gameObject);
    }
}
