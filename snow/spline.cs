using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spline : MonoBehaviour
{
    Matrix4x4 basisMatrix = new Matrix4x4();

    public float bezierScale = 20.4f;
    Vector3 c0; //= new Vector3(scale*, 5, 0);
    Vector3 c1; //= new Vector3(0, 0, 0);
    // Vector3 c2 = new Vector3(12, -5, -1.4f);

    Vector3 c2; //= new Vector3();
    Vector3 c3; //= new Vector3(20, 8.2f, 4.4f);
    List<float> table = new List<float>();
    float resolution = 200;
    float totalFrames = 2000;
    float currentFrame = 0.0f;
    //bool isInterpolationDone = false;
    List<Vector3> splinePoints = new List<Vector3>();
    List<Vector3> velocityList = new List<Vector3>();
    List<Vector3> accelerationList = new List<Vector3>();
    //float duration = 7.0f;

    Vector4 xControls;
    void Awake()
    {
        

        c0 = new Vector3(bezierScale * 0.8f, (bezierScale * .2f)+15f, bezierScale * 0f);
        c1 = new Vector3(bezierScale * 1.2f, (bezierScale * 1.7f)+15f, bezierScale * 0f);
        c2 = new Vector3(bezierScale * -1.5f, (bezierScale * -1.7f)+15f, bezierScale * 0f);
        c3 = new Vector3(bezierScale*-1f, (bezierScale * 0f)+15f, bezierScale * 0f);

        basisMatrix.SetRow(0, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
        basisMatrix.SetRow(1, new Vector4(-3.0f, 3.0f, 0.0f, 0.0f));
        basisMatrix.SetRow(2, new Vector4(3.0f, -6.0f, 3.0f, 0.0f));
        basisMatrix.SetRow(3, new Vector4(-1.0f, 3.0f, -3.0f, 1.0f));


        //push points to splinePoints list by deciding resolution and iterating 
        //thru resolution amount of times 
        float scale = 1.0f / resolution;
        for (int res = 0; res < resolution; res++)
        {
            Vector3 point = evaluateSpline(scale * res);
            splinePoints.Add(point);

            velocityList.Add(velocityAt(scale * res));
            accelerationList.Add(accelerationAt(scale * res));
        }

        //for all points in spline, find calculate dist between every pair 
        //and push to the table list
        generateArcLengthTable();
    }

    void Update()
    {
        currentFrame++;
        for (int i = 0; i < splinePoints.Count - 1; i++)
        {
           Debug.DrawLine(splinePoints[i], splinePoints[i + 1]);
        }

    }
    public Vector3 tangentAtI(int i){
        return splinePoints[i] - splinePoints[i + 1];
    }

    Vector3 velocityAt(float t){
        Vector4 xControls = new Vector4(c0.x, c1.x, c2.x, c3.x);
        Vector4 yControls = new Vector4(c0.y, c1.y, c2.y, c3.y);
        Vector4 zControls = new Vector4(c0.z, c1.z, c2.z, c3.z);

        Vector4 xCoeff = basisMatrix * xControls;
        Vector4 yCoeff = basisMatrix * yControls;
        Vector4 zCoeff = basisMatrix * zControls;

        float xcr, ycr, zcr;
        xcr = xCoeff[0] * 0 + xCoeff[1] + 2f * t * xCoeff[2] + 3f * t * t * xCoeff[3];
        ycr = yCoeff[0]*0 + yCoeff[1] + 2f * t * yCoeff[2] + 3f * t * t * yCoeff[3];
        zcr = zCoeff[0]*0 + zCoeff[1] + 2f * t * zCoeff[2] + 3f * t * t * zCoeff[3];

        return new Vector3(xcr, ycr, zcr);
    }

    Vector3 accelerationAt(float t){
        Vector4 xControls = new Vector4(c0.x, c1.x, c2.x, c3.x);
        Vector4 yControls = new Vector4(c0.y, c1.y, c2.y, c3.y);
        Vector4 zControls = new Vector4(c0.z, c1.z, c2.z, c3.z);

        Vector4 xCoeff = basisMatrix * xControls;
        Vector4 yCoeff = basisMatrix * yControls;
        Vector4 zCoeff = basisMatrix * zControls;

        float xcr, ycr, zcr;

        xcr = xCoeff[0] * 0 + xCoeff[1]*0 + 2f * xCoeff[2] + 6f * t * xCoeff[3];
        ycr = yCoeff[0] * 0 + yCoeff[1]*0 + 2f * yCoeff[2] + 6f * t * yCoeff[3];
        zcr = zCoeff[0] * 0 + zCoeff[1]*0 + 2f * zCoeff[2] + 6f * t * zCoeff[3];

        return new Vector3(xcr, ycr, zcr);

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

    public Vector3 interpolateOnSpline()
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
        if (table.Count != 0)
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

    public Vector3 getPositionAtI(int i)
    {
        return splinePoints[i];
    }

    public Vector3 getVelocityAtI(int i)
    {
        return velocityList[i];
    }

    public Vector3 getAccelerationAtI(int i)
    {
        return accelerationList[i];
    }

    public int getVelocityListLength()
    {
        return velocityList.Count;
    }

    public int getAccelerationListLength()
    {
        return accelerationList.Count;
    }

    public int getListLength(){
        return splinePoints.Count;
    }
    float chooseEpsilon()
    {
        float epsilon = 0.0f;
        float diff;

        for (int i = 0; i < table.Count - 1; ++i)
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
