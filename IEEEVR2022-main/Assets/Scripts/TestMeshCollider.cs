using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeshCollider : MonoBehaviour
{
    public Collider collider;
    public SkinnedMeshRenderer mesh;
    public GameObject gameObject;

    // Start is called before the first frame update
    void Update()
    {
        //Debug.Log(mesh.bounds.Contains(gameObject.transform.position));
        Debug.Log(this.GetComponent<Renderer>().bounds.Contains(gameObject.transform.position));
        //this.transform.bounds
        // Debug.Log( (collider.bounds.Contains(gameObject.transform.position))) ;
        //Debug.Log( (collider.bounds.SqrDistance(gameObject.transform.position))) ;
        
    }

}
