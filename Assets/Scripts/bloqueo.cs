using UnityEngine;

public class bloqueo : MonoBehaviour
{
    public float velocidad = 10f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mover();
    }

    void mover(){
        if(transform.position.x < -5.55f){
            velocidad = 10;
        }
        if(transform.position.x > 5.55f){
            velocidad = -10;
        }
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }
}
