using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointer : MonoBehaviour
{
    Transform target;
    Vector3 dir;
    float hideDistance = 8;
    public int getId = 0;
    private void Start()
    {
        target = GameObject.Find("enemy" + getId).gameObject.transform;
        dir = new Vector3(0, 0, 0);
    }
    private void Update()
    {
        if(target != null && target.gameObject.activeSelf == true)
        {
            target = GameObject.Find("enemy" + getId).gameObject.transform;
            dir = target.position - transform.position;
        }
        else
        {
            Destroy(this.gameObject);
        }
        if (dir.magnitude < hideDistance)
        {
            SetChildrenActive(false);
        }
        else
        {
            SetChildrenActive(true);
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }        
    }
    void SetChildrenActive(bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }  
}
