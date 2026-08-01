using UnityEngine;

public class ladrilloVerde : MonoBehaviour
{
    private int golpe = 0;
    private SpriteRenderer cambiaColor;

    void Start()
    {
        cambiaColor = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision){
        golpe++;
        if(golpe == 1){
            cambiaColor.color = Color.blue;
        }
        if (collision.gameObject.CompareTag("pelota") && golpe == 2){//Se destruye a los 2 golpes
            //Nos comunicamos con el gameManager
            gameManager gm = FindFirstObjectByType<gameManager>();

            //Le decimos al gameManager que anote un punto
            gm.PuntoLadrilloVerde();

            Destroy(gameObject);
            golpe = 0;
        } 
    }
}
