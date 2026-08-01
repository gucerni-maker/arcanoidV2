using UnityEngine;

public class ladrilloAzul : MonoBehaviour
{

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("pelota")){
            
            //Nos comunicamos con el gameManager
            gameManager gm = FindFirstObjectByType<gameManager>();

            //Le decimos al gameManager que anote un punto
            gm.PuntoLadrilloAzul();
            
            Destroy(gameObject);
        } 
    }
}
