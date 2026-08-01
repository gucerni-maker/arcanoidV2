using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;//requerido para usar SceneManager (reiniciar el juego)
using System.Collections;// requerido para usar IEnumerator
using UnityEngine.SceneManagement;//requerido para usar SceneManager

public class gameManager : MonoBehaviour
{

    public AudioClip sonidoLadrillo;//No olvidar agregar un componente Audio Source al prefab del ladrillo (sin ninguna modificacion, es requerido para dar sonido)
    public GameObject pelotaPrefab;
    public GameObject ladrilloAzul, ladrilloVerde, ladrilloRojo;
    public GameObject paletaPlayer;
    public paleta posicionPaleta; //debe tener el mismo nombre que el script para poder referenciar
    public UIDocument uiDocument;
    
    private Label scoreText1, vidasRestantes, puntaje, cronometro, nivel, etapa;
    private Label finDelJuego, continuarJuego, sinTiempo, victoria, contador;
    private Button restartButton, iniciar, siguiente;
    private GameObject pelotaActual;
    private AudioSource playerAudio;

    private bool esperandoInput = false;
    private bool timerActivo = true;
    private float tiempoRestante = 120f; //tiempo del cronometro
    private int puntajeTotal = 0;
    private int cantidadVidas = 3;
    private int cuentaLadrillos = 0;

    
    private float[] bloquePosX = {-5f, -3f, -1f, 1f, 3f, 5f}; 

    void Start()
    {

        playerAudio = GetComponent<AudioSource>();

        scoreText1 = uiDocument.rootVisualElement.Q<Label>("puntaje");
        vidasRestantes = uiDocument.rootVisualElement.Q<Label>("vidas");
        finDelJuego = uiDocument.rootVisualElement.Q<Label>("gameOver");
        continuarJuego = uiDocument.rootVisualElement.Q<Label>("continuar");
        cronometro = uiDocument.rootVisualElement.Q<Label>("tiempo");
        sinTiempo = uiDocument.rootVisualElement.Q<Label>("sinTiempo");
        victoria = uiDocument.rootVisualElement.Q<Label>("victoria");
        nivel = uiDocument.rootVisualElement.Q<Label>("nivel");
        etapa = uiDocument.rootVisualElement.Q<Label>("etapa");
        restartButton = uiDocument.rootVisualElement.Q<Button>("reiniciar");
        iniciar = uiDocument.rootVisualElement.Q<Button>("iniciar");
        siguiente = uiDocument.rootVisualElement.Q<Button>("siguiente");
        contador = uiDocument.rootVisualElement.Q<Label>("contador");

        restartButton.clicked += ReloadScene;
        iniciar.clicked += iniciarJuego;
        siguiente.clicked += siguienteScene;
        
        //Oculta elementos de la interfaz, como pantalla de gameover y boton restart
        finDelJuego.style.display = DisplayStyle.None;
        sinTiempo.style.display = DisplayStyle.None;
        continuarJuego.style.display = DisplayStyle.None;
        victoria.style.display = DisplayStyle.None;
        restartButton.style.display = DisplayStyle.None;
        siguiente.style.display = DisplayStyle.None;

        //contador 3, 2, 1
        contador.style.display = DisplayStyle.None;
        
    }

    void Update()
    {
         if (esperandoInput && Input.GetKeyDown(KeyCode.Space) && cantidadVidas > 0){
            SpawnPelota();
            esperandoInput = false;
            continuarJuego.style.display = DisplayStyle.None;
        }

        //############## PARA EL CRONOMETRO ################
        if (timerActivo && cantidadVidas > 0)
        {
            
            

            // Actualizar el texto en pantalla
            if (pelotaActual != null){
                tiempoRestante -= Time.deltaTime;
                int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
                int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
                cronometro.text = string.Format("{0:00}:{1:00}", minutos, segundos);
            }
            // Verificar si el tiempo se acabó
            if (tiempoRestante <= 0f)
            {
                tiempoRestante = 0f;
                timerActivo = false;
                cantidadVidas = 0;
                GameOver();
            }
        }
        //##################################################

    }

    //Oculta el boton de inicio y llama a una cuenta regresiva antes de iniciar
    public void iniciarJuego(){
        iniciar.style.display = DisplayStyle.None;
        StartCoroutine(CuentaRegresiva());
       
    }
    //Crea una cuenta regresiva y luego inicia la partida
    IEnumerator CuentaRegresiva(){
        contador.style.display = DisplayStyle.Flex;
        contador.text = "3";
        CreaLadrillo();

        yield return new WaitForSeconds(1);

        contador.text = "2";
        yield return new WaitForSeconds(1);

        contador.text = "1";
        yield return new WaitForSeconds(1);

        contador.text = "Ya!";
        yield return new WaitForSeconds(1);

        contador.style.display = DisplayStyle.None;
        etapa.style.display = DisplayStyle.None;
        
        SpawnPelota();
    }

    //########  PARA MANEJAR LA CANTIDAD DE VIDAS ##########
    public void PelotaPerdida(){
        esperandoInput = true;
        cantidadVidas--;
        vidasRestantes.text = cantidadVidas.ToString();
        
        if(cantidadVidas < 1){
            GameOver();
        }
        if(cantidadVidas > 0){
            continuarJuego.style.display = DisplayStyle.Flex;
        }
    }
    //######################################################


    //##########  PARA LA PANTALLA GAME OVER #############
    public void GameOver(){
        finDelJuego.style.display = DisplayStyle.Flex;
        restartButton.style.display = DisplayStyle.Flex;

        if(tiempoRestante <= 0){
            sinTiempo.style.display = DisplayStyle.Flex;
        }
        
        timerActivo = false;
        cronometro.text = "00:00";
        vidasRestantes.text = "0";
        
        if (pelotaActual != null)
        {
            Destroy(pelotaActual);
            pelotaActual = null;
        }
    }
    //######################################################


    //###########  PARA LA CREACION DE LA PELOTA ###########
    public void SpawnPelota(){
        
        //obtenemos la posicion x de la paleta
        float posicionX = posicionPaleta.ObtenerPosicionX(); 

        //crea una pelota y entrega una referencia para destruirla al acabar el tiempo
        pelotaActual = Instantiate(pelotaPrefab, new Vector2(posicionX,-3.65f), Quaternion.identity);
    }
    //######################################################


    //###########  PARA LA CREACION DE LADRILLOS ###########
    public void CreaLadrillo(){
        
        for(int i = 0; i < bloquePosX.Length; i++){
            Instantiate(ladrilloAzul, new Vector2(bloquePosX[i], 2.14f), Quaternion.identity);
            Instantiate(ladrilloVerde, new Vector2(bloquePosX[i], 2.88f), Quaternion.identity);
            Instantiate(ladrilloRojo, new Vector2(bloquePosX[i], 3.62f), Quaternion.identity);
        }
        //multiplicamos el largo del arreglo por la cantidad de instancias
        cuentaLadrillos = bloquePosX.Length * 3;
    }
    //######################################################


    //#### LO QUE SUCEDE LUEGO DE DESTRUIR UN LADRILLO #####

    //controla el puntaje de cada ladrillo
    public void PuntoLadrilloAzul(){
        int escena = SceneManager.GetActiveScene().buildIndex;
        //Descuenta 1 ladrillo cada vez que se destruye
        cuentaLadrillos--;

        //Reproduce un sonido
        playerAudio.PlayOneShot(sonidoLadrillo, 1.0f);
        
        //Agregar un puntaje a la variable  
        puntajeTotal += 10;

        //Actualiza el puntaje en la UI
        scoreText1.text = puntajeTotal.ToString();
        
        //Si la cantidad de ladrillos es cero y la escena es cero, se pasa al otro nivel
        if(cuentaLadrillos == 0 && escena == 0){
            SiguienteNivel();
        }

        //si la cantidad de ladrillos es cero y la escena es uno, se gana
        if(cuentaLadrillos == 0 && escena == 1){
            Victoria();
        }
    }
    public void PuntoLadrilloVerde(){
        int escena = SceneManager.GetActiveScene().buildIndex;
        cuentaLadrillos--;
        playerAudio.PlayOneShot(sonidoLadrillo, 1.0f);
        puntajeTotal +=  20;
        scoreText1.text = puntajeTotal.ToString();

        if(cuentaLadrillos == 0 && escena == 0){
            SiguienteNivel();
        }
        if(cuentaLadrillos == 0 && escena == 1){
            Victoria();
        }

    }
    public void PuntoLadrilloRojo(){
        int escena = SceneManager.GetActiveScene().buildIndex;
        cuentaLadrillos--;
        playerAudio.PlayOneShot(sonidoLadrillo, 1.0f);
        puntajeTotal +=  30;
        scoreText1.text = puntajeTotal.ToString();

        if(cuentaLadrillos == 0 && escena == 0){
            SiguienteNivel();
        }
        if(cuentaLadrillos == 0 && escena == 1){
            Victoria();
        }        
    }
    //######################################################

        //######################################################

    //###########  PASAR AL SIGUIENTE NIVEL ################
    public void SiguienteNivel(){
        
        /*int escena = SceneManager.GetActiveScene().buildIndex;
        if (escena == 0){
            DatosJuego.Instance.resultadoSet1P1 = puntoPlayer1;
            DatosJuego.Instance.resultadoSet1P2 = puntoPlayer2;
        }
        if (escena == 1){
            DatosJuego.Instance.resultadoSet2P1 = puntoPlayer1;
            DatosJuego.Instance.resultadoSet2P2 = puntoPlayer2;
        }
        if (escena == 2){
            DatosJuego.Instance.resultadoSet3P1 = puntoPlayer1;
            DatosJuego.Instance.resultadoSet3P2 = puntoPlayer2;
        }*/
        
         if (pelotaActual != null){
            Destroy(pelotaActual);
            pelotaActual = null;      
        }

        siguiente.style.display = DisplayStyle.Flex;
    }
    //######################################################
    
    //reinicia el juego luego de presionar el boton de reiniciar
    void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //########  CONTROLA LA VENTANA DE VICTORIA ############
    public void Victoria(){
        //Destruimos la pelota y la paleta
        if (pelotaActual != null){
            Destroy(pelotaActual);
            Destroy(paletaPlayer);
            pelotaActual = null;
        }

        //Mostramos el mensaje de victoria
        victoria.style.display = DisplayStyle.Flex;

        //Detenemos el tiempo
        timerActivo = false;
        cronometro.text = "00:00";

        //Mostramos el boton para reiniciar la partida
        restartButton.style.display = DisplayStyle.Flex;
    }
    //######################################################

    //############ Cambia al siguiente nivel ###############
    void siguienteScene(){

        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(escenaActual + 1);

        if (escenaActual == 0){
            etapa.style.display = DisplayStyle.Flex;
            nivel.text = "2";
            etapa.text = "Nivel 2";
        } 
    }
    //######################################################
}
