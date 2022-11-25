using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using Unity.Barracuda;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine.Serialization;

public class EnemyAIAgent : Agent
{
    public GameObject Target;
    public GameObject Shield;
    public Animator enemy_ani;
    public GameObject HitTarget;
    public float block = 0;
    public float rotate = 0;
    public float targetangle = 0;
    public float targetdis = 0;
    public float rotatespeed = 0;
    public float blood = 1;
    public float speed = 10;
    public float timer = 0;
    public float rotateangle = 0;
    public float enemyminspeed = 0;
    public float enemymaxspeed = 0;
    public float enemyminangle = 0;
    public float enemymaxangle = 0;
    public bool isTest = false;
    void Start()
    {
        enemy_ani = this.GetComponent<Animator>();
    }


    public override void OnEpisodeBegin()
    {
        if (blood < 1)
        {
            blood = 1;
            timer = 0;
        }
        timer = 0;
        rotateangle = 0;
        block = 0;
        rotate = 0;
        Shield.transform.localPosition = new Vector3(-0.033f, 0.153f, -0.055f);
        if (isTest)
        {
            this.transform.localPosition = new Vector3(0, 0, -4f);
            this.transform.localEulerAngles = new Vector3(0, 0, 0);
            Target.transform.localPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(0.3f, 5f), 5);
            Target.GetComponent<WeaponMove>().speed = Random.Range(enemyminspeed, enemymaxspeed);
            Target.GetComponent<WeaponMove>().rotationAngle = Random.Range(enemyminangle, enemymaxangle);
            Target.GetComponent<WeaponMove>().distanceToTarget = Vector3.Distance(Target.GetComponent<WeaponMove>().gameObject.transform.position, Target.GetComponent<WeaponMove>().HitTarget.transform.position);    
        }
        Target.GetComponent<WeaponMove>().isHit = false;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Target.transform.position);
        sensor.AddObservation(HitTarget.transform.position);

        sensor.AddObservation(block);
        sensor.AddObservation(rotate);
        sensor.AddObservation(targetangle);
        sensor.AddObservation(targetdis);
        sensor.AddObservation(Target.GetComponent<WeaponMove>().Cur_weaponSpeed);
    }
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        block = vectorAction.ContinuousActions[0];
        rotate = vectorAction.ContinuousActions[1];
        if (block > 0)
        {
            enemy_ani.SetBool("block", true);
            timer += Time.deltaTime;
        } 
        else if (block <= 0) enemy_ani.SetBool("block", false);
        if (rotate != 0)
        {
            rotateangle += Mathf.Abs(rotate) * Time.deltaTime;
        }
        this.transform.localEulerAngles += new Vector3(0, rotatespeed * rotate * Time.deltaTime, 0);
        targetdis = Vector3.Distance(HitTarget.transform.position, Target.transform.position);
        targetangle = CalAngle(targetdis);
       if (Target.GetComponent<WeaponMove>().isHit)
        {
            if(blood == 0)
            {
                if(block <= 0) SetReward(-1.0f -0.5f * (timer - 1) - 0.5f * rotateangle);
                else if(block > 0) SetReward(- 0.5f * (timer - 1) - 0.5f * rotateangle);
                Debug.Log("沒檔到");
                timer = 0;
                rotateangle = 0;
                EndEpisode();
            }
            else if(blood == 1)
            {
                if(block > 0) SetReward(1.5f - 0.5f * (timer - 1) - 0.5f * rotateangle);
                else SetReward(0);
                Debug.Log("檔到");
                timer = 0;
                rotateangle = 0;
                EndEpisode();
            }
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var inputaction = actionsOut.ContinuousActions;
        switch (Input.GetKey(KeyCode.Space))
        {
            case true:
                inputaction[0] = 1;
                break;
            case false:
                inputaction[0] = -1;
                break;
        }
        inputaction[1] = Input.GetAxis("Horizontal");
    }
    public Vector3 Vector3xz(Vector3 position)
    {
        Vector3 newposition = new Vector3(position.x, 0, position.z);
        return newposition;
    }
    public float CalAngle(float Distance)
    {
        Distance = Vector3.Distance(HitTarget.transform.position, Target.transform.position);
        Vector3 targetdir = (Vector3xz(Target.transform.position) - Vector3xz(HitTarget.transform.position));
        float Angle = Vector3.Angle(targetdir, transform.forward);
        if(Vector3.Angle(targetdir, transform.right) > Vector3.Angle(targetdir, -transform.right))
        {
            return -Angle;
        }
        else return Angle;
    }
}
