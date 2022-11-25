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

public class BallAgent : Agent
{
    Rigidbody rBody;
    public float speed = 10;
    void Start()
    {
        // 获取Object中的Rigidbody组件，本例中即Ball的Rigidbody组件
        rBody = GetComponent<Rigidbody>();
    }

    // 获取Target对象
    // Script中的public属性会显示在Object的该Script组件中，可以通过拖拽来指定要关联的对象
    public Transform Target;
    public override void OnEpisodeBegin()
    {
        // this指代Ball Object，if语句用于判断Ball的y坐标是否为负(即Ball是否掉落Floor)
        if (this.transform.localPosition.y < 0)
        {
            // 如果Ball掉落，则在新一轮游戏开始时重置Ball到Floor中央
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // 在新一轮游戏开始时，把Target放置在Floor上的一个随机位置
        Target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.5f,
                                           Random.value * 8 - 4);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target和Agent的位置信息
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent的速度信息
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        // Actions的size = 2, 指示Ball在X轴和Z轴方向(即水平面)上的移动信号
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction.ContinuousActions[0];
        controlSignal.z = vectorAction.ContinuousActions[1];
        // 通过在Rigidbody上作用力来使Ball移动
        rBody.AddForce(controlSignal * speed);

        // 获取Ball移动后与Target的距离
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // 通过Ball和Target之间的距离来判断Ball是否碰触了Target
        if (distanceToTarget < 1.42f)
        {
            // 如果Ball碰触了Target, 获得1.0的奖励，并结束本次Episode
            SetReward(1.0f);
            EndEpisode();
        }

        // 判断Ball是否掉落Floor，如果掉落则之间结束本次Episode
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var inputaction = actionsOut.ContinuousActions;
        inputaction[0] = Input.GetAxis("Horizontal");
        inputaction[1] = Input.GetAxis("Vertical");
    }
}
