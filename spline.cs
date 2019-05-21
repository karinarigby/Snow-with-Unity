using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spline : MonoBehaviour {
    Matrix4x4 basisMatrix = new Matrix4x4();

    //spline control points
    Vector3 c0 = new Vector3(-20, 5, 0);
    Vector3 c1 = new Vector3(0, 0, 0);
    Vector3 c2 = new Vector3(boidSystem.flyingSpace, boidSystem.flyingSpace, boidSystem.flyingSpace);
    Vector3 c3 = new Vector3(20, 8.2f, 4.4f);


    List<float> table = new List<float>();
    float resolution = 500;
    float totalFrames = 700;
    float currentFrame = 0.0f;
    bool isInterpolationDone = false;

    List<Vector3> splinePoints = new List<Vector3>();
   

    public GameObject cameraPrefab;
    public GameObject cam;

    void Start () 
    {
        basisMatrix.SetRow(0, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
        basisMatrix.SetRow(1, new Vector4(-3.0f, 3.0f, 0.0f, 0.0f));
        basisMatrix.SetRow(2, new Vector4(3.0f, -6.0f, 3.0f, 0.0f));
        basisMatrix.SetRow(3, new Vector4(-1.0f, 3.0f, -3.0f, 1.0f));


        //push points to splinePoints list by deciding resolution and iterating 
        //thru resolution amount of times 
        float scale = 1.0f / resolution;
        for (int res = 0; res < resolution; ++res)
        {
            Vector3 point = evaluateSpline(scale * res);
            splinePoints.Add(point);
        }

        //for all points in spline, calculate distance between every pair and push to the table list
        generateArcLengthTable();

        cam = (GameObject)Instantiate(cameraPrefab, splinePoints[0], Quaternion.identity);

            // gameObject.AddComponent<camera>
        GetComponent<Camera>().transform.position = interpolateOnSpline();
		

	}
	
    void Update () 
    {
        currentFrame++;

        for (int i = 0; i < splinePoints.Count-1; i++){
            Debug.DrawLine(splinePoints[i], splinePoints[i + 1]);
        }

        //update camera position (the following three are different options)
        GetComponent<Camera>().transform.position = interpolateOnSpline();
 
        //median
        GetComponent<Camera>().transform.LookAt(boidSystem.calculateMedianPoint()); 

        //middle
      //  camera.transform.LookAt(boidSystem.getMiddle());

        //first boid
      //  camera.transform.LookAt(boidSystem.getFirst());

		
	}

   Vector3 evaluateSpline(float t) 
   {
        Vector4 xControls = new Vector4(c0.x, c1.x, c2.x, c3.x);
        Vector4 yControls = new Vector4(c0.y, c1.y, c2.y, c3.y);
        Vector4 zControls = new Vector4(c0.z, c1.z, c2.z, c3.z);

        Vector4 xCoeff = basisMatrix * xControls;
        Vector4 yCoeff = basisMatrix * yControls;
        Vector4 zCoeff = basisMatrix * zControls;

        float xcr, ycr, zcr;
        xcr = xCoeff[0] + t * xCoeff[1] + t * t * xCoeff[2] + t * t * t * xCoeff[3];
        ycr = yCoeff[0] + t * yCoeff[1] + t * t * yCoeff[2] + t * t * t * yCoeff[3];
        zcr = zCoeff[0] + t * zCoeff[1] + t * t * zCoeff[2] + t * t * t * zCoeff[3];


        return new Vector3(xcr, ycr, zcr);
    }

    Vector3 interpolateOnSpline() 
    {
        float totalDistance = table[table.Count - 1];
        float step = totalDistance / totalFrames;
        float currDistance = step * currentFrame;
        int index = tableLookUp(currDistance);

        float t = (1.0f / resolution) * (index % resolution);
        return evaluateSpline(t);
    }

    void generateArcLengthTable() 
    {
        if (table.Count!=0)
        {
            table.Clear();
        }

        float scale = 1.0f / resolution;
        table.Add(0.0f);

        for (int i = 1; i < resolution + 1; ++i)
        {
            Vector3 p0 = evaluateSpline((i - 1) * scale);
            Vector3 p1 = evaluateSpline(i * scale);
            Vector3 dist = p0 - p1;
            table.Add(table[i - 1] + dist.magnitude);
        }
    }

    Vector3 getPosition() 
    {
        return new Vector3 (0, 0, 0);
    }

    float chooseEpsilon() 
    {
        float epsilon = 0.0f;
        float diff;

        for (int i = 0; i<table.Count - 1; ++i)
        {
            diff = Mathf.Abs(table[i] - table[i + 1]);
            if (diff > epsilon)
            {
                epsilon = diff;
            }
        }

        return epsilon;
    }


    int tableLookUp(float distance) 
    {
        float epsilon = chooseEpsilon();
        for (int i = 0; i < table.Count; ++i)
        {
            if (Mathf.Abs(table[i] - distance) < epsilon)
            {
                return i;
            }
        }
        return -1;
    }
}
