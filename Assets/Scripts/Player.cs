using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    public Joystick moveJoystick, rotateJoystick;
    float moveHorizontal;
    float moveVertical;
    public float speed, maxSpeed, boostSpeed, reviveRadius;
    public Camera cam;
    public Slider boostBar;
    public GameObject shot;
    public Transform shotSpawn;
    public bool isBoosting = false;
    public Button boostButton, fireButton;
    float camOffset = 6, timeOffset = 0, enteranceTimeOffset = 0;
    float camOffsetRatio = 0;
    public LayerMask shotLayer, enemiesLayer;
    public GameObject reviveBurst, fireBurst;
    GameObject reviveBurstTemp, fireBurstTemp;
    AudioManager audioManager;
    public ParticleSystem playerBoom, playerSparks;
    public float ratio = 1;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        reviveBurstTemp = Instantiate(reviveBurst, transform);
        fireBurstTemp = Instantiate(fireBurst, transform);
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audio");
        }
    }
    void FixedUpdate()
    {
        float posX, posY;
        posX = transform.position.x;
        posY = transform.position.y;        
        Movement(moveHorizontal, moveVertical);
        if (GameManager.isPlaying == true)
        {            
            if (camOffsetRatio < 1)
            {
                timeOffset += Time.deltaTime;
                camOffsetRatio = timeOffset / 2;
            }
            camOffset = Mathf.Lerp(4f, 0f, camOffsetRatio);
        }
        else
        {
            if (camOffsetRatio < 1)
            {
                enteranceTimeOffset += Time.deltaTime;
                camOffsetRatio = enteranceTimeOffset / 6;
            }
            camOffset = Mathf.Lerp(6f, 4f, camOffsetRatio);
            transform.Translate(new Vector3(0f, 0.05f, 0f));
        }
        if(camOffsetRatio>=1f)
        {
            camOffsetRatio = 0f;
        }
        cam.transform.position = new Vector3(posX, posY + camOffset, -10);
        Rotate();
        boostBar.value = ratio;
    }
    void Movement(float moveX, float moveY)
    {
        moveHorizontal = moveJoystick.Horizontal * speed;
        moveVertical = moveJoystick.Vertical * speed;
        Vector3 movement = new Vector3(moveX, moveY, 0f);
        rb.AddForce(movement);
        if(isBoosting == false)
        {            
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
            boostButton.GetComponent<Button>().enabled = true;
        }
        else
        {
            boostButton.GetComponent<Button>().enabled = false;
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 3 * maxSpeed);
        }
    }
    void Rotate()
    {
        Vector3 rotation = new Vector3(rotateJoystick.Horizontal, rotateJoystick.Vertical, 4096f);        
        if (rotation.x != 0f && rotation.y != 0f)
        {
            transform.rotation = Quaternion.LookRotation(rotation, Vector3.back);
        }
    }
    public void Firing()
    {
        GameObject clone = Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        fireBurstTemp.GetComponent<ParticleSystem>().Play();
        audioManager.PlaySound("player_shot");
        Destroy(clone, 3f);
    }
    public void Revive()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, reviveRadius, shotLayer);
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, reviveRadius/2.5f, enemiesLayer);
        ratio = 1;
        isBoosting = false;
        foreach (Collider2D obj in objects)
        {
            Destroy(obj.gameObject);
        }
        foreach (Collider2D obj in enemies)
        {
            Destroy(obj.gameObject);
            EnemySpawner.enemyCount--;
        }

        reviveBurstTemp.GetComponent<ParticleSystem>().Play();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            explosion();
            this.gameObject.SetActive(false);
            audioManager.PlaySound("player_dead");
            audioManager.PlaySound("enemy_dead");
            GameManager.playerDead = true;
            Destroy(collision.gameObject);
            EnemySpawner.enemyCount--;
            fireButton.GetComponent<Button>().enabled = false;
            boostButton.GetComponent<Button>().enabled = false;
        }
        if (collision.tag == "Enemy shot" || collision.tag == "missile")
        {
            explosion();
            audioManager.PlaySound("player_dead");
            this.gameObject.SetActive(false);
            GameManager.playerDead = true;
            Destroy(collision.gameObject);
            fireButton.GetComponent<Button>().enabled = false;
            boostButton.GetComponent<Button>().enabled = false;
        }
    }
    void explosion()
    {
        ParticleSystem boom = Instantiate(playerBoom) as ParticleSystem;
        ParticleSystem sparks = Instantiate(playerSparks) as ParticleSystem;
        boom.transform.position = transform.position;
        boom.Play();
        sparks.transform.position = transform.position;
        sparks.Play();
        Destroy(boom.gameObject, 3f);
        Destroy(sparks.gameObject, 3f);
    }
    public void Boost()
    {
        isBoosting = true;
        rb.AddForce(new Vector2(rb.velocity.x * boostSpeed, rb.velocity.y * boostSpeed), ForceMode2D.Impulse);
        StartCoroutine("Boosting");
    }
    public IEnumerator Boosting()
    {
        ratio = 0;
        yield return new WaitForSecondsRealtime(1.5f);
        while (ratio < 1f)
        {
            ratio += Time.deltaTime / 10;
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, Mathf.Lerp(rb.velocity.magnitude, maxSpeed, ratio));
            yield return new WaitForFixedUpdate();
            Debug.Log(ratio);
        }    
        isBoosting = false;
        
        yield return null;
    }
}

