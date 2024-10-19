using UnityEngine;

public class AppleMove : MonoBehaviour
{
    private GameObject apple;
    private GameObject banana;
    private float speed = 5f;

    private void Start()
    {
        apple = GameObject.Find("Apple");
        banana = GameObject.Find("Banana");
    }

    private void Update()
    {
        Vector3 direction = banana.transform.position - apple.transform.position;
        apple.transform.Translate(direction.normalized * speed * Time.deltaTime);
    }
}
