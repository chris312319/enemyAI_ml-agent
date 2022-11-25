using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMove : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject HitTarget;
    public float speed = 10;    //速度  
    public float rotationAngle = 60;
    public float distanceToTarget;   //兩者之間的距離  
    public bool move = true;
    public EnemyAIAgent agent;
    public bool isHit;
    public float Cur_weaponSpeed;
    public Vector3 pre_position;

    void Start()
    {
        pre_position = this.transform.position;
        isHit = false;
        distanceToTarget = Vector3.Distance(this.transform.position, HitTarget.transform.position);
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        if (HitTarget == null)
        {
            if (GameObject.Find("mixamorig:Spine") != null)
            {
                HitTarget = GameObject.Find("mixamorig:Spine");
                StartCoroutine(Move());
            }
        }
        Cur_weaponSpeed = (this.transform.position - pre_position).magnitude / Time.deltaTime;
        pre_position = this.transform.position;
        //var direction = HitTarget.transform.position - transform.position;
        //transform.Translate(direction.normalized * Time.deltaTime * speed, Space.World);
    }
    public void OnTriggerStay(Collider collision)
    {
        if(collision.gameObject.name == "Shield")
        {
            isHit = true;
            if (agent.block > 0) agent.SetReward(1.5f - 0.5f * (agent.timer - 1) - 0.5f * agent.rotateangle);
            else agent.SetReward(0);
            Debug.Log("檔到");
            agent.EndEpisode();
        }
        if (collision.gameObject.name == "mixamorig:Spine")
        {
            isHit = true;
            agent.blood = 0;
            if (agent.block <= 0) agent.SetReward(-1.0f - 0.5f * (agent.timer - 1) - 0.5f * agent.rotateangle);
            else if(agent.block > 0) agent.SetReward(- 0.5f * (agent.timer - 1) - 0.5f * agent.rotateangle);
            Debug.Log("沒檔到");
            agent.EndEpisode();
        }
    }

    IEnumerator Move()
    {
        while (move)  //移動到目標點停止移動
        {
            Vector3 targetPos = HitTarget.transform.position;

            //讓始終它朝着目標  
            this.transform.LookAt(targetPos);

            //計算弧線中的夾角  
            float angle = Mathf.Min(1, Vector3.Distance(this.transform.position, targetPos) / distanceToTarget) * rotationAngle;
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);
            float currentDist = Vector3.Distance(this.transform.position, HitTarget.transform.position);           
            this.transform.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));
            yield return null;
        }
    }
}
