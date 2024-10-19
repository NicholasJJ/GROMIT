using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    private float creationTime;

    [SerializeField] public float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        creationTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > creationTime + lifetime) {
            Destroy(gameObject);
        }
        
    }

    public static IEnumerator WaitThenDestroy(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }

}
