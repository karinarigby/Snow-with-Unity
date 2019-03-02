using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frenet : MonoBehaviour {



    //V = 
    // k is V x C x V / V ^4
    //T = V / |V|
    //N = K/|K|
    //B = T x N 
    Vector3 P; 
    Vector3 V;
    Vector3 Q;
    Vector3 k;
    Vector3 T;
    Vector3 N;
    Vector3 B;

    int counter;
 
    public List<Vector3[]> frameList = new List<Vector3[]>();



    spline spline;
    private void Awake()
    {
        spline = gameObject.GetComponent<spline>();
    }

    void Start () {



        while (counter < spline.getListLength())
        {
            P = spline.getPositionAtI(counter);
            V = spline.getVelocityAtI(counter);
            Q = spline.getAccelerationAtI(counter);
            k = (Vector3.Cross(Vector3.Cross(V, Q), V)) / Mathf.Pow(V.magnitude, 4);

            T = V / V.magnitude;
            N = k / k.magnitude;
            B = Vector3.Cross(T, N);

            Vector3[] currentFrenet = new Vector3[3];
            currentFrenet[0] = T;
            currentFrenet[1] = N;
            currentFrenet[2] = B;

            frameList.Add(currentFrenet);
      
            //to reset it so its always adding the next set of 3 vectors
           // currentFrenet.Clear();
            counter++;
        }
	}
	
    //used to fetch the inner list at a certain index
    public Vector3[] frenetAt(int counter)
    {
        Vector3[] returnList = frameList[0];
        return frameList[counter];
    }
}
