using UnityEngine;

public class paleta : MonoBehaviour
{
    public float velocidad = 8f;
    private float limiteIzq = -5.21f;
    private float limiteDer = 5.16f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moverPaleta();

        //controla que la paleta no pase el limite izquierdo y derecho
        if (transform.position.x < limiteIzq){
            transform.position = new Vector2(limiteIzq, transform.position.y);
        }
        else if (transform.position.x > limiteDer){
            transform.position = new Vector2(limiteDer, transform.position.y);
        }
    }

    void moverPaleta(){
        float movimiento = 0;
        if (Input.GetKey(KeyCode.RightArrow)){
            movimiento = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow)){
            movimiento = -1;
        }
        transform.Translate(Vector2.right * movimiento * velocidad * Time.deltaTime);
    }

    //para indicarle al gameManager la posicion de la paleta
    public float ObtenerPosicionX(){
        return transform.position.x;
    }
}
