using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    public float speed;    
    AudioManager audioManager;
    GameObject playerPosition;
    Rigidbody2D rb;
    public ParticleSystem missileBoom;
    void Start()
    {        
        rb = GetComponent<Rigidbody2D>(); 
        if(gameObject.tag != "missile")
        {
            rb.velocity = transform.up * speed;
        }        
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audio");
        }
    }
    private void FixedUpdate()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player");
        if (gameObject.tag == "missile")
        {
            missileBehaviour();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Shot" && gameObject.tag == "missile")
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }    
    void missileBehaviour()
    {
        if (playerPosition == null || GameManager.playerDead == true)
        {
            Debug.Log("No player");
        }
        else
        {
            Vector3 direction = playerPosition.transform.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, playerPosition.transform.position, Time.deltaTime * speed);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f;
        }
    }
    void explosion()
    {
        ParticleSystem boom = Instantiate(missileBoom) as ParticleSystem;
        boom.transform.position = transform.position;
        boom.Play();
        Destroy(boom.gameObject, boom.main.duration);
    }
    private void OnDestroy()
    {
        if(gameObject.tag == "missile")
        {
            explosion();
        }
        
    }
    
}
