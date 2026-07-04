using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed;
    [SerializeField] float lifetime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //destroy bullet after "lifetime" seconds
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        //makes bullet move forward
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }
}
