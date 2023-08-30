/*
 * Copyright (C) 2021 
 * Max-Planck-Gesellschaft zur Förderung der Wissenschaften e.V. (MPG),
 * acting on behalf of its Max Planck Institute for Intelligent Systems and
 * the Max Planck Institute for Biological Cybernetics. All rights reserved.
 *
 * Max-Planck-Gesellschaft zur Förderung der Wissenschaften e.V. (MPG) is
 * holder of all proprietary rights on this computer program. You can only use
 * this computer program if you have closed a license agreement with MPG or
 * you get the right to use the computer program from someone who is authorized
 * to grant you that right.
 * Any use of the computer program without a valid license is prohibited and
 * liable to prosecution.
 *
 * Contact: ps-license@tuebingen.mpg.de
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;

#endif

// Joint recalculation
using LightweightMatrixCSharp;

public class SMPLX : MonoBehaviour
{
    public const int NUM_BETAS = 10;
    public const int NUM_EXPRESSIONS = 10;
    public const int NUM_JOINTS = 55;

    public enum ModelType {Unknown, Female, Neutral, Male};
    public enum HandPose {Flat, Relaxed};
    public enum BodyPose {T, A};
    public enum TotalCap {Pose1, Pose2};

    public ModelType modelType = ModelType.Unknown;

    public float[] betas = new float[NUM_BETAS];
    public float[] expressions = new float[NUM_EXPRESSIONS];

    public bool usePoseCorrectives = true;
    public bool showJointPositions = false;

    private SkinnedMeshRenderer _smr = null;
    private Mesh _sharedMeshDefault = null;
    private bool _defaultShape = true;

    private int _numBetaShapes;
    private int _numExpressions;
    private int _numPoseCorrectives;

    private Mesh _bakedMesh = null;
    private Vector3[] _jointPositions = null;
    private Quaternion[] _jointRotations = null;

    string[] _bodyJointNames = new string[] { "pelvis","left_hip","right_hip","spine1","left_knee","right_knee","spine2","left_ankle","right_ankle","spine3", "left_foot","right_foot","neck","left_collar","right_collar","head","left_shoulder","right_shoulder","left_elbow", "right_elbow","left_wrist","right_wrist","jaw","left_eye_smplhf","right_eye_smplhf","left_index1","left_index2","left_index3","left_middle1","left_middle2","left_middle3","left_pinky1","left_pinky2","left_pinky3","left_ring1","left_ring2","left_ring3","left_thumb1","left_thumb2","left_thumb3","right_index1","right_index2","right_index3","right_middle1","right_middle2","right_middle3","right_pinky1","right_pinky2","right_pinky3","right_ring1","right_ring2","right_ring3","right_thumb1","right_thumb2","right_thumb3" };
    string[] _handLeftJointNames = new string[] { "left_index1","left_index2","left_index3","left_middle1","left_middle2","left_middle3","left_pinky1","left_pinky2","left_pinky3","left_ring1","left_ring2","left_ring3","left_thumb1","left_thumb2","left_thumb3" } ;
    string[] _handRightJointNames = new string[] { "right_index1","right_index2","right_index3","right_middle1","right_middle2","right_middle3","right_pinky1","right_pinky2","right_pinky3","right_ring1","right_ring2","right_ring3","right_thumb1","right_thumb2","right_thumb3" } ;
    float[] _handFlatLeft = new float[] { 0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f };
    float[] _handFlatRight = new float[] { 0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f };
    float[] _handRelaxedLeft = new float[] { 0.11167871206998825f,0.042892176657915115f,-0.41644182801246643f,0.10881132632493973f,-0.06598567962646484f,-0.7562199831008911f,-0.09639296680688858f,-0.09091565757989883f,-0.18845929205417633f,-0.1180950403213501f,0.050943851470947266f,-0.529584527015686f,-0.14369840919971466f,0.055241700261831284f,-0.7048571109771729f,-0.01918291673064232f,-0.09233684837818146f,-0.33791351318359375f,-0.4570329785346985f,-0.1962839514017105f,-0.6254575252532959f,-0.21465237438678741f,-0.06599828600883484f,-0.5068942308425903f,-0.3697243630886078f,-0.060344625264406204f,-0.07949022948741913f,-0.1418696939945221f,-0.08585263043642044f,-0.6355282664299011f,-0.3033415973186493f,-0.05788097530603409f,-0.6313892006874084f,-0.17612089216709137f,-0.13209307193756104f,-0.37335458397865295f,0.8509643077850342f,0.27692273259162903f,-0.09154807031154633f,-0.4998394250869751f,0.02655647136271f,0.05288087576627731f,0.5355591773986816f,0.04596104100346565f,-0.2773580253124237f };
    float[] _handRelaxedRight = new float[] { 0.11167871206998825f, -0.042892176657915115f, 0.41644182801246643f, 0.10881132632493973f, 0.06598567962646484f, 0.7562199831008911f, -0.09639296680688858f, 0.09091565757989883f, 0.18845929205417633f, -0.1180950403213501f, -0.050943851470947266f, 0.529584527015686f, -0.14369840919971466f, -0.055241700261831284f, 0.7048571109771729f, -0.01918291673064232f, 0.09233684837818146f, 0.33791351318359375f, -0.4570329785346985f, 0.1962839514017105f, 0.6254575252532959f, -0.21465237438678741f, 0.06599828600883484f, 0.5068942308425903f, -0.3697243630886078f, 0.060344625264406204f, 0.07949022948741913f, -0.1418696939945221f, 0.08585263043642044f, 0.6355282664299011f, -0.3033415973186493f, 0.05788097530603409f, 0.6313892006874084f, -0.17612089216709137f, 0.13209307193756104f, 0.37335458397865295f, 0.8509643077850342f, -0.27692273259162903f, 0.09154807031154633f, -0.4998394250869751f, -0.02655647136271f, -0.05288087576627731f, 0.5355591773986816f, -0.04596104100346565f, 0.2773580253124237f };
    
    float[] _hand_TotalCap = new float[]
    {
        0.1116787156904433f, 0.04289217484104639f, -0.416441836812446f, 0.10881132513711185f, -0.06598567887822672f,
       -0.7562200100001023f, -0.09639296514009964f, -0.09091565922041477f, -0.1884592906970161f, -0.11809503934386675f,
        0.05094385260705536f, -0.529584499976084f, -0.14369840869244882f, 0.05524170001527844f, -0.7048571406917286f,
       -0.019182916839865817f, -0.09233684821597642f, -0.3379135244947721f, -0.4570329833266365f, -0.196283945164882f,
       -0.6254575328442732f, -0.21465237881086022f, -0.06599828649166892f, -0.5068942070038754f, -0.36972435736649994f,
       -0.06034462636097786f, -0.07949022787634757f, -0.14186969453172138f, -0.08585263331718808f, -0.6355282566897771f,
       -0.3033415874850632f, -0.05788097522832921f, -0.6313892099233043f, -0.1761208850183806f, -0.13209307627166406f,
       -0.37335457646458126f, 0.8509642789706306f, 0.2769227420650918f, -0.09154806978240376f, -0.49983943762160615f,
        0.026556472160458842f, 0.05288087673273013f, 0.5355591477841196f, 0.04596104095518329f, -0.2773580212595622f,

        0.1116787156904433f, -0.04289217484104638f, 0.41644183681244606f, 0.10881132513711185f, 0.0659856788782267f,
        0.7562200100001023f, -0.09639296514009964f, 0.09091565922041477f, 0.18845929069701614f, -0.11809503934386674f,
       -0.05094385260705537f, 0.529584499976084f, -0.14369840869244885f, -0.05524170001527845f, 0.7048571406917286f,
       -0.01918291683986582f, 0.09233684821597642f, 0.3379135244947721f, -0.4570329833266365f, 0.19628394516488204f,
        0.6254575328442732f, -0.21465237881086027f, 0.06599828649166892f, 0.5068942070038754f, -0.36972435736649994f,
        0.06034462636097784f, 0.07949022787634759f, -0.14186969453172144f, 0.08585263331718808f, 0.6355282566897771f,
       -0.3033415874850632f, 0.05788097522832922f, 0.6313892099233043f, -0.17612088501838064f, 0.13209307627166406f,
        0.37335457646458126f, 0.8509642789706306f, -0.2769227420650918f, 0.09154806978240378f, -0.49983943762160615f,
       -0.026556472160458842f, -0.05288087673273012f, 0.5355591477841195f, -0.0459610409551833f, 0.2773580212595623f
    };

    float[] _bodyPose_TotalCap = new float[] //360 프레임 동작
    {
        0.2508107632484311f, 0.03216074242810782f, 0.10912917936014355f, -0.31800420820850267f, -0.0012811404627807756f,
        0.023556043542668534f, 0.13524424691654952f, -0.04924968551764046f, 0.045695597027839964f, 0.4659097664667112f,
       -0.32726184152712734f, 0.0008502115086684362f, 0.317273427987009f, -0.1273294300909277f, 0.02323712854297955f,
        0.024007342125742587f, -0.008887174850699729f, 0.017597460419262714f, -0.19442258953887961f, 0.16201394363015706f,
       -0.032453225373093385f, -0.11262586263052105f, -0.22718388008885637f, 0.046026537706329494f, 0.07292777066643467f,
       -0.024766267094683873f, 0.02229179113394267f,   0.0f                , 0.0f                 , 0.0f                ,
        0.0f                 , 0.0f                ,   0.0f                , 0.023866591171002746f, 0.03356046648076521f,
       -0.001046146685077119f, 0.0211055313418709f, -0.008567407351046823f, -0.2897909794391707f, 0.013427340277648069f,
        0.005479770950440317f, 0.31066192848488167f,-0.11341250548766953f, 0.01406268156609005f, 0.06362080830112839f,
        0.09710963417329717f, -0.2991691202488064f, -0.9181692335452595f, 0.13854412879598682f, 0.15677339440120422f,
        0.9692313904527288f, 0.3188469056929627f, -0.5035933006236596f, 0.017498089619854443f, 0.09957737849046583f,
        0.38766734819338555f,-0.12601337019364003f,-0.102503082811151f, -0.07793541600493292f, 0.15846093388161703f,
       -0.0609614106287764f, 0.007995267655383687f,-0.05143216441521378f
    };

    Dictionary<string, Transform> _transformFromName;

    // Joint recalculation
    public static Dictionary<string, Matrix[]> JointMatrices = null;

    public void Awake()
    {
        if (_transformFromName == null)
        {
            _transformFromName = new Dictionary<string, Transform>();
            Transform[] transforms = gameObject.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in transforms)
            {
                _transformFromName.Add(t.name, t);
            }
        }

        if (_jointPositions == null)
        {
            _jointPositions = new Vector3[NUM_JOINTS];
            for (int i=0; i< NUM_JOINTS; i++)
            {
                Transform joint = _transformFromName[_bodyJointNames[i]];
                _jointPositions[i] = joint.position;
            }
        }

        if (_jointRotations == null)
        {
            _jointRotations = new Quaternion[NUM_JOINTS];
        }

        if (SMPLX.JointMatrices == null)
            InitJointRegressor();

        if (_smr != null)
            return;

        _smr = GetComponentInChildren<SkinnedMeshRenderer>();

        // Get skinned mesh blend shape values
        _numBetaShapes = 0;
        _numExpressions = 0;
        _numPoseCorrectives = 0;

        int blendShapeCount = _smr.sharedMesh.blendShapeCount;
        for (int i=0; i<blendShapeCount; i++)
        {
            string name = _smr.sharedMesh.GetBlendShapeName(i);
            if (name.StartsWith("Shape"))
                _numBetaShapes++;
            else if (name.StartsWith("Exp"))
                _numExpressions++;
            else if (name.StartsWith("Pose"))
                _numPoseCorrectives++;
        }
    }

    private bool InitJointRegressor()
    {
        SMPLX.JointMatrices = new Dictionary<string, Matrix[]>();

        // Setup gender specific joint regressors
        string[] genders = new string[] {"female", "neutral", "male"};
        foreach (string gender in genders)
        {
            Debug.Log("[SMPL-X] Setup betas-to-joints regressor: " + gender);
            string name_betas = "betasToJoints_" + gender;
            string name_template = "templateJ_" + gender;

            Matrix[] betasToJoints = new Matrix[3];
            Matrix[] templateJ = new Matrix[3];
            for (int i=0; i<=2; i++)
            {
                betasToJoints[i] = new Matrix(NUM_JOINTS, NUM_BETAS);
                templateJ[i] = new Matrix(NUM_JOINTS, 1);
            }

            // Setup matrix values from JSON resource files
            string name = "smplx_betas_to_joints_" + gender;
            TextAsset ta = Resources.Load<TextAsset>(name);
            if (ta == null)
            {
                Debug.LogError("[SMPL-X] Cannot find betas-to-joint regressor: SMPLX/Resources/" + name);
                return false;
            }
            SimpleJSON.JSONNode node = SimpleJSON.JSON.Parse(ta.text);

            for (int i=0; i < NUM_JOINTS; i++)
            {
                // Init beta regressor matrix
                for (int j=0; j< NUM_BETAS; j++)
                {
                    (betasToJoints[0])[i, j] = node["betasJ_regr"][i][0][j].AsDouble;
                    (betasToJoints[1])[i, j] = node["betasJ_regr"][i][1][j].AsDouble;
                    (betasToJoints[2])[i, j] = node["betasJ_regr"][i][2][j].AsDouble;
                }

                // Init joint template matrix
                double x = node["template_J"][i][0].AsDouble;
                double y = node["template_J"][i][1].AsDouble;
                double z = node["template_J"][i][2].AsDouble;

                (templateJ[0])[i, 0] = x;
                (templateJ[1])[i, 0] = y;
                (templateJ[2])[i, 0] = z;
            }

            SMPLX.JointMatrices.Add(name_betas, betasToJoints);
            SMPLX.JointMatrices.Add(name_template, templateJ);
        }
        return true;
    }

    public bool HasBetaShapes()
    {
        return (_numBetaShapes > 0);
    }

    public bool HasExpressions()
    {
        return (_numExpressions > 0);
    }

    public bool HasPoseCorrectives()
    {
        return (_numPoseCorrectives > 0);
    }

    public Vector3[] GetJointPositions()
    {
        return _jointPositions;
    }

    public void SetBetaShapes()
    {
        if (! HasBetaShapes() )
        {
            Debug.LogError("[SMPL-X] ERROR: Cannot set beta shapes on model without beta shapes");
            return;
        }

        _defaultShape = true;
        for (int i=0; i<NUM_BETAS; i++)
        {
            _smr.SetBlendShapeWeight(i, betas[i] * 100); // blend shape weights are specified in percentage

            if (betas[i] != 0.0f)
                _defaultShape = false;
        }

        UpdateJointPositions();
    }

    public void SetExpressions()
    {
        if (! HasExpressions() )
        {
            Debug.LogError("[SMPL-X] ERROR: Cannot set expressions on model without expressions");
            return;
        }

        for (int i=0; i<NUM_EXPRESSIONS; i++)
            _smr.SetBlendShapeWeight(i + NUM_BETAS, expressions[i] * 100); // blend shape weights are specified in percentage
    }

    public void SnapToGroundPlane()
    {
        if (_bakedMesh == null)
            _bakedMesh = new Mesh();

        _smr.BakeMesh(_bakedMesh);
        Vector3[] vertices =_bakedMesh.vertices;
        float yMin = vertices[0].y;
        for (int i=1; i<vertices.Length; i++)
        {
            float y = vertices[i].y;

            if (y < yMin)
                yMin = y;
        }

        Vector3 localPosition = gameObject.transform.localPosition;
        if (Mathf.Abs(yMin) < 0.00001)
            yMin = 0.0f;

        localPosition.y = -yMin;
        gameObject.transform.localPosition = localPosition;

        // Update joint world positions
        UpdateJointPositions(false);

    }

    public void GetModelInfo(out int shapes, out int expressions, out int poseCorrectives)
    {
        shapes = _numBetaShapes;
        expressions = _numExpressions;
        poseCorrectives = _numPoseCorrectives;
    }

    // Return Unity Quaternion for given SMPL-X rodrigues notation
    public static Quaternion QuatFromRodrigues(float rodX, float rodY, float rodZ)
    {
        // Local joint coordinate systems
        //   SMPL-X: X-Right, Y-Up, Z-Back, Right-handed
        //   Unity:  X-Left,  Y-Up, Z-Back, Left-handed
        Vector3 axis = new Vector3(-rodX, rodY, rodZ);
        float angle_deg = - axis.magnitude * Mathf.Rad2Deg;
        Vector3.Normalize(axis);

        Quaternion quat = Quaternion.AngleAxis(angle_deg, axis);
        
        return quat;
    }

    public void SetLocalJointRotation(string name, Quaternion quatLocal)
    {
        Transform joint = _transformFromName[name];
        joint.localRotation = quatLocal;
    }

    //public void SetWorldJointRotation(string name, Quaternion quatWorld) //CustromJointRotation function1 - KHB
    //{
    //    Transform joint = _transformFromName[name];
    //    //joint.rotation = quatWorld;
    //    joint.rotation = Quaternion.Inverse(Quaternion.Euler(180.0f, 0.0f, 0.0f)) * quatWorld * new Quaternion(0.0f, 0.0f, 1.0f, 0.0f);
    //}
    public void SetWorldJointRotation(string name, Quaternion quatWorld) //CustromJointRotation function1 - KHB
    {
        Transform joint = _transformFromName[name];
        //joint.rotation = quatWorld;
        joint.rotation =  quatWorld * Quaternion.Inverse(Quaternion.Euler(0.0f, 0.0f, 0.0f));
    }

    public void SetWorld2LocalJointRotation(string name, Quaternion quatWorld) //CustromJointRotation function2 - KHB
    {
        Transform joint = _transformFromName[name];
        Transform jointParent = joint.parent;

        //joint.localRotation = Quaternion.Inverse(jointParent.rotation) * Quaternion.Inverse(Quaternion.Euler(180.0f, 0.0f, 0.0f)) * quatWorld * new Quaternion(0.0f, 0.0f, 1.0f, 0.0f);//original
        joint.localRotation = Quaternion.Inverse(jointParent.rotation) * Quaternion.Inverse(Quaternion.Euler(180.0f, 0.0f, 0.0f)) * quatWorld * new Quaternion(0.0f, 0.0f, 1.0f, 0.0f);//original

        //joint.localRotation = Quaternion.Inverse(jointParent.rotation) * Quaternion.Inverse(Quaternion.Euler(180.0f, 180.0f, 0.0f)) * quatWorld * new Quaternion(0.0f, 0.0f, 1.0f, 0.0f);

    }
    public void PrintLocalRotation(string name)
    {
        Transform joint = _transformFromName[name];
        Debug.LogFormat("{0}의 로컬 회전 값\nW: {1}, X: {2}, Y: {3}, Z: {4}", name, joint.localRotation.w, joint.localRotation.x, joint.localRotation.y, joint.localRotation.z);
    }

    public void SetHandPose(HandPose pose)
    {
        float[] left = null;
        float[] right = null;

        if (pose == HandPose.Flat)
        {
            left = _handFlatLeft;
            right = _handFlatRight;
        }
        else if (pose == HandPose.Relaxed)
        {
            left = _handRelaxedLeft;
            right = _handRelaxedRight;
        }

        if ((left != null) && (right != null))
        {
            for (int i=0; i<15; i++)
            {
                string name = _handLeftJointNames[i];
                float rodX = left[i*3 + 0];
                float rodY = left[i*3 + 1];
                float rodZ = left[i*3 + 2];
                Quaternion quat = QuatFromRodrigues(rodX, rodY, rodZ);
                SetLocalJointRotation(name, quat);

                name = _handRightJointNames[i];
                rodX = right[i*3 + 0];
                rodY = right[i*3 + 1];
                rodZ = right[i*3 + 2];
                quat = QuatFromRodrigues(rodX, rodY, rodZ);
                SetLocalJointRotation(name, quat);
            }
        }

        UpdateJointPositions(false);

    }

    public void ResetBodyPose()
    {
        foreach(string name in _bodyJointNames)
        {
            Transform joint = _transformFromName[name];
            joint.localRotation = Quaternion.identity;
        }

        UpdateJointPositions(false);
    }

    public void SetBodyPose(BodyPose pose)
    {
        if (pose == BodyPose.T)
        {
            ResetBodyPose();
        }
        else if (pose == BodyPose.A)
        {
            ResetBodyPose();
            SetLocalJointRotation("left_collar", Quaternion.Euler(0.0f, 0.0f, 10.0f));
            SetLocalJointRotation("left_shoulder", Quaternion.Euler(0.0f, 0.0f, 35.0f));
            SetLocalJointRotation("right_collar", Quaternion.Euler(0.0f, 0.0f, -10.0f));
            SetLocalJointRotation("right_shoulder", Quaternion.Euler(0.0f, 0.0f, -35.0f));
        }
        UpdatePoseCorrectives();
        UpdateJointPositions(false);
    }

    public void EnablePoseCorrectives(bool enabled)
    {
        usePoseCorrectives = enabled;
        if (usePoseCorrectives)
        {
            UpdatePoseCorrectives();
        }
        else
        {
            int blendShapeCount = _smr.sharedMesh.blendShapeCount;
            for (int i=0; i<blendShapeCount; i++)
            {
                string name = _smr.sharedMesh.GetBlendShapeName(i);
                if (name.StartsWith("Pose"))
                    _smr.SetBlendShapeWeight(i, 0.0f);
            }
        }
    }

    public void UpdatePoseCorrectives()
    {
        if (!usePoseCorrectives)
            return;

        if (! HasPoseCorrectives())
            return;

        // Body joint #0 has no pose correctives
        for (int i=1; i<_bodyJointNames.Length; i++)
        {
            string name = _bodyJointNames[i];
            Quaternion quat = _transformFromName[name].localRotation;

            // Local joint coordinate systems
            //   Unity:  X-Left,  Y-Up, Z-Back, Left-handed
            //   SMPL-X: X-Right, Y-Up, Z-Back, Right-handed
            Quaternion quatSMPLX = new Quaternion(-quat.x, quat.y, quat.z, -quat.w);
            Matrix4x4 m = Matrix4x4.Rotate(quatSMPLX);
            // Subtract identity matrix to get proper pose shape weights
            m[0,0] = m[0,0] - 1.0f;
            m[1,1] = m[1,1] - 1.0f;
            m[2,2] = m[2,2] - 1.0f;
            
            // Get corrective pose start index
            int poseStartIndex = NUM_BETAS + NUM_EXPRESSIONS + (i-1)*9;

            _smr.SetBlendShapeWeight(poseStartIndex + 0, 100.0f * m[0,0]);
            _smr.SetBlendShapeWeight(poseStartIndex + 1, 100.0f * m[0,1]);
            _smr.SetBlendShapeWeight(poseStartIndex + 2, 100.0f * m[0,2]);

            _smr.SetBlendShapeWeight(poseStartIndex + 3, 100.0f * m[1,0]);
            _smr.SetBlendShapeWeight(poseStartIndex + 4, 100.0f * m[1,1]);
            _smr.SetBlendShapeWeight(poseStartIndex + 5, 100.0f * m[1,2]);

            _smr.SetBlendShapeWeight(poseStartIndex + 6, 100.0f * m[2,0]);
            _smr.SetBlendShapeWeight(poseStartIndex + 7, 100.0f * m[2,1]);
            _smr.SetBlendShapeWeight(poseStartIndex + 8, 100.0f * m[2,2]);
        }
    }

    public bool UpdateJointPositions(bool recalculateJoints = true)
    {
        if (HasBetaShapes() && recalculateJoints)
        {
            if (_sharedMeshDefault == null)
            {
                // Do not clone mesh if we haven't modified the shape parameters yet
                if (_defaultShape)
                    return false;

                // Clone default shared mesh so that we can modify later the shared mesh bind pose without affecting other shared instances.
                // Note that this will drastically increase the Unity scene file size and make Unity Editor very slow on save when multiple bodies like this are used.
                _sharedMeshDefault = _smr.sharedMesh;
                _smr.sharedMesh = (Mesh)Instantiate( _smr.sharedMesh );
                Debug.LogWarning("[SMPL-X] Cloning shared mesh to allow for joint recalculation on beta shape change [" + gameObject.name + "]. Note that this will increase the current scene file size significantly if model contains pose correctives.");
            }

            // Save pose and repose to T-Pose
            for (int i=0; i<NUM_JOINTS; i++)
            {
                Transform joint = _transformFromName[_bodyJointNames[i]];
                _jointRotations[i] = joint.localRotation;
                joint.localRotation = Quaternion.identity;
            }

            // Create beta value matrix
            Matrix betaMatrix = new Matrix(NUM_BETAS, 1);
            for (int row = 0; row < NUM_BETAS; row++)
            {
                betaMatrix[row, 0] = betas[row];
            }

            // Apply joint regressor to beta matrix to calculate new joint positions
            string gender = "";
            if (modelType == SMPLX.ModelType.Female)
                gender = "female";
            else if (modelType == SMPLX.ModelType.Neutral)
                gender = "neutral";
            else if (modelType == SMPLX.ModelType.Male)
                gender = "male";
            else
            {
                Debug.LogError("[SMPL-X] ERROR: Joint regressor needs model type information (Female/Neutral/Male)");
                return false;
            }

            Matrix[] betasToJoints = SMPLX.JointMatrices["betasToJoints_" + gender];
            Matrix[] templateJ = SMPLX.JointMatrices["templateJ_" + gender];;

            Matrix newJointsX = betasToJoints[0] * betaMatrix + templateJ[0];
            Matrix newJointsY = betasToJoints[1] * betaMatrix + templateJ[1];
            Matrix newJointsZ = betasToJoints[2] * betaMatrix + templateJ[2];

            // Update joint position cache
            for (int index = 0; index < NUM_JOINTS; index++)
            {
                Transform joint = _transformFromName[_bodyJointNames[index]];

                // Convert regressor coordinate system (OpenGL) to Unity coordinate system by negating X value
                Vector3 position = new Vector3(-(float)newJointsX[index, 0], (float)newJointsY[index, 0], (float)newJointsZ[index, 0]);

                // Regressor joint positions from joint calculation are centered at origin in world space
                // Transform to game object space for correct world space position
                joint.position = gameObject.transform.TransformPoint(position);
            }

            // Set new bind pose
            Matrix4x4[] bindPoses = _smr.sharedMesh.bindposes;
            Transform[] bones = _smr.bones;
            for (int i=0; i<bones.Length; i++)
            {
                // The bind pose is bone's inverse transformation matrix.
                // Make this matrix relative to the avatar root so that we can move the root game object around freely.
                bindPoses[i] = bones[i].worldToLocalMatrix * gameObject.transform.localToWorldMatrix;
            }
            _smr.sharedMesh.bindposes = bindPoses;

            // Restore pose
            for (int i=0; i<NUM_JOINTS; i++)
            {
                Transform joint = _transformFromName[_bodyJointNames[i]];
                joint.localRotation = _jointRotations[i];

                // Update joint position cache
                _jointPositions[i] = joint.position;

            }
        }
        else
        {
            for (int i=0; i<NUM_JOINTS; i++)
            {
                // Update joint position cache
                Transform joint = _transformFromName[_bodyJointNames[i]];
                _jointPositions[i] = joint.position;
            }
        }

        return true;
    }


    /*
    {"pelvis","spine2","right_shoulder","right_elbow", "left_shoulder","left_elbow","right_hip","right_knee","left_hip","left_knee" }
    */
   
    // Update is called once per frame

    void Update()
    {
        //평소에는 안쓰이는 구간.
        if (!usePoseCorrectives)
            UpdatePoseCorrectives();

        
    }
     
}

////////////////////////////////////////////////////////////////////////////////
// Custom editor code
////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
[CustomEditor(typeof(SMPLX))]
public class SMPLX_Editor : Editor {

    private SMPLX _target;
    private SerializedProperty _modelTypeProperty;
    private bool _showShape = true;
    private bool _showExpression = true;
    private bool _autoSnapToGroundPlane = true;
    private string _modelInfoText;

    void Awake() 
    {
        _target = (SMPLX)target;
        _target.Awake(); // initialize member values in Editor mode

        int shapes, expressions, poseCorrectives;
        _target.GetModelInfo(out shapes, out expressions, out poseCorrectives);
        _modelInfoText = string.Format("Model: {0} beta shapes, {1} expressions, {2} pose correctives", shapes, expressions, poseCorrectives);
    }

    void OnEnable()
    {
        // Fetch the objects from the GameObject script to display in the inspector
        _modelTypeProperty = serializedObject.FindProperty("modelType");
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(_target, _target.name); // allow GUI undo in custom editor
        Color defaultColor=GUI.backgroundColor;

        using (new EditorGUILayout.VerticalScope("Box")) 
        {
            // Info
            EditorGUILayout.HelpBox(_modelInfoText, MessageType.None);

            // Shape
            if (_target.HasBetaShapes() || _target.HasPoseCorrectives() )
            {
                using (new EditorGUILayout.VerticalScope("Box")) 
                {
                    using (new EditorGUILayout.VerticalScope("Box")) 
                    {
                        GUI.backgroundColor = Color.yellow;
                        if (GUILayout.Button("Shape"))
                            _showShape = ! _showShape;
                        GUI.backgroundColor = defaultColor;

                        if (_target.HasPoseCorrectives())
                        {
                            float labelWidth = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = 200;
                            bool usePoseCorrectivesNew = EditorGUILayout.Toggle("Use Pose Correctives", _target.usePoseCorrectives);
                            if (usePoseCorrectivesNew != _target.usePoseCorrectives)
                            {
                                if (usePoseCorrectivesNew)
                                    _target.EnablePoseCorrectives(true);
                                else
                                    _target.EnablePoseCorrectives(false);
                            }
                            EditorGUIUtility.labelWidth = labelWidth;
                        }

                        if (_target.HasBetaShapes())
                        {
                            EditorGUILayout.PropertyField(_modelTypeProperty);
                        }

                    }
                    if (_showShape && _target.HasBetaShapes())
                    {
                        using (new EditorGUILayout.VerticalScope("Box")) 
                        {
                            for (int i=0; i<SMPLX.NUM_BETAS; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Beta " + i, GUILayout.Width(50));
                                _target.betas[i] = EditorGUILayout.Slider(_target.betas[i], -5, 5);
                                // no effect: GUILayout.FlexibleSpace();
                                EditorGUILayout.EndHorizontal();
                            }

                            float labelWidth = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = 200;
                            _autoSnapToGroundPlane = EditorGUILayout.Toggle("Snap Feet To Local Ground Plane", _autoSnapToGroundPlane);
                            EditorGUIUtility.labelWidth = labelWidth;

                        }
                        using (new EditorGUILayout.VerticalScope("Box")) 
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Set"))
                            {
                                _target.SetBetaShapes();

                                if (_autoSnapToGroundPlane)
                                    _target.SnapToGroundPlane();

                            }
                            if (GUILayout.Button("Random"))
                            {
                                for (int i=0; i<SMPLX.NUM_BETAS; i++)
                                {
                                    _target.betas[i] = Random.Range(-2.0f, 2.0f);
                                }
                                _target.SetBetaShapes();

                                if (_autoSnapToGroundPlane)
                                    _target.SnapToGroundPlane();
                            }
                            if (GUILayout.Button("Reset"))
                            {
                                for (int i=0; i<SMPLX.NUM_BETAS; i++)
                                {
                                    _target.betas[i] = 0.0f;
                                }
                                _target.SetBetaShapes();

                                if (_autoSnapToGroundPlane)
                                    _target.SnapToGroundPlane();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }

            // Expression
            if (_target.HasExpressions())
            {
                using (new EditorGUILayout.VerticalScope("Box")) 
                {
                    using (new EditorGUILayout.VerticalScope("Box")) 
                    {
                        GUI.backgroundColor = Color.yellow;
                        if (GUILayout.Button("Expression"))
                            _showExpression = ! _showExpression;
                        GUI.backgroundColor = defaultColor;
                    }

                    if (_showExpression)
                    {
                        using (new EditorGUILayout.VerticalScope("Box")) 
                        {
                            for (int i=0; i<SMPLX.NUM_BETAS; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Exp " + i, GUILayout.Width(50));
                                _target.expressions[i] = EditorGUILayout.Slider(_target.expressions[i], -2, 2);
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        using (new EditorGUILayout.VerticalScope("Box")) 
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Set"))
                            {
                                _target.SetExpressions();
                            }
                            if (GUILayout.Button("Random"))
                            {
                                for (int i=0; i<SMPLX.NUM_EXPRESSIONS; i++)
                                {
                                    _target.expressions[i] = Random.Range(-2.0f, 2.0f);
                                }
                                _target.SetExpressions();
                            }
                            if (GUILayout.Button("Reset"))
                            {
                                for (int i=0; i<SMPLX.NUM_EXPRESSIONS; i++)
                                {
                                    _target.expressions[i] = 0.0f;
                                }
                                _target.SetExpressions();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }

            // Pose
            using (new EditorGUILayout.VerticalScope("Box")) 
            {
                using (new EditorGUILayout.VerticalScope("Box")) 
                {
                    GUI.backgroundColor = Color.yellow;
                    GUILayout.Button("Pose");
                    GUI.backgroundColor = defaultColor;

                }

                using (new EditorGUILayout.VerticalScope("Box")) 
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Body Pose", GUILayout.Width(100));
                    if (GUILayout.Button("T-Pose"))
                    {
                        _target.SetBodyPose(SMPLX.BodyPose.T);
                    }
                    if (GUILayout.Button("A-Pose"))
                    {
                        _target.SetBodyPose(SMPLX.BodyPose.A);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Hand Pose", GUILayout.Width(100));
                    if (GUILayout.Button("    Flat    "))
                    {
                        _target.SetHandPose(SMPLX.HandPose.Flat);
                    }
                    if (GUILayout.Button("Relaxed"))
                    {
                        _target.SetHandPose(SMPLX.HandPose.Relaxed);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("TotalCap", GUILayout.Width(100));
                    if (GUILayout.Button("    Pose1   "))
                    {
                        _target.SetHandPose(SMPLX.HandPose.Flat); //임시
                        //_target.SetHandPose(SMPLX.TotalCap.Pose1); // 대충 SetBodyPose랑 비슷한 내용이 들어갈 예정
                    }
                    if (GUILayout.Button("    Pose2   "))
                    {
                        _target.SetHandPose(SMPLX.HandPose.Relaxed); //임시
                        //_target.SetHandPose(SMPLX.TotalCap.Pose2);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }          

            // Drawing
            using (new EditorGUILayout.VerticalScope("Box"))
            {
                using (new EditorGUILayout.VerticalScope("Box")) 
                {
                    GUI.backgroundColor = Color.yellow;
                    GUILayout.Button("Drawing");
                    GUI.backgroundColor = defaultColor;
                }

                using (new EditorGUILayout.VerticalScope("Box"))
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 200;
                    bool showJointPositions = EditorGUILayout.Toggle("Show Joint Positions", _target.showJointPositions);
                    if (showJointPositions != _target.showJointPositions)
                    {
                        if (showJointPositions)
                            _target.UpdateJointPositions(false);

                        _target.showJointPositions = showJointPositions;
                        SceneView.RepaintAll();
                    }
                    EditorGUIUtility.labelWidth = labelWidth;
                }
            }
        }

        // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        if (! _target.showJointPositions)
            return;

        Handles.color = Color.yellow;

        Vector3[] jointPositions = _target.GetJointPositions();
        foreach (Vector3 pos in jointPositions)
        {
            Handles.SphereHandleCap(0, pos, Quaternion.identity, 0.025f, EventType.Repaint);
        }
    }
}
#endif // UNITY_EDITOR
