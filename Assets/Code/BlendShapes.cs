using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapes : MonoBehaviour
{
    public GameObject Body;
    int blendShapeCount;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    float blendOne = 0f;
    float blendTwo = 0f;
    float blendSpeed = 1f;
    bool blendOneFinished = false;

    //public string[] getBlendShapeNames(GameObject Body)
    //{
    //    SkinnedMeshRenderer head = Body.GetComponent<SkinnedMeshRenderer>();
    //    Mesh m = head.sharedMesh;

    //    string[] arr;
    //    arr = new string[m.blendShapeCount];

    //    for(int i =0; i< m.blendShapeCount; i++)
    //    {
    //        string s = m.GetBlendShapeName(i);
    //        print("BlendShape: " + i + " " + s);

    //        arr[i] = s;
    //    }


    //    return arr;
    //}

    private void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        //getBlendShapeNames(Body);
        blendShapeCount = skinnedMesh.blendShapeCount;

    }

    // Update is called once per frame
    void Update()
    {
        if (blendShapeCount > 2)
        {
            if (blendOne < 100f)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(0, blendOne);
                blendOne += blendSpeed;
            }
            else
            {
                blendOneFinished = true;
            }

            if (blendOneFinished == true && blendTwo < 100f)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(1, blendTwo);
                blendTwo += blendSpeed;
            }
        }
    }
}
