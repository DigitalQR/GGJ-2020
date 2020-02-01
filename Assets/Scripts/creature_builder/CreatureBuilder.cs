using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CreatureBuilder : MonoBehaviour
{
    [SerializeField]
    private float m_distanceFromCamera = 15f;

    [SerializeField]
    private Transform m_headPrefabTemplate;
    [SerializeField]
    private Transform m_boneBaseTemplate;

    [SerializeField]
    private Transform m_headPrefab;
    [SerializeField]
    private Transform m_boneBase;

    private Vector3 m_buildOffset = new Vector3 { x = 100, y = 100, z = 100 };
    private Camera m_camera = null;
    private Transform m_headTemplate;
    private bool m_headPlacedInScene;

    private void Start()
    {
        m_camera = Camera.main;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            PlaceTemplateLigament();
        }
        else if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            PlaceTemplateHead();                       
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            BuildCreature(m_headTemplate);
        }
    }

    private void PlaceTemplateLigament()
    {
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {   
            PlaceTemplateLigament(hit);
        }
    }

    private void PlaceTemplateHead()
    {
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.TryGetComponent(out Head head))
            {
                Destroy(objectHit.gameObject);
                m_headPlacedInScene = false;
            }
        }
        else if (!m_headPlacedInScene)
        {
            Vector3 newHeadPosition = ray.direction * m_distanceFromCamera + m_camera.transform.position;
            m_headTemplate = Instantiate(m_headPrefabTemplate, newHeadPosition, Quaternion.identity);
            m_headPlacedInScene = true;
        }
    }

    private Transform PlaceTemplateLigament(RaycastHit hit)
    {
        Transform newLigament = Instantiate(m_boneBaseTemplate, hit.point, Quaternion.identity);        
        Transform joint = newLigament.GetChild(0);

        float distance = Vector3.Distance(joint.position, newLigament.position);
        
        newLigament.up = hit.normal;
        newLigament.position += newLigament.up * distance;
        newLigament.SetParent(hit.transform);

        return newLigament;
    }

    private Transform BuildHead(Transform headTemplate)
    {
        Vector3 builtMonsterHeadPos = headTemplate.position + m_buildOffset;
        Transform head = Instantiate(m_headPrefab, builtMonsterHeadPos, headTemplate.rotation);

        return head;
    }

    private Transform BuildLigament(Transform ligamentTemplate, Transform parent)
    {
        Vector3 builtMonsterHeadPos = ligamentTemplate.position + m_buildOffset;
        Transform ligament = Instantiate(m_boneBase, builtMonsterHeadPos, ligamentTemplate.rotation);
        ligament.SetParent(parent);

        return ligament;
    }

    private LigamentChain BuildRestOfLimb(Transform limbRoot, Transform prevBuiltLimb)
    {
        int childCount = limbRoot.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = limbRoot.GetChild(i);
            if (child.TryGetComponent(out Ligament ligament))
            {
                Transform newLigament = BuildLigament(child, prevBuiltLimb);
                return BuildRestOfLimb(child, newLigament);                
            }
        }

        return prevBuiltLimb.GetComponent<LigamentChain>();
    }

    private LimbController MakeLimbRoot(Transform ligament, Rigidbody parentRigidBody)
    {
        LimbController newLimbController = ligament.gameObject.AddComponent<LimbController>();
        bool newLigamentHasChain = ligament.TryGetComponent(out LigamentChain newLigamentChain);
        Assert.IsTrue(newLigamentHasChain);

        ConfigurableJoint configurableJoint = ligament.GetComponent<ConfigurableJoint>();
        configurableJoint.connectedBody = parentRigidBody;

        return newLimbController;
    }
        
    private Transform BuildCreature(Transform head)
    {
        Transform builtCreatureRoot = BuildHead(head);
        Rigidbody parentRigidNumber = builtCreatureRoot.GetComponent<Rigidbody>();

        int childCount = head.childCount;
        for(int i = 0; i < childCount; i++)
        {
            Transform child = head.GetChild(i);
            if(child.TryGetComponent(out Ligament ligament))
            {
                Transform newLigament = BuildLigament(child, builtCreatureRoot);
                LimbController newLimbController = MakeLimbRoot(newLigament, parentRigidNumber);
                LigamentChain endJoint = BuildRestOfLimb(child, newLigament);

                newLimbController.BuildLimb(endJoint);
            }
        }

        return builtCreatureRoot;
    }
}
