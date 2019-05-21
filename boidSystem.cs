using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* This class for flocking was based on House and Keyser's implementation in Ch. 6.5 in "Foundations of Physically Based Modelling & Animation". 
 *  See https://en.wikipedia.org/wiki/Flocking_(behavior) for flocking behavior.
 * 
 * The algorithm for generating a boid system is as follows:
 * 
 * boidList = randomly generate a list of n boids 
 * spatialGrid = create uniform spatial grid (a 3D array, that encompasses the environment that the boids can fly in). Each voxel is of type List.
 * 
 * for each boid in boidList: 
 *    hash boid's position to appropiate voxel in spatialGrid. Append boid to list in voxel.
 *      
 * for each voxel:
 *      iterate through each boid in the list. for each boid in the list:
 *         create an interaction list between the current boid and the other boids in the rest of the list and boids in surrounding voxels.
 *         only consider those that are in neighboring cells. 
 *        
 *         eliminate boids that are not within distance r2.
 *         eliminate boids that are in boids blind region (when some magnitude > some theta/2)
 *         Of those boids within distance, prioritize those that are closer.
 * 
 *         boid acceleration = collision vector + velocity matching vector + centering vector)
 *         prioritize most important acceleration source 
 *     
*/

public class boidSystem : MonoBehaviour
{
    public GameObject boidPrefab;
    static int numBoids = 15;
    public static int flyingSpace = 80;

    Vector3 position;
    public static GameObject[] boidList = new GameObject[numBoids];
    List<GameObject> interactionList = new List<GameObject>();
    List<GameObject>[,,] spatialGrid;

    Vector3 collision = new Vector3(0, 0, 0);
    Vector3 velocityMatch;
    Vector3 centering;
    public float kCollisionScale = 0.5f;
    public float kVelocityScale = 0.7f;
    public float kCenteringScale = 0.1f;

    Vector3 distance;
    Vector3 combinedAcceleration = new Vector3(0, 0.01f, 0);

    float r1 = 0.5f;
    float r2 = 1.5f;
    float kd;
    float ktheta;
    float thetaOne;
    float thetaTwo;
    float thetaMagnitude;

    boids currBoid;
    public float accelerationCap = 0.5f;
    Vector3 temp;

    public GameObject cameraPrefab;
    public static GameObject camera;
    GameObject cube;

    public static Vector3 getMiddle(){
        float middle = flyingSpace / 2;
        return new Vector3(middle, middle, middle);
    } 

    public static Vector3 getFirst(){
        return boidList[0].transform.position;
    }

    //Update called once per update
    void UpdateBoidPositions()
    {
        for (int i = 0; i < flyingSpace; i++)
        {
            for (int j = 0; j < flyingSpace; j++)
            {
                for (int k = 0; k < flyingSpace; k++)
                {

                    if (spatialGrid[i, j, k] != null)
                    {
                        //for each boid in list at voxel
                        int count = spatialGrid[i, j, k].Count;
                        for (int boidInVoxelList = 0; boidInVoxelList < count; boidInVoxelList++)
                        {
                            currBoid = spatialGrid[i, j, k][boidInVoxelList].GetComponent<boids>();

                            //create interaction list
                            //everything in this voxel can be added
                            // interactionList.AddRange(spatialGrid[i, j, k]); //plus everything that is one voxel away on all sides

                            //this will add 27 boxes if 3x3x3 or greater. otherwise will add all 8. 
                            for (int n = i - 1; n <= i + 1; n++)
                            {
                                //if its less than 0 we dont want that
                                if (n < 0) continue;
                                if (n >= flyingSpace) continue;  
                                for (int p = j - 1; p <= j + 1; p++)
                                {
                                    if (p < 0) continue;
                                    if (p >= flyingSpace) continue;
                                    for (int q = k - 1; q <= k + 1; q++)
                                    {
                                        if (q < 0) continue;
                                        if (q >= flyingSpace) continue;
                                        // Debug.Log("Would be adding boids from voxel [" + n + "," + p + "," + q + "]. /n/n");


                                        if (spatialGrid[n, p, q] != null)
                                        {
                                            interactionList.AddRange(spatialGrid[n, p, q]);
                                        }
                                    }
                                }
                            }

                            Vector3 ourBoidPosition = spatialGrid[i, j, k][boidInVoxelList].transform.position;
                           
        //now we have interaction list with our ijk boid. iterate thru each in list and add up accelerations           
                            int interactionCount = interactionList.Count;
                            for (int r = 0; r < interactionCount; r++)
                            {
                                if (interactionList[r].GetInstanceID() != currBoid.GetInstanceID())
                                {
                                    boids otherBoid = interactionList[r].GetComponent<boids>();

                                    //compute distance between our boid and the boid from the interaction list
                                    distance = interactionList[r].transform.position - ourBoidPosition;

                                    //compute ktheta
                                    thetaOne = currBoid.GetThetaOne();
                                    thetaTwo = currBoid.GetThetaTwo();
                                    thetaMagnitude = Vector3.Angle(currBoid.getVelocity(), otherBoid.transform.position);

                                    if ((-thetaOne / 2 <= thetaMagnitude) && thetaMagnitude <= thetaOne / 2)
                                    {
                                        ktheta = 1.0f;
                                    }
                                    else if (((thetaOne / 2) <= thetaMagnitude) && (thetaMagnitude <= (thetaTwo / 2)))
                                    {
                                        ktheta = ((thetaTwo / 2) - thetaMagnitude) / ((thetaTwo / 2) - (thetaOne / 2));
                                    }
                                    else
                                    {
                                        ktheta = 0.0f;
                                    }

                                    //compute kd
                                    if (distance.magnitude < r1)
                                    {
                                        kd = 1.0f;
                                    }
                                    else if ((r1 <= distance.magnitude) && (distance.magnitude <= r2))
                                    {
                                        kd = (r2 - distance.magnitude) / (r2 - r1);
                                    }
                                    else
                                    {
                                        kd = 0.0f;
                                    }

                                    //pg 107 from House and Keyser's text
                                    collision += ktheta * kd * (-1.0f * (kCollisionScale / (distance.magnitude + 0.01f)) * distance.normalized);
                                    velocityMatch += ktheta * kd * (kVelocityScale * (otherBoid.getVelocity() - currBoid.getVelocity()));
                                    centering += ktheta * kd * kCenteringScale * distance;
                                }
                            }

                            //prioritizing acceleration source (pg 108)
                            float ar = accelerationCap;
                            combinedAcceleration = Mathf.Min(ar, collision.magnitude) * collision.normalized;
                            ar = accelerationCap - combinedAcceleration.magnitude;
                            combinedAcceleration = combinedAcceleration + Mathf.Min(ar, velocityMatch.magnitude) * velocityMatch.normalized;
                            ar = accelerationCap - combinedAcceleration.magnitude;
                            combinedAcceleration = combinedAcceleration + Mathf.Min(ar, centering.magnitude) * centering.normalized;

                            //check walls. r2 is the far distance used in calculating kd
                           
                           //check if our boid is nearing where X = 0
                           if (ourBoidPosition.x < (3*flyingSpace/7))
                            {
                                combinedAcceleration.x += accelerationCap/2;
                            }

                           //check if our boid is nearing where X = max X bound
                            else if ((flyingSpace - ourBoidPosition.x) < 3*flyingSpace / 7)
                            {
                                combinedAcceleration.x += -accelerationCap/2;
                            }

                            //so if acceleration = 0 then reversing doesnt work
                            //check if our boid is nearing bound where Y = 0
                            if ((ourBoidPosition.y) < 3*flyingSpace / 7)
                            {
                                combinedAcceleration.y += accelerationCap/2;
                            }

                            //check if our boid is nearing where Y = max Y bound
                            else if ((flyingSpace - ourBoidPosition.y) < 3*flyingSpace / 7)
                            {
                                combinedAcceleration.y -= accelerationCap/2;
                            }

                            //check if our boid is nearing where Z = 0
                            if ((ourBoidPosition.z) < 3*flyingSpace / 7)
                            {
                                combinedAcceleration.z +=accelerationCap/2;
                            }

                            //check if our boid is nearing where Z = max Z bound
                            else if ((flyingSpace - ourBoidPosition.z) < 3*flyingSpace / 7)
                            {   
                                combinedAcceleration.z += -accelerationCap/2;
                            }

                            currBoid.setVelocity(combinedAcceleration * Time.deltaTime + currBoid.getVelocity());
                           
                            //update position
                            temp = ourBoidPosition;
                            spatialGrid[i, j, k][boidInVoxelList].transform.position =
                                 (2.0f * ourBoidPosition) - currBoid.getOldPosition() + ((Time.deltaTime * Time.deltaTime) * (combinedAcceleration));
                            currBoid.setOldPosition(temp);
                            interactionList.Clear();
                        }
                    }
                }
                 
            }
        }
        System.Array.Clear(spatialGrid, 0, spatialGrid.GetLength(0) * spatialGrid.GetLength(1) * spatialGrid.GetLength(2));

    }
    public static Vector3 calculateMedianPoint()
    {
        float x=0.0f;
        float y=0.0f;
        float z=0.0f;
        Vector3 median = new Vector3(0, 0, 0);
        int count = boidList.Length;

        for (int i = 0; i < count; i++)
        {
            x += boidList[i].transform.position.x;
            y += boidList[i].transform.position.y;
            z += boidList[i].transform.position.z;

        }
        x = x / count;
        y = y / count;
        z = z / count;
        median.Set(x, y, z);
        return median;
    }

    void InitSpatialGrid()
    {

        spatialGrid = new List<GameObject>[flyingSpace, flyingSpace, flyingSpace];
        for (int i = 0; i < numBoids; i++)
        {
            //initialize spatial grid again. 
            //if its null at hash make a list at the voxel.
            currBoid = boidList[i].GetComponent<boids>();
            position = currBoid.transform.position;

            //if boid position outside of limits, kill and reinitialize
            if (position.x <0.0f || position.x>flyingSpace 
                || position.y <0.0f || position.y>flyingSpace
                || position.z <0.0f || position.z >flyingSpace)
            {
                position = new Vector3
                      (Random.Range(0.0f + flyingSpace / 8, flyingSpace - flyingSpace / 8),
                      Random.Range(0.0f + flyingSpace / 8, flyingSpace - flyingSpace / 8),
                      Random.Range(0.0f + flyingSpace / 8, flyingSpace - flyingSpace / 8));
                  //  calculateMedianPoint();
                Destroy(boidList[i]);
                boidList[i] = Instantiate(boidPrefab, position, Quaternion.identity); 

            }

            if (spatialGrid[(int)Mathf.Floor(position.x), (int)Mathf.Floor(position.y), (int)Mathf.Floor(position.z)] == null)
            {

                spatialGrid[(int)Mathf.Floor(position.x),
                            (int)Mathf.Floor(position.y),
                            (int)Mathf.Floor(position.z)] = new List<GameObject>();
            }

            spatialGrid[(int)Mathf.Floor(position.x),
                        (int)Mathf.Floor(position.y),
                        (int)Mathf.Floor(position.z)].Add(boidList[i]);
        }
    }

    void Start()
    {

        //if in outside voxels (i.e., approaching the bounds of flyingSpace) and their direction is approaching the end, then add importance for collision avoidance
        //so if boid is approaching global world limit of 30 in any axis
        //so if distance between flyingspace.x and curr boid.x is within r1 r2, add importance

        spatialGrid = new List<GameObject>[flyingSpace, flyingSpace, flyingSpace];

        for (int i = 0; i < numBoids; i++)
        {
            position = new Vector3(Random.Range(0.0f + (3*flyingSpace / 7), flyingSpace - (3*flyingSpace / 7)),
                                   Random.Range(0.0f + (3*flyingSpace / 7), flyingSpace - (3*flyingSpace / 7)),
                                   Random.Range(0.0f + (3*flyingSpace / 7), flyingSpace - (3*flyingSpace / 7)));

            boidList[i] = Instantiate(boidPrefab, position, Quaternion.identity);

            
            if (spatialGrid[(int)Mathf.Floor(position.x), (int)Mathf.Floor(position.y), (int)Mathf.Floor(position.z)] == null)
            {

                spatialGrid[(int)Mathf.Floor(position.x),
                            (int)Mathf.Floor(position.y),
                            (int)Mathf.Floor(position.z)] = new List<GameObject>();
            }

            spatialGrid[(int)Mathf.Floor(position.x),
                        (int)Mathf.Floor(position.y),
                        (int)Mathf.Floor(position.z)].Add(boidList[i]);

        }

        position = calculateMedianPoint();
    }

    void Update()
    {
        InitSpatialGrid();

//        cube.transform.position = calculateMedianPoint();
        //for each voxel in uniform spatial grid
        UpdateBoidPositions();
        //now all of the positions are updated, boids need to be rewritten to spatial grid

    }
}