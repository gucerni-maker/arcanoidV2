using UnityEngine;

public class helice : MonoBehaviour
{

    public GameObject helice1;
    public GameObject helice2;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        helice1.transform.Rotate(new Vector3(0, 0, -30) * Time.deltaTime);
        helice2.transform.Rotate(new Vector3(0, 0, 30) * Time.deltaTime);
    }

}
