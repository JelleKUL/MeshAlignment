using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCutter : MonoBehaviour
{
    [SerializeField]
    private Mesh targetMesh;

    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private float rad;

    [SerializeField]
    [Min(0)]
    private int maxLoopsPerframe = 100;

    Mesh readyMesh;
    bool ready = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(startPosition, Mathf.Pow(rad,2));
    }

    /*
    // Start is called before the first frame update
    public void StartMesh()
    {
        Debug.Log("Starting Mesh");
        StartCoroutine(MyCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (ready)
        {
            GetComponent<MeshFilter>().mesh = readyMesh;
            ready = false;
        }
    }

    

    // this is a coroutine body
    IEnumerator MyCoroutine()
    {
        // concurrently run something
        System.Func<Mesh> concurrentMethod = SetNewMesh;
        var concurrentResult = concurrentMethod.BeginInvoke(null, null);
        while (!concurrentResult.IsCompleted)
        {
            yield return new WaitForEndOfFrame();
        }
        //readyMesh = concurrentMethod.EndInvoke(concurrentResult);
        //ready = true;
        Debug.Log("Done");
     }

    Mesh SetNewMesh()
    {
        return CreateBoundMesh(targetMesh.vertices, targetMesh.triangles, startPosition, rad);
    }

    public void SetBoundsMesh()
    {
        Debug.Log("Starting getting the mesh");
        GetComponent<MeshFilter>().mesh = GetMeshAsync().Result;
    }

    async Task<Mesh> GetMeshAsync()
    {
        Debug.Log("Starting the async task");
        var result = await Task.Run(() =>CreateBoundMesh(targetMesh.vertices, targetMesh.triangles, startPosition, rad));
        Debug.Log("async task completed");
        return result;
    }
    */

    private void Start()
    {
        StartCoroutine(CreateBoundMesh(targetMesh.vertices, targetMesh.triangles, startPosition, rad));
    }

    IEnumerator CreateBoundMesh(Vector3[] oldVerts, int[] oldTris, Vector3 centerPostion, float radius)
    {
        float startTime = Time.time;

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        for (int i = 0; i < oldTris.Length; i+=3)
        {
            bool triIsIn = false; // assume all the vertices of the tri are outide the radius. if there is a single point inside, we add the tri to the new mesh

            for (int j = 0; j < 3; j++)
            {
                if(Vector3.SqrMagnitude(oldVerts[oldTris[i+j]] - centerPostion) < radius)
                {
                    //the vertex inside the bounding radius, so the we include this tri
                    triIsIn = true;
                }
            }

            if (triIsIn)
            {
                // add the verts to the list
                for (int j = 0; j < 3; j++)
                {
                    verts.Add(oldVerts[oldTris[i + j]]);
                }

                // add the tris to the list
                for (int j = 0; j < 3; j++)
                {
                    tris.Add(verts.Count-3 + j);
                }
            }
            Debug.Log("Tris: " + i + " are done");

            if(i % (maxLoopsPerframe*3) == 0)
            {
                Debug.Log("Done enough for one frame");
                yield return new WaitForEndOfFrame();
            }
        }

        Mesh newMesh = new Mesh();
        newMesh.name = "TerrainMesh";
        newMesh.vertices = verts.ToArray();
        newMesh.triangles = tris.ToArray();

        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        //return newMesh;

        GetComponent<MeshFilter>().mesh = newMesh;

        float endTime = Time.time;

        Debug.Log("time elapsed: " + (endTime - startTime));
    }
}
