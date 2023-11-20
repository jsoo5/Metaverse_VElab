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

public class kiosk : MonoBehaviour
{
    Dictionary<int, string> oneD_Category = new Dictionary<int, string>
    {{0, "Ŀ��"},{1, "��ī����"},{2, "��"},{3, "������"},{4, "����Ʈ"},{5, "��ٱ���"} };
    ////////////////////////////////////////////////////////////
    Dictionary<int, string> twoD_coffee = new  Dictionary<int, string>
    {{0, "�Ƹ޸�ī��"},{1, "ī���"},{2, "�ٴҶ��"},{3, "ī��Ḷ���ƶ�"},{4, "�ݵ���"},{5, "��ٱ���"}};
    Dictionary<int, string> twoD_decaf = new Dictionary<int, string>
    {{0, "��ī����_�Ƹ޸�ī��"},{1, "��ī����_ī���"},{2, "��ī����_�ٴҶ��"},{3, "��ī����_ī��Ḷ���ƶ�"},{4, "��ī����_�ݵ���"},{5, "��ٱ���"}};
    Dictionary<int, string> twoD_tea = new Dictionary<int, string>
    {{0, "��׷���Ƽ"},{1, "���̺���Ƽ"},{2, "�𽺹�Ƽ"},{3, "ĳ����"},{4, "����Ŀ��"},{5, "��ٱ���"}};
    Dictionary<int, string> twoD_smoothy = new Dictionary<int, string>
    {{0, "�÷��ο��Ʈ_������"},{1, "����_������"},{2, "������Ʈ_������"},{3, "��纣��_������"},{4, "�ٴҶ�_������"},{5, "��ٱ���"}};
    Dictionary<int, string> twoD_dessert = new Dictionary<int, string>
    {{0, "ġ������ũ"},{1, "Ƽ��̼�"},{2, "��ī��"},{3, "��Ű"},{4, "�������"},{5, "��ٱ���"}};
    /// /////////////////////////////////////////////////////////

    Dictionary<int, string> threeD_pay = new Dictionary<int, string>
    {{0, "�ſ�ī��"},{1, "���̺�����"},{2, "ļļ������"},{3, "�������"}};

    Dictionary<string, int> Total_Menu_price = new Dictionary<string, int>
    {
        {"�Ƹ޸�ī��", 3000},{"ī���", 3500},{"�ٴҶ��", 4000},{"ī��Ḷ���ƶ�",4500},{"�ݵ���",4000},
        {"��ī����_�Ƹ޸�ī��", 3300},{"��ī����_ī���", 3800},{"��ī����_�ٴҶ��", 4300},{"��ī����_ī��Ḷ���ƶ�",4800},{"��ī����_�ݵ���",4300},
        {"��׷���Ƽ", 2800},{"���̺���Ƽ", 2800},{"�𽺹�Ƽ", 2800},{"ĳ����",2800},{"����Ŀ��",2800},
        {"�÷��ο��Ʈ_������", 4500},{"����_������", 4500},{"������Ʈ_������", 2800},{"��纣��_������",2800},{"�ٴҶ�_������",2800},
        {"ġ������ũ", 4500},{"Ƽ��̼�", 5000},{"��ī��", 3000},{"��Ű",2500},{"�������",3000}
    };
    Dictionary<string, int> cart  = new Dictionary<string, int>();
    Dictionary<int, string> cart_modify = new Dictionary<int, string>();


    int MenuIndex = 0;
    int change_counter = 0;
    int total_sum_price = 0;
    int prior_depth = 0;

    public string direction_from_motion_gesture = "";
    string kiosk_direction = "";
    string selected_category = "";
    string selected_menu = "";
    string current_menu;

    bool is_step0, is_step1, is_step2, is_step3, is_step4, is_step5, is_cart_modify = false;
    public bool is_duplicate = false;
    public bool is_loop_once_finished = false;
    bool is_ready = false;
    bool is_played = false;

    Renderer kioskIMG;
    Material start;
    Material C1, C2, C3, C4, C5, C6, C7;
    public Text printMessage_1, printMessage_2;

    float waitingTime;
    float timer;
    float delaytimer; 

    void step1_2_SELECT_CATEGORY1(string direction, Dictionary<int, string> category_OR_menu, int step_num)
    {
        Material[] MenuMaterial = new Material[] { C1, C2, C3, C4, C5, C6 };
        kioskIMG.material = MenuMaterial[MenuIndex];

        if (!is_played) //���� ����� �ѹ��� ó���մϴ�. or �𷺼��� �ԷµǾ� ���ŵ� ��� �ѹ� �÷�����...�ϱ�?
        {
            //������ �޴��� �����ֱ�.
            current_menu = category_OR_menu[MenuIndex];
            Debug.LogFormat("���� ���õ� �޴��� [{0}]�Դϴ�.", current_menu);
            is_played = true;
        }
        if (is_played) 
        {
            if (direction == "Up")
            {
                MenuIndex = 0;
                Debug.Log("����. �ش� �޴��� �̵��մϴ�");
                Debug.LogFormat("�̶��� ���õ� �޴���?: {0}", current_menu);

                //current_menu��  cart�� ���·� up�� ������ ���
                if (current_menu == "��ٱ���")//1������ 2�������� ��ٱ��Ϸ� �ٷ� ���ϴ�. ���࿡ ���� �� ���� prior ������ �ٽ� ���ƿɴϴ�.
                {
                    is_step1 = false; is_step2 = false;
                    is_step4 = true;
                }

                if (step_num == 1 && current_menu != "��ٱ���") //1�������� 2������ �Ѿ ��
                {
                    is_step1 = false; is_step2 = true;
                    selected_category = current_menu;

                }
                //�޴��� �����ϰ�, ��ٱ��Ͽ� �߰��� �� ����3���� �Ѿ�ϴ�. 
                if (step_num == 2 && current_menu != "��ٱ���")
                {
                    selected_menu = current_menu;

                    //��ٱ��Ͽ� �߰�
                    if (cart.ContainsKey(current_menu))
                    {
                        cart[current_menu] += 1;
                    }
                    else
                    {
                        cart.Add(current_menu, 1);
                    }
                    is_step2 = false; is_step3 = true;
                }
                is_duplicate = true;
                direction_from_motion_gesture = "Defualt";
                is_played = false;
            }
            if (direction == "Down")
            {
                Debug.Log("���. ���� �޴��� �̵��մϴ�.");
                MenuIndex = 0;
                if (step_num == 1) { is_step1 = false; is_step0 = true; }
                if (step_num == 2) { is_step2 = false; is_step1 = true; }
                direction_from_motion_gesture = "Defualt";
                is_played = false;
                is_duplicate = true;

            }
            if (direction == "Left")
            {
                Debug.Log("�����޴�");
                MenuIndex++;
                if (MenuIndex > 5) MenuIndex = 0;
                direction_from_motion_gesture = "Defualt";
                is_played = false;
                is_duplicate = true;

            }
            if (direction == "Right")
            {
                Debug.Log("�����޴�");
                MenuIndex--;
                if (MenuIndex < 0) MenuIndex = 5;
                direction_from_motion_gesture = "Defualt";
                is_played = false;
                is_duplicate = true;

            }
        }
    }
    void in_cart()
    {
        if (!is_played) 
        {
            Debug.Log("��ٱ��� �б� ����.");
            printMessage_1.text = "���� �ܰ�� '��ٱ��� �ܰ�'�Դϴ�.";
            printMessage_2.text = "��: ������������ �Ѿ��\n�Ʒ�: ���� �޴��� ���ư���\n����: ��ٱ��� ����.";
            //1. ���� ��ٱ��Ͽ� �ִ� �޴����� �ҷ��ݴϴ�. 
            //���� ��ųʸ��� ����ٸ� ���ٰ� ����ϰ� �ʱ� �޴��� �Ѿ�ϴ�.
            if (cart.Keys.Count == 0)
            {
                Debug.Log("���� ��ٱ��ϰ� ����ֽ��ϴ�. �ֹ��� ���� �ʱ� �޴��� �̵��մϴ�.");
                is_step0 = false; is_step2 = false; is_step3 = false; is_step4 = false;
                is_step1 = true;

                change_counter = 0;
                MenuIndex = 0;
            }
            else
            {
                Debug.Log("���� ��ٱ��Ͽ� �ִ� �޴����� ������ �����ϴ�.");

                //�޴��� ������ 0���� Ű�� ������ �װ� �����մϴ�.
                List<string> remove_keys_list = new List<string>();
                foreach (var key in cart.Keys)
                {
                    if (cart[key] == 0) remove_keys_list.Add(key);
                }
                foreach (var item in remove_keys_list) { cart.Remove(item); }
                //�޴��� ������ 0���� Ű�� ������ �װ� �����մϴ�.


                if (cart.Keys.Count != 0) //Ȥ�� ��� �޴��� ���������� �ʾҴ��� Ȯ���� �մϴ�. ���� ��� �޴��� �������ߴٸ� �ؿ��� �ʱ�� �Ѿ�ϴ�.
                {
                    foreach (var key in cart.Keys)
                    {
                        Debug.LogFormat("{0} : {1}��", key, cart[key]);
                    }
                }
            }
            if (cart.Keys.Count == 0)
            {
                Debug.Log("���� ��ٱ��ϰ� ����ֽ��ϴ�. �ֹ��� ���� �ʱ� �޴��� �̵��մϴ�.");
                is_step0 = false; is_step2 = false; is_step3 = false; is_step4 = false;
                is_step1 = true;

                change_counter = 0;
                MenuIndex = 0;
            }

            //2. �� �� �� �޴��� ������ ���� �� ������ ����մϴ�.
            if (cart.Keys.Count != 0)
            {
                total_sum_price = 0; // �ٸ� �б⿡ ������ �� ��ġ�� �ʰ� �ʱ�ȭ...
                foreach (var key in cart.Keys)
                {
                    total_sum_price += Total_Menu_price[key] * cart[key];
                }
                Debug.LogFormat("���� �� �ݾ�: {0}", total_sum_price);
                Debug.Log("������ ���� ����ó�� �����ּ���. ��: ������ ����. �Ʒ�: �����޴�.");
                is_played = true;
            }
        }
        if (is_played) //��ٱ��� ����Ʈ �ѹ� �÷��� �ǰ�. �������� �ִ� ��.
        {
            //Debug.Log("��ٱ���: ����ó �б� ����.");

            if (direction_from_motion_gesture == "Up")
            {
                Debug.Log("��ٱ���_���� \nȮ��. ���� �ܰ�� �Ѿ�ϴ�.");

                is_played = false; is_step4 = false; is_step5 = true;
                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
            if (direction_from_motion_gesture == "Down")
            {
                Debug.Log("��ٱ���_�Ʒ��� \n.���� �޴��� ���ư��ϴ�.");
                is_played = false; is_step4 = false;
                if (prior_depth == 1) { is_step1 = true; }
                if (prior_depth == 2) { is_step2 = true; Debug.LogFormat("�̶��� selected_menu: {0}", selected_menu);
                }
                if (prior_depth == 3) { is_step2 = true; Debug.LogFormat("�̶��� selected_menu: {0}", selected_menu);
                }
                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
            if (direction_from_motion_gesture == "Left")//�޴� ����(���� ���� or ����(0�̸� ����))
            {
                Debug.Log("��ٱ���_����.�ʱ� �޴��� ���ư��ϴ�.");
                is_played = false; //is_step4 = false; is_step5 = true;
                is_cart_modify = true;
                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
            if (direction_from_motion_gesture == "Right")
            {
                Debug.Log("��ٱ���_������.�ʱ� �޴��� ���ư��ϴ�.");
                is_played = false; is_step4 = false; is_step5 = true;
                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
        }
    }
    void cart_menu_modifier()


    {
        if (!is_played)// �ѹ� ����Ǿ��� ��
        {
            printMessage_1.text = "���� �ܰ�� '��ٱ��� ���� �ܰ�'�Դϴ�. ���� �޴��� ���ư����� �޴����� �׸񿡼� �� �Ʒ��� �Է����ּ���.";
            printMessage_2.text = "��: ���� ����/���ư���\n�Ʒ�: ��������/���ư���\n����,������: �޴��̵�";
            cart_modify = new Dictionary<int, string>(); //���� ������ īƮ ������ �ʱ�ȭ �մϴ�. �ȱ׷��� �ߺ� ���� ��
            MenuIndex = 0;
            int i = 1;
            cart_modify.Add( 0, "���ø޴�");
            foreach (var key in cart.Keys) 
            {
                cart_modify.Add(i, key);
                i++;
            }
            i = 0;
            foreach (var key in cart_modify.Keys) 
            {
                Debug.LogFormat("������ ��ٱ��� ��Ȳ:\n{0}: {1}",key, cart_modify[key]);
            }
            is_played = true;
            // ��ٱ��Ͽ��� ������ ������ �޴��� ����ּ���.
        }
        if (is_played)
        {
            if (direction_from_motion_gesture == "Up") //���� ��
            {
                if (MenuIndex == 0)
                {
                    Debug.Log("��ٱ��� ����_����.�����������ϴ�.");
                    is_cart_modify = false;
                    is_played = false;

                }
                else 
                {
                    Debug.LogFormat("{0}�� �޴� ������ �ø��ϴ�.", cart_modify[MenuIndex]);

                    cart[cart_modify[MenuIndex]] += 1;
                    Debug.LogFormat("{0}�� ����: {1}", cart_modify[MenuIndex], cart[cart_modify[MenuIndex]]);

                }
                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
            if (direction_from_motion_gesture == "Down") //���� �ٿ�
            {
                if (MenuIndex == 0)
                {
                    Debug.Log("��ٱ��� ����_�Ʒ���.�����������ϴ�.");
                    is_cart_modify = false;
                    is_played = false;
                }
                else 
                {
                    Debug.LogFormat("{0}�� �޴� ������ ���Դϴ�.", cart_modify[MenuIndex]);

                    cart[cart_modify[MenuIndex]] -= 1;
                    if (cart[cart_modify[MenuIndex]] < 0) cart[cart_modify[MenuIndex]] = 0;
                    Debug.LogFormat("{0}�� ����: {1}", cart_modify[MenuIndex], cart[cart_modify[MenuIndex]]);
                }


                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
            if (direction_from_motion_gesture == "Left") //�޴� �̵�-��
            {
                Debug.Log("��ٱ��� ����_����.�����������ϴ�.");
                MenuIndex++;
                if (MenuIndex > cart_modify.Keys.Count-1) MenuIndex = 0;

                if (MenuIndex == 0) Debug.LogFormat("�޴�: ��ٱ��Ϸ� ���ư����� ���� �Ʒ����� �Է����ּ���.");
                else 
                {
                    Debug.LogFormat("���� ���õ� ��ٱ��� �޴�:{0}\n���� ���õ� �޴��� ����:{1}", cart_modify[MenuIndex], cart[cart_modify[MenuIndex]]);
                }

                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
            if (direction_from_motion_gesture == "Right") //�޴� �̵�-��
            {
                Debug.Log("��ٱ��� ����_������.�����������ϴ�.");
                MenuIndex--;
                if (MenuIndex < 0) MenuIndex = cart_modify.Keys.Count - 1;

                if (MenuIndex == 0) Debug.LogFormat("�޴�: ��ٱ��Ϸ� ���ư����� ���� �Ʒ����� �Է����ּ���.");
                else 
                {
                    Debug.LogFormat("���� ���õ� ��ٱ��� �޴�:{0}\n���� ���õ� �޴��� ����:{1}", cart_modify[MenuIndex], cart[cart_modify[MenuIndex]]);
                }

                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
        }// is played �ʱ�ȭ �ѹ� ���Ѿ���
    }

    void PAYMENT() 
    {
        Material[] MenuMaterial = new Material[] { C1, C2, C3, C4};
        kioskIMG.material = MenuMaterial[MenuIndex];
        if (!is_played) // ó���� ����˴ϴ�.
        {        
            Debug.Log("�����������.");

            current_menu = threeD_pay[0];
            MenuIndex = 0;
            printMessage_1.text = "�� ������ �ݾ��� " + total_sum_price.ToString() + "���Դϴ�.";
            printMessage_2.text = "��: ������ ������������ ����\n�Ʒ�:��ٱ��Ϸ� ���ư���\n����,������:���� ���� ����";
            Debug.LogFormat("�� ������ �ݾ��� {0}���Դϴ�.", total_sum_price.ToString());
            Debug.Log("���������� �������ּ���. ��ٱ��Ϸ� ���÷��� �ڷΰ��⸦ �����ּ���.");
            Debug.LogFormat("���õ� ���������� '{0}' �Դϴ�.", current_menu);

            //Debug.LogFormat("���� �޴��� '{0}' �Դϴ�.", current_menu);

            is_played = true;
        }

        if (is_played) //���Ŀ� ����ϸ鼭 ���ư��� ��ũ��Ʈ�Դϴ�.
        {
            if (direction_from_motion_gesture == "Up")
            {
                Debug.Log("����_����.�ش� ���� ������� ������ ������ �ּ���.");
                cart = new Dictionary<string, int>(); //��ٱ��� ���� >> �ʱ�ȭ������ ���� ����...
                is_played = false; is_step5 = false;
                is_ready = false; is_step0 = true; //�ʱ�� ���ư��ϴ�.
                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
            if (direction_from_motion_gesture == "Down")
            {
                Debug.Log("����_�Ʒ���.��ٱ��Ϸ� ���ư��ϴ�.");
                is_played = false; is_step5 = false; is_step4 = true;
                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
            if (direction_from_motion_gesture == "Left")//�޴� ����(���� ���� or ����(0�̸� ����))
            {
                Debug.Log("����_����.");
                MenuIndex++;
                if (MenuIndex > 3) MenuIndex = 0;
                current_menu = threeD_pay[MenuIndex];
                Debug.LogFormat("���õ� ���������� '{0}' �Դϴ�.", current_menu);
                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }
            if (direction_from_motion_gesture == "Right")
            {
                Debug.Log("����_������.");
                MenuIndex--;
                if (MenuIndex < 0) MenuIndex = 3;
                current_menu = threeD_pay[MenuIndex];
                Debug.LogFormat("���õ� ���������� '{0}' �Դϴ�.", current_menu);

                direction_from_motion_gesture = "Defualt";
                is_duplicate = true;

            }

        }


    }
    void COMMAND_WITH_ARROWS()
    {
        //direction_from_motion_gesture = "Defualt"; //�������� ��Ʈ���Ҷ��¿��� ��Ȱ��ȭ ���Ѿ���
        //ȭ��ǥŰ�� �𷺼� �� �ֱ�.
        if (Input.GetKeyDown(KeyCode.R)) { is_ready = true; }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { direction_from_motion_gesture = "Up";  }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { direction_from_motion_gesture = "Down"; }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { direction_from_motion_gesture = "Left";}
        if (Input.GetKeyDown(KeyCode.RightArrow)) { direction_from_motion_gesture = "Right";  }
    }
    // Start is called before the first frame update
    void Start()
    {
        printMessage_1 = GameObject.Find("print_msg_3").GetComponent<Text>();
        printMessage_2 = GameObject.Find("print_msg_2").GetComponent<Text>();

        C1 = Resources.Load<Material>("1_Coffee");
        C2 = Resources.Load<Material>("2_Decaf");
        C3 = Resources.Load<Material>("3_Tea");
        C4 = Resources.Load<Material>("4_Smoothy");
        C5 = Resources.Load<Material>("5_Dessert");
        C6 = Resources.Load<Material>("Materials/KioskMenuMaterial/Cart");
        C7 = Resources.Load<Material>("Materials/KioskMenuMaterial/Cart_modify");
        start = Resources.Load<Material>("Materials/KioskMenuMaterial/Ready");
        //Debug.Log("�������. ���ϴ� ī�װ��� �������ּ���");
        is_step0 = true;

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        //COMMAND_WITH_ARROWS(); //Ű���� �Է� ���
        //printMessage_2.text = "���� direction �Է°�:" + direction_from_motion_gesture;


        kioskIMG = GameObject.Find("Screen").GetComponent<MeshRenderer>();
        //ȭ��ǥ�� ��Ʈ���ϴ� ����϶� ���� ���� �ϱ�. 
        direction_from_motion_gesture = GameObject.Find("Xsens").GetComponent<motion_gesture>().direction;
        is_ready = GameObject.Find("Xsens").GetComponent<motion_gesture>().is_ready_to_order;


        if (is_step0)
        {
            if (!is_played) // ó���� ����˴ϴ�.
            {
                prior_depth = 0;
                MenuIndex = 0;
                printMessage_1.text = "Ű����ũ�� �����Դϴ�. �������!. \n�����Ͻ÷��� �¿�� ���� �����ּ���.";
                Debug.Log("Ű����ũ�� �����Դϴ�. �������!. �����Ͻ÷��� �¿�� ���� �����ּ���.");
                kioskIMG.material = start;
                is_played = true;

            }

            if (is_played) //���Ŀ� ����ϸ鼭 ���ư��� ��ũ��Ʈ�Դϴ�.
            {
                
                if (is_ready)
                {
                    is_played = false;

                    is_step0 = false;
                    is_step1 = true;
                    
                }

            }

        }
        if (is_step1)
        {
            printMessage_1.text = "�ֹ��Ͻ� �޴��� ī�װ��� ����ּ���! ���� ī�װ��� []�Դϴ�.";
            printMessage_2.text = "��: ī�װ� ����, �Ʒ�:���� �޴�\n����, ������: �޴� �̵�";
            prior_depth = 1;
            C1 = Resources.Load<Material>("Materials/KioskMenuMaterial/1_Category/1_Coffee");
            C2 = Resources.Load<Material>("Materials/KioskMenuMaterial/1_Category/2_Decaf");
            C3 = Resources.Load<Material>("Materials/KioskMenuMaterial/1_Category/3_Tea");
            C4 = Resources.Load<Material>("Materials/KioskMenuMaterial/1_Category/4_Smoothy");
            C5 = Resources.Load<Material>("Materials/KioskMenuMaterial/1_Category/5_Dessert");
            step1_2_SELECT_CATEGORY1(direction_from_motion_gesture, oneD_Category, 1);
        }
        if (is_step2)
        {
            prior_depth = 2;
            printMessage_1.text = "�ֹ��Ͻ� �޴��� ����ּ���!";
            printMessage_2.text = "��: ī�װ� ����, �Ʒ�:���� �޴�\n����, ������: �޴� �̵�";

            if (selected_category == "Ŀ��")
            {
                C1 = Resources.Load<Material>("Materials/KioskMenuMaterial/2_Coffee/Coffee_1"); C2 = Resources.Load<Material>("Materials/KioskMenuMaterial/2_Coffee/Coffee_2");
                C3 = Resources.Load<Material>("Materials/KioskMenuMaterial/2_Coffee/Coffee_3"); C4 = Resources.Load<Material>("Materials/KioskMenuMaterial/2_Coffee/Coffee_4");
                C5 = Resources.Load<Material>("Materials/KioskMenuMaterial/2_Coffee/Coffee_5"); C6 = Resources.Load<Material>("Materials/KioskMenuMaterial/Cart");
                step1_2_SELECT_CATEGORY1(direction_from_motion_gesture, twoD_coffee, 2);
            }
            if (selected_category == "��ī����")
            {
                C1 = Resources.Load<Material>("Materials/KioskMenuMaterial/3_Decaf/Decaf_1"); C2 = Resources.Load<Material>("Materials/KioskMenuMaterial/3_Decaf/Decaf_2");
                C3 = Resources.Load<Material>("Materials/KioskMenuMaterial/3_Decaf/Decaf_3"); C4 = Resources.Load<Material>("Materials/KioskMenuMaterial/3_Decaf/Decaf_4");
                C5 = Resources.Load<Material>("Materials/KioskMenuMaterial/3_Decaf/Decaf_5"); C6 = Resources.Load<Material>("Materials/KioskMenuMaterial/Cart");
                step1_2_SELECT_CATEGORY1(direction_from_motion_gesture, twoD_decaf, 2);
            }

            if (selected_category == "��")
            {
                C1 = Resources.Load<Material>("Materials/KioskMenuMaterial/4_Tea/Tea_1"); C2 = Resources.Load<Material>("Materials/KioskMenuMaterial/4_Tea/Tea_2");
                C3 = Resources.Load<Material>("Materials/KioskMenuMaterial/4_Tea/Tea_3"); C4 = Resources.Load<Material>("Materials/KioskMenuMaterial/4_Tea/Tea_4");
                C5 = Resources.Load<Material>("Materials/KioskMenuMaterial/4_Tea/Tea_5"); C6 = Resources.Load<Material>("Materials/KioskMenuMaterial/Cart");
                step1_2_SELECT_CATEGORY1(direction_from_motion_gesture, twoD_tea, 2);
            }
            if (selected_category == "������")
            {
                C1 = Resources.Load<Material>("Materials/KioskMenuMaterial/5_Smoothy/Smoothy_1"); C2 = Resources.Load<Material>("Materials/KioskMenuMaterial/5_Smoothy/Smoothy_2");
                C3 = Resources.Load<Material>("Materials/KioskMenuMaterial/5_Smoothy/Smoothy_3"); C4 = Resources.Load<Material>("Materials/KioskMenuMaterial/5_Smoothy/Smoothy_4");
                C5 = Resources.Load<Material>("Materials/KioskMenuMaterial/5_Smoothy/Smoothy_5"); C6 = Resources.Load<Material>("Materials/KioskMenuMaterial/Cart");
                step1_2_SELECT_CATEGORY1(direction_from_motion_gesture, twoD_smoothy, 2);
            }
            if (selected_category == "����Ʈ")
            {
                C1 = Resources.Load<Material>("Materials/KioskMenuMaterial/6_Dessert/Dessert_1"); C2 = Resources.Load<Material>("Materials/KioskMenuMaterial/6_Dessert/Dessert_2");
                C3 = Resources.Load<Material>("Materials/KioskMenuMaterial/6_Dessert/Dessert_3"); C4 = Resources.Load<Material>("Materials/KioskMenuMaterial/6_Dessert/Dessert_4");
                C5 = Resources.Load<Material>("Materials/KioskMenuMaterial/6_Dessert/Dessert_5"); C6 = Resources.Load<Material>("Materials/KioskMenuMaterial/Cart");
                step1_2_SELECT_CATEGORY1(direction_from_motion_gesture, twoD_dessert, 2);
            }
            //if (selected_menu == "��ٱ���") { is_step2 = false; is_step4 = true; }
        }
        if (is_step3)
        {
            prior_depth = 3;

            //1. �� OR ���̽�? >> �׷��� �������� ����...

            //2. �� ��?
            //3. �޴��� �߰� �ֹ��Ͻðڽ��ϱ�?(YES: ī�װ��� �̵�, NO: ��ٱ��Ϸ� �̵�)



            Debug.Log("�߰��޴��� ��Ű�ڽ��ϱ�? �ƴϸ� ��ٱ��Ϸ� ���ڽ��ϱ�? ������ ���� �ȵ����� ��ٱ��Ϸ� ���ô�.");//�̰� �����ؾ���. til ��õ��


 
            is_step3 = false;
            is_step4 = true;
            //is_played = false;
        }
        if (is_step4)
        {
            //��ٱ��� ����. ����� 1������ 2������ ����� �Ѿ�� �� �ֽ��ϴ�.
            if (!is_cart_modify) 
            {
                kioskIMG.material = C6;
                in_cart();
            }
            if (is_cart_modify) 
            {
                kioskIMG.material = C7;
                cart_menu_modifier();
            }
            
   


        }
        if (is_step5)
        {
            C1 = Resources.Load<Material>("Materials/KioskMenuMaterial/7_Pay/Pay_Creditcard"); C2 = Resources.Load<Material>("Materials/KioskMenuMaterial/7_Pay/Pay_Naverpay");
            C3 = Resources.Load<Material>("Materials/KioskMenuMaterial/7_Pay/Pay_Kakaopay"); C4 = Resources.Load<Material>("Materials/KioskMenuMaterial/7_Pay/Pay_Coupon");
            //���� ������ �������ּ���...
            PAYMENT();

        }
        // 1���� �Է� ������ ���� �ش�. �ѹ� ������ �ԷµǸ�, 1�� �� ���� ������ �Էµ� ������ �Է� ��Ȱ��ȭ
        //if (direction_from_motion_gesture == "Up" || direction_from_motion_gesture == "Down" || direction_from_motion_gesture == "Left" || direction_from_motion_gesture == "Right")
        //{
        //    delaytimer += Time.deltaTime;
        //    if (is_duplicate)
        //    {                
        //        kiosk_direction = direction_from_motion_gesture;// 1. �ƹ�ư ����ó ���ϸ� is_duplicate Ʈ��� ��. �׷��� 
        //    }
        //    if (delaytimer > 0.5)
        //    {
        //        is_duplicate = false;
        //    }

        //}
        if (is_duplicate) { is_loop_once_finished = true; }

    }
}
