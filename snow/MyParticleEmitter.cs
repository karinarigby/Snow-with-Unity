using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Method was derived from "Foundations of Physically Based
 * Modeling and Animation" by Donald House
*/
public class MyParticleEmitter : MonoBehaviour
{
    float radius = 30f;
    static int numFlakes = 8090;
    GameObject[] flakeList = new GameObject[numFlakes];
    int[] inactiveList = new int[numFlakes];
    int inactiveCount;

    public GameObject snowPrefab;
    int maxRandLength = 1000;
    float sky = 0.5f;
    Vortex vortex;
    spline spline;
    frenet frenet;
  

    float generationRate = 470f;
    float fractionRH;
   
    List<Vector2> randList = new List<Vector2>();
    int rj;
    List<float> heightRandList = new List<float>();
    int hj;

    int counter;

    List<float> gaussRList = new List<float>();
    private void Awake()
    {
        spline = gameObject.GetComponent<spline>();
        frenet = gameObject.GetComponent<frenet>();
        vortex = gameObject.GetComponent<Vortex>();
    }

    void Start()
    {
        for (int i = 0; i < maxRandLength; i++)
        {
            randList.Add((Random.insideUnitCircle));
            heightRandList.Add(Random.Range(sky - 0.1f, sky));
        }

        InitParticleList();
        Generate();
    }

    void Update()
    {
        TestAndDeactivate();
        Generate();
        ComputeAccelerations();
    }
    void InitParticleList()
    {

        Vector3 position;
       
        for (int i = 0; i < numFlakes; i++)
        {
            position = new Vector3(heightRandList[i % maxRandLength], randList[i % maxRandLength].y, randList[(i) % maxRandLength].x);
            flakeList[i] = Instantiate(snowPrefab, position, Quaternion.identity);
            MyParticle flake = flakeList[i].GetComponent<MyParticle>();

            flake.SetInitUnitPosition(position);
            flake.SetCount(Random.Range(0, 100));
           
            float scale = heightRandList[(i + 4) % maxRandLength] - sky;
        }
        Clear(flakeList);
    }

    void Clear(GameObject[] particles)
    {
        GameObject gameObject;
        for (int i = 0; i < particles.Length; i++)
        {
            gameObject = particles[i].GetComponent<GameObject>();
            particles[i].SetActive(false);
            inactiveList[inactiveCount] = i;
            inactiveCount++;
        }
    }

    void TestAndDeactivate()
    {
        //does it meet requirements to be killed? ie going below y-5
        MyParticle flake;
        for (int i = 0; i < flakeList.Length; i++)
        {
            flake = flakeList[i].GetComponent<MyParticle>();
            if (flakeList[i].activeSelf)
            {
                if ((flake.GetCount()>=spline.getListLength()-10)||flake.GetLifespan()>=4f)
                {
                    flakeList[i].SetActive(false);
                          inactiveList[inactiveCount] = i;
                    flake.ResetLifespan();
                    Vector3 initPosition = new Vector3(heightRandList[i % maxRandLength], randList[i % maxRandLength].y, randList[(i) % maxRandLength].x);

                    //this carries the transformation about the unit circle, about 0 0 0 axis. will change in swirl
                    flake.transform.position = initPosition;
                    flake.SetInitUnitPosition(initPosition);

                    flake.SetOldPosition(flake.transform.position - flake.GetRelativeOldPosition());
                    inactiveCount++;
                }
            }
        }
    }

    void ComputeAccelerations()
    {
        MyParticle flake;
        Vector3 combinedAcceleration = new Vector3(0f, -0.0f, 0f);
   
        for (int i = 0; i < flakeList.Length; i++)
        {
            flake = flakeList[i].GetComponent<MyParticle>();
            if (flakeList[i].activeSelf)
            {
                CurvedSwirl(flake);
                flake.IncCount();
            }
        }

    }

    void Swirl(MyParticle flake)
    {
        int count = flake.GetCount();
        if( count < spline.getListLength())
        {
            Vector3 positionAtI = spline.getPositionAtI(count);
            Vector3[] coordinateFrame = frenet.frenetAt(count);

            Vector3 T = coordinateFrame[0];
            Vector3 N = coordinateFrame[1];
            Vector3 B = coordinateFrame[2];

            float omega = GetOmega(flake, T);

            Debug.DrawLine(positionAtI, positionAtI+T);
            Debug.DrawLine(positionAtI, positionAtI + N);
            Debug.DrawLine(positionAtI, positionAtI + B);

            //we need the position it should be at, plus rotation
            flake.transform.position = positionAtI;
            flake.transform.rotation = Quaternion.LookRotation(T, N);
            Vector3 oldPosition = flake.transform.position;

            flake.transform.RotateAround(positionAtI, T, Time.time);

            flake.IncCount();
        }
    }

    float GetOmega(MyParticle flake, Vector3 axis)
    {
        float omega;
        Vector3 center = axis;

        // axis is the direction
        Vector3 xvi = vortex.BaseToParticle(flake.transform.position, center);
        float li = vortex.ProjectionLengthAtParticle(xvi);
        Vector3 ri = vortex.OrthoVFromAxisToParticle(xvi, li);
        float riMag = vortex.RadiusDistFromParticleToAxis(ri);

   
        if (vortex.InVortexCylinder(li, riMag))
        {
            float fi = vortex.Frequency(riMag);
            omega = vortex.GetOmega(fi);
        }
        else { omega = 0f; }
        return omega;

    }

    void CurvedSwirl(MyParticle flake)
    {
        int count = flake.GetCount();
        if (count < spline.getListLength())
        {

            //transform of flake = this point
            Vector3 positionAtI = spline.getPositionAtI(count);
          
            Vector3[] coordinateFrame = frenet.frenetAt(count);

            Vector3 T = coordinateFrame[0];
            Vector3 N = coordinateFrame[1];
            Vector3 B = coordinateFrame[2];

            float omega = GetCurvedOmega(flake, T, N, B, positionAtI);

            //we need the position it should be at, plus rotation
            flake.transform.position = positionAtI;
            flake.transform.rotation = Quaternion.LookRotation(T, N);

            Vector3 flakeOriginalPosition = flake.getInitUnitPosition();

            flake.transform.position = positionAtI + flakeOriginalPosition ;

            float factor = 1f;
            Vector3 oldPosition = flake.transform.position;

            flake.transform.RotateAround(positionAtI, T, omega*Time.time);

            flake.IncCount();
           
        }
    }
  
    float GetCurvedOmega(MyParticle flake, Vector3 T, Vector3 N, Vector3 B, Vector3 splinePosition)
    {
        float omega;
        float distance = (float)flake.GetCount() / (float)spline.getListLength();
        Vector3 center = splinePosition- distance*T;
        // axis is the direction
       
        Vector3 xvi = vortex.BaseToParticle(flake.transform.position, center);

        float li = vortex.ProjectionLengthAtParticle(xvi);

        Vector3 ri = vortex.OrthoVFromAxisToParticle(xvi, li);
       
        float riMag = vortex.RadiusDistFromParticleToAxis(ri);
       
        //if (vortex.InVortexCylinder(li, riMag))
        //{
            float fi = vortex.Frequency(riMag);
            omega = vortex.GetOmega(fi);
       // }else{
        //  omega = 0f;
       // }

        return omega;

    }

    void Generate()
    {
         fractionRH += generationRate * Time.deltaTime - Mathf.Floor(generationRate * Time.deltaTime);
        int n = Mathf.FloorToInt(generationRate * Time.deltaTime);
        if (fractionRH >= 1)
        {
            n++;
            fractionRH--;
        }

        //set n elements as active 
        //take value at index of inactive list, set it as active in particlelist,
        for (int i = 0; i < n; i++)
        {
            if (inactiveCount > 0)
            {
                flakeList[inactiveList[inactiveCount - 1]].SetActive(true);
                inactiveCount--;

            }

        }



    }


}
