using UnityEngine;

public class ladrilloRojo : MonoBehaviour
{
    private int golpe = 0;
    //private SpriteRenderer cambiaColor;
    private SpriteRenderer spriteRenderer;
    public Sprite spriteDanado, spriteDanado2;

    void Start()
    {
        //cambiaColor = GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision){
        golpe++;
        if(golpe == 1){
            //cambiaColor.color = Color.blue;
            spriteRenderer.sprite = spriteDanado;
        }
        if(golpe == 2){
           //cambiaColor.color = Color.blue;
            spriteRenderer.sprite = spriteDanado2;
        }

        if (collision.gameObject.CompareTag("pelota") && golpe == 3){//Se destruye a los 3 golpes
            //Nos comunicamos con el gameManager
            gameManager gm = FindFirstObjectByType<gameManager>();

            //Le decimos al gameManager que anote un punto
            gm.PuntoLadrilloRojo();
            
            Destroy(gameObject);
            golpe = 0;
        } 
    }
}
