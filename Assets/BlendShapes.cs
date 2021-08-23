using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapes : MonoBehaviour
{
    public GameObject Body;

    public string[] getBlendShapeNames(GameObject Body)
    {
        SkinnedMeshRenderer head = Body.GetComponent<SkinnedMeshRenderer>();
        Mesh m = head.sharedMesh;

        string[] arr;
        arr = new string[m.blendShapeCount];

        for(int i =0; i< m.blendShapeCount; i++)
        {
            string s = m.GetBlendShapeName(i);
            print("BlendShape: " + i + " " + s);

            arr[i] = s;
        }


        return arr;
    }


    // Start is called before the first frame update
    void Start()
    {
        getBlendShapeNames(Body);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
