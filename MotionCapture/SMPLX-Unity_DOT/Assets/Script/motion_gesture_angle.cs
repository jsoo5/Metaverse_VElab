using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

/*
�ش� �ڵ忡�� Ȯ���ؾ��� ���� 
1. ����: quat >> rm >> euler�� ��ȯ�Ͽ� ��µǴ� ���� Ȯ��
2. smpl�� ���� ����Ʈ���� ��µǴ� euler�� �����̶� 1���̶� ��.
 */
public class motion_gesture_angle : MonoBehaviour
{
    GameObject from_playscript_dot_cs;
    Text print_msg3;
    Text print_msg2;

    List<Quaternion> finalQuatList = new List<Quaternion>
    {
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
    };
    List<Vector3> quat_to_eulerList = new List<Vector3>
    {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f),
    };
    float timer;
    float waitingTime;

    bool is_playing_avatar;
    bool is_left_shoulder_ready, is_left_elbow_ready = false;
    bool is_left_arm_ready, is_right_arm_ready = false;
    bool is_standby = false;
    int standby_counter, other_pose_counter = 0;

    public string direction = "Default"; // Kiosk Trigger

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    //����ó �غ��ڼ��� üũ�ϱ� ���� �Լ�
    void Standby() 
    {

        // �̰��� ���Ȱ� ������ ������ ���̵Ǵ� ��ũ��Ʈ�� �ۼ� -  ģ������ ���� �ϼ�...
        //left condiiton
        //left_shoulder
        //if (quat_to_eulerList[4].x < 40 && quat_to_eulerList[4].y > 300 && quat_to_eulerList[4].y < 340 && quat_to_eulerList[4].z > 30 && quat_to_eulerList[4].z < 70)
        //{ is_left_shoulder_ready = true; }
        //    //left_elbow
        //if (quat_to_eulerList[5].x < 70 && quat_to_eulerList[5].y > 170 && quat_to_eulerList[5].y < 260 && quat_to_eulerList[5].z > 300 && quat_to_eulerList[5].z < 60) 
        //{ is_left_elbow_ready = true; }
        //if (is_left_shoulder_ready && is_left_shoulder_ready) is_left_arm_ready = true;

        //ģ���������� Ű����
        is_left_arm_ready = true;

        //right condition
        if (quat_to_eulerList[3].y > 90.0f && quat_to_eulerList[3].y < 120.0f && quat_to_eulerList[3].z > 0.0f && quat_to_eulerList[3].z < 40.0f)
        { is_right_arm_ready = true; }
        else 
        {
            is_right_arm_ready = false;
        }



        if (is_left_arm_ready && is_right_arm_ready)
        {
            standby_counter ++;
            other_pose_counter = 0;

            if (standby_counter >= 60)
            {
                print_msg2.text = "������ ���Ĺ��� ���� �����մϴ�.";
                Debug.Log("������ ���Ĺ��� ���� �����մϴ�.");
                is_standby = true;
                standby_counter = 0;
                other_pose_counter = 0;
                direction = "Ready";
            }
        }
        else 
        {
            standby_counter = 0;
            other_pose_counter ++;
            if (other_pose_counter >= 120)
            {
                print_msg2.text = "����ó ���Ĺ��̸�尡 �ƴմϴ�.";
                Debug.Log("����ó ���Ĺ��̸�尡 �ƴմϴ�.");
                is_standby = false;
                standby_counter = 0;
                other_pose_counter = 0;
                direction = "Default";
            }
        }




    }


    //standby�� ���� �� ����.
    void Gesture_recognization() 
    {
        if (is_standby)
        {
            //���� ����ó�϶�
            if (quat_to_eulerList[3].z > 60.0f && quat_to_eulerList[3].z < 90.0f)
            {
                Debug.Log("���� �������Դϴ�.");
                print_msg2.text = "���� �������Դϴ�.";
                direction = "Up";

                is_standby = false;
                standby_counter = 0;
                other_pose_counter = 0;
            }
            //�Ʒ��� ����ó�϶�
            else if (quat_to_eulerList[3].z > 270.0f && quat_to_eulerList[3].z < 300.0f)
            {
                Debug.Log("�Ʒ��� �������Դϴ�.");
                print_msg2.text = "�Ʒ��� �������Դϴ�.";
                direction = "Down";

                is_standby = false;
                standby_counter = 0;
                other_pose_counter = 0;

            }
            //if ("������ ����ó�϶�") { }
            else if (quat_to_eulerList[3].y < 85.0f  && quat_to_eulerList[2].y < 350.0f && quat_to_eulerList[2].y > 330.0f)
            {
                Debug.Log("������ �������Դϴ�.");
                print_msg2.text = "������ �������Դϴ�.";
                direction = "Right";

                is_standby = false;
                standby_counter = 0;
                other_pose_counter = 0;
            }

            //if ("���� ����ó�϶�") { }
            else if (quat_to_eulerList[3].y > 150.0f)
            {
                Debug.Log("���� �������Դϴ�.");
                print_msg2.text = "���� �������Դϴ�.";
                direction = "Left";

                is_standby = false;
                standby_counter = 0;
                other_pose_counter = 0;
            }
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////

    public static Vector3 RotationMatrixToEulerAngles(Matrix4x4 rotationMatrix)
    {
        // Extract the rotation part of the 4x4 matrix and convert it to a Quaternion
        Quaternion quaternion = Quaternion.LookRotation(
            rotationMatrix.GetColumn(2),
            rotationMatrix.GetColumn(1)
        );

        // Convert the Quaternion to Euler angles
        return quaternion.eulerAngles;
    }
    
    //���ʹϾ� ������ ȸ����ķ� ��ȯ �� ���Ϸ��� ��ȯ

    public static Matrix4x4 QuaternionToRotationMatrix(Quaternion quaternion)
    {
        float w = quaternion.w;
        float x = quaternion.x;
        float y = quaternion.y;
        float z = quaternion.z;

        Matrix4x4 rotationMatrix = new Matrix4x4();

        rotationMatrix.m00 = 1 - 2 * y * y - 2 * z * z;
        rotationMatrix.m01 = 2 * x * y - 2 * w * z;
        rotationMatrix.m02 = 2 * x * z + 2 * w * y;

        rotationMatrix.m10 = 2 * x * y + 2 * w * z;
        rotationMatrix.m11 = 1 - 2 * x * x - 2 * z * z;
        rotationMatrix.m12 = 2 * y * z - 2 * w * x;

        rotationMatrix.m20 = 2 * x * z - 2 * w * y;
        rotationMatrix.m21 = 2 * y * z + 2 * w * x;
        rotationMatrix.m22 = 1 - 2 * x * x - 2 * y * y;

        rotationMatrix.m03 = rotationMatrix.m13 = rotationMatrix.m23 = rotationMatrix.m30 = rotationMatrix.m31 = rotationMatrix.m32 = 0;
        rotationMatrix.m33 = 1;

        return rotationMatrix;
    }

    Vector3 Quat_to_RotationM_to_Euler(Quaternion input_quaternion) 
    {
        Matrix4x4 temp;
        Vector3 ret;
        temp = QuaternionToRotationMatrix(input_quaternion);
        ret = RotationMatrixToEulerAngles(temp);

        return ret;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////



    // Start is called before the first frame update
    void Start()
    {

        waitingTime = 0.01667f;
        timer = 0.0f;

        from_playscript_dot_cs = GameObject.Find("Xsens");

        print_msg2 = GameObject.Find("print_msg_2").GetComponent<Text>();
        print_msg3 = GameObject.Find("print_msg_3").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //0: pel, 1: chest, 2: R_Shouler, 3: R_Elbow, 4: L_Shouler, 5: L_Elbow 6~: empty in dot
        finalQuatList = from_playscript_dot_cs.GetComponent<playscript>().final_Input_List; //playscript���� ���� ���� + Ķ���극�̼ǵ� ���ʹϾ� ����

        for (int i = 0; i < 6; i++)// �޾ƿ°� �ϳ��� �Ҵ�
        {
            quat_to_eulerList[i] = Quat_to_RotationM_to_Euler(finalQuatList[i]);
        }

        is_playing_avatar = from_playscript_dot_cs.GetComponent<playscript>().is_play_avatar;

        //�������� ������ ������� ����ó �Է� �غ� üũ �մϴ�.


        if (is_playing_avatar)
        {
            timer += Time.deltaTime;

            if (timer > waitingTime)
            {
                if (direction == "Up" || direction == "Down" || direction == "Left" || direction == "Right")
                { 
                    direction = "Default"; // �ʱ�ȭ �ѹ�
                }

                if (direction == "Default") 
                {
                    Standby();
                }
                if (is_standby)
                { 
                    Gesture_recognization(); 
                } 

                timer = 0;
            }
        }


            //string euler_msg = "Pelvis    >> X: " + quat_to_eulerList[0].x.ToString("F2") + " Y: " + quat_to_eulerList[0].y.ToString("F2") + " Z: " + quat_to_eulerList[0].z.ToString("F2") + "\n"+
            //                   "Chest     >> X: " + quat_to_eulerList[1].x.ToString("F2") + " Y: " + quat_to_eulerList[1].y.ToString("F2") + " Z: " + quat_to_eulerList[1].z.ToString("F2") + "\n" +
            //                   "R_Shouler >> X: " + quat_to_eulerList[2].x.ToString("F2") + " Y: " + quat_to_eulerList[2].y.ToString("F2") + " Z: " + quat_to_eulerList[2].z.ToString("F2") + "\n" +
            //                   "R_Elbow   >> X: " + quat_to_eulerList[3].x.ToString("F2") + " Y: " + quat_to_eulerList[3].y.ToString("F2") + " Z: " + quat_to_eulerList[3].z.ToString("F2") + "\n" +
            //                   "L_Shouler >> X: " + quat_to_eulerList[4].x.ToString("F2") + " Y: " + quat_to_eulerList[4].y.ToString("F2") + " Z: " + quat_to_eulerList[4].z.ToString("F2") + "\n" +
            //                   "L_Elbow   >> X: " + quat_to_eulerList[5].x.ToString("F2") + " Y: " + quat_to_eulerList[5].y.ToString("F2") + " Z: " + quat_to_eulerList[5].z.ToString("F2") + "\n";

            string euler_msg =

                   "R_Shouler >> X: " + quat_to_eulerList[2].x.ToString("F2") + " Y: " + quat_to_eulerList[2].y.ToString("F2") + " Z: " + quat_to_eulerList[2].z.ToString("F2") + "\n" +
                   "R_Elbow   >> X: " + quat_to_eulerList[3].x.ToString("F2") + " Y: " + quat_to_eulerList[3].y.ToString("F2") + " Z: " + quat_to_eulerList[3].z.ToString("F2") + "\n\n" +
                   "L_Shouler >> X: " + quat_to_eulerList[4].x.ToString("F2") + " Y: " + quat_to_eulerList[4].y.ToString("F2") + " Z: " + quat_to_eulerList[4].z.ToString("F2") + "\n"+
                   "L_Elbow   >> X: " + quat_to_eulerList[5].x.ToString("F2") + " Y: " + quat_to_eulerList[5].y.ToString("F2") + " Z: " + quat_to_eulerList[5].z.ToString("F2") + "\n";
        print_msg3.text = euler_msg;
    }
    
}
