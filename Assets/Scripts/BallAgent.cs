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
        // ��ȡObject�е�Rigidbody����������м�Ball��Rigidbody���
        rBody = GetComponent<Rigidbody>();
    }

    // ��ȡTarget����
    // Script�е�public���Ի���ʾ��Object�ĸ�Script����У�����ͨ����ק��ָ��Ҫ�����Ķ���
    public Transform Target;
    public override void OnEpisodeBegin()
    {
        // thisָ��Ball Object��if��������ж�Ball��y�����Ƿ�Ϊ��(��Ball�Ƿ����Floor)
        if (this.transform.localPosition.y < 0)
        {
            // ���Ball���䣬������һ����Ϸ��ʼʱ����Ball��Floor����
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // ����һ����Ϸ��ʼʱ����Target������Floor�ϵ�һ�����λ��
        Target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.5f,
                                           Random.value * 8 - 4);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target��Agent��λ����Ϣ
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent���ٶ���Ϣ
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        // Actions��size = 2, ָʾBall��X���Z�᷽��(��ˮƽ��)�ϵ��ƶ��ź�
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction.ContinuousActions[0];
        controlSignal.z = vectorAction.ContinuousActions[1];
        // ͨ����Rigidbody����������ʹBall�ƶ�
        rBody.AddForce(controlSignal * speed);

        // ��ȡBall�ƶ�����Target�ľ���
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // ͨ��Ball��Target֮��ľ������ж�Ball�Ƿ�������Target
        if (distanceToTarget < 1.42f)
        {
            // ���Ball������Target, ���1.0�Ľ���������������Episode
            SetReward(1.0f);
            EndEpisode();
        }

        // �ж�Ball�Ƿ����Floor�����������֮���������Episode
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
