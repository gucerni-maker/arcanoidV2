using UnityEngine;

public class movimientoPelota : MonoBehaviour
{
    public float velocidad = 10f;
    private Rigidbody2D rb;
    private gameManager gm;
    public AudioClip sonidoPaleta;//No olvidar agregar un componente Audio Source al prefab de la pelota (sin ninguna modificacion, es requerido para dar sonido)
    private AudioSource playerAudio;
  
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //Nos comunicamos con el gameManager
        gm = FindFirstObjectByType<gameManager>();

        //creamos el audio para el efecto de sonido del rebote
        playerAudio = GetComponent<AudioSource>();

        LanzarPelota();
    }

    void Update()
    {

    }

    void LanzarPelota(){
        float[] angulos ={30f,45f,60f,120f,135f,150f};
        float angulo = angulos[Random.Range(0, angulos.Length)];
        float rad = angulo * Mathf.Deg2Rad;
        Vector2 direccion = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        rb.linearVelocity = direccion * velocidad;
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("bordeInferior")){
            if (gm != null){
                gm.PelotaPerdida(); 
            }

           Destroy(gameObject);
        }
    } 

    // Corregimos el problema de rebote horizontal y agregamos deflexion a la paleta (si la pelota golpea el borde de la paleta, la pelota cambia su direccion)
    void OnCollisionEnter2D(Collision2D collision){
        // 1. Verificamos si lo que golpeamos es la paleta
        if (collision.gameObject.CompareTag("paleta")){
            playerAudio.PlayOneShot(sonidoPaleta, 1.0f);
            AplicarDeflexionDePaleta(collision);
        }
        else{
            // 2. Si es una pared o un ladrillo, aplicamos la corrección anti-horizontal
            CorregirTrayectoriaHorizontal();
        }
    }  

    void AplicarDeflexionDePaleta(Collision2D collision){
        // A. Obtenemos el punto exacto donde la pelota tocó la paleta
        ContactPoint2D contact = collision.contacts[0];

        // B. Obtenemos el ancho real de la paleta (tamaño del collider * escala del objeto)
        BoxCollider2D paddleCollider = collision.collider.GetComponent<BoxCollider2D>();
        float anchoRealPaleta = paddleCollider.size.x * collision.transform.localScale.x;

        // C. Calculamos la diferencia entre el punto de golpe y el centro de la paleta
        float centroPaletaX = collision.transform.position.x;
        float offsetDeGolpe = contact.point.x - centroPaletaX;

        // D. Normalizamos ese valor para que esté entre -1 (borde izquierdo) y 1 (borde derecho)
        float golpeNormalizado = offsetDeGolpe / (anchoRealPaleta / 2f);
        
        // Por seguridad, limitamos el valor entre -1 y 1 (por si la física genera un punto de contacto extraño)
        golpeNormalizado = Mathf.Clamp(golpeNormalizado, -1f, 1f);

        // E. Definimos el ángulo máximo de rebote (60 grados es un estándar muy bueno)
        float anguloMaximoRebote = 60f;
        
        // Calculamos el ángulo final. Si golpeó el centro (0), el ángulo es 0.
        float anguloDeRebote = golpeNormalizado * anguloMaximoRebote;

        // F. Convertimos ese ángulo a una dirección Vector2.
        // Matemáticamente: Seno para X, Coseno para Y. 
        // A 0 grados, Seno(0)=0, Coseno(0)=1 -> Dirección (0, 1) que es recto hacia arriba.
        float radianes = anguloDeRebote * Mathf.Deg2Rad;
        Vector2 nuevaDireccion = new Vector2(Mathf.Sin(radianes), Mathf.Cos(radianes));

        // G. Aplicamos la nueva dirección, manteniendo la velocidad constante
        rb.linearVelocity = nuevaDireccion * velocidad;
    }  

    void CorregirTrayectoriaHorizontal()
    {
        // 1. Definimos la velocidad vertical MÍNIMA que permitimos
        float minVelocidadY = 3.5f; 

        // 2. Obtenemos la velocidad actual de la pelota
        Vector2 velocidadActual = rb.linearVelocity;

        // 3. Verificamos si el movimiento vertical es demasiado pequeño (casi horizontal)
        if (Mathf.Abs(velocidadActual.y) < minVelocidadY)
        {
            // Determinamos si la pelota iba hacia arriba (1) o hacia abajo (-1)
            int direccionY = velocidadActual.y >= 0 ? 1 : -1;
            
            // Determinamos si la pelota iba hacia la derecha (1) o izquierda (-1)
            int direccionX = velocidadActual.x >= 0 ? 1 : -1;

            // 4. Forzamos la velocidad Y al mínimo permitido
            float nuevaVelocidadY = minVelocidadY * direccionY;

            // 5. Recalculamos la velocidad X para que la velocidad TOTAL de la pelota no cambie.
            // Usamos Pitágoras: (VelocidadTotal)^2 = (VelX)^2 + (VelY)^2
            // Despejando VelX: VelX = RaizCuadrada( (VelTotal)^2 - (VelY)^2 )
            float nuevaVelocidadX = direccionX * Mathf.Sqrt(Mathf.Pow(velocidad, 2) - Mathf.Pow(minVelocidadY, 2));

            // 6. Aplicamos la nueva velocidad corregida
            rb.linearVelocity = new Vector2(nuevaVelocidadX, nuevaVelocidadY);
            
        }
    }   
}
 