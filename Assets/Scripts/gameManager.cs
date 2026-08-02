using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;//requerido para usar SceneManager (reiniciar el juego)
using System.Collections;// requerido para usar IEnumerator

public class gameManager : MonoBehaviour
{

    public AudioClip sonidoLadrillo;//No olvidar agregar un componente Audio Source al prefab del ladrillo (sin ninguna modificacion, es requerido para dar sonido)
    public GameObject pelotaPrefab;
    public GameObject bloqueo;//la barra que se mueve en el nivel 2
    public GameObject ladrilloAzul, ladrilloVerde, ladrilloRojo;
    public GameObject paletaPlayer;
    public GameObject helice1, helice2;
    public paleta posicionPaleta; //debe tener el mismo nombre que el script para poder referenciar
    public UIDocument uiDocument;
    public DatosJuego DatosJuego;
    
    private Label scoreText1, vidasRestantes, puntaje, cronometro, nivel, etapa;
    private Label finDelJuego, continuarJuego, sinTiempo, victoria, contador;
    private Button restartButton, iniciar, siguiente;
    private GameObject pelotaActual;
    private AudioSource playerAudio;

    private bool esperandoInput = false;
    private bool timerActivo = true;
    private float tiempoRestante = 120f; //tiempo del cronometro
    private int cantidadVidas = 3;
    private int cuentaLadrillos = 0;

    
    private float[] bloquePosX = {-5f, -3f, -1f, 1f, 3f, 5f}; //Fila completa
    private float[] bloqueNivel2 = {-5f, -1f, 3f};//uno si y uno no
    private float[] bloqueNivel3 = {-3f, 1f, 5f};//uno si y uno no

    void Start()
    {

        playerAudio = GetComponent<AudioSource>();

        //instanciamos los labels
        scoreText1 = uiDocument.rootVisualElement.Q<Label>("puntaje");
        vidasRestantes = uiDocument.rootVisualElement.Q<Label>("vidas");
        finDelJuego = uiDocument.rootVisualElement.Q<Label>("gameOver");
        continuarJuego = uiDocument.rootVisualElement.Q<Label>("continuar");
        cronometro = uiDocument.rootVisualElement.Q<Label>("tiempo");
        sinTiempo = uiDocument.rootVisualElement.Q<Label>("sinTiempo");
        victoria = uiDocument.rootVisualElement.Q<Label>("victoria");
        nivel = uiDocument.rootVisualElement.Q<Label>("nivel");
        etapa = uiDocument.rootVisualElement.Q<Label>("etapa");
        contador = uiDocument.rootVisualElement.Q<Label>("contador");

        //instanciamos los botones
        restartButton = uiDocument.rootVisualElement.Q<Button>("reiniciar");
        iniciar = uiDocument.rootVisualElement.Q<Button>("iniciar");
        siguiente = uiDocument.rootVisualElement.Q<Button>("siguiente");
        
        //asociamos los botones a su respectiva funcion
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
        paletaPlayer.gameObject.SetActive(false);
        contador.style.display = DisplayStyle.None;//contador 3, 2, 1

        //Obtenemos el puntaje del almacenamiento
        scoreText1.text = DatosJuego.Instance.puntaje1.ToString();

        //Obtenemos las vidas restantes del almacenamiento
        vidasRestantes.text = DatosJuego.Instance.vidas.ToString();

        //modifica elementos al inicio del juego
        if(SceneManager.GetActiveScene().buildIndex == 0){
            etapa.text = "Nivel 1";
            nivel.text = "1";
        }
        if(SceneManager.GetActiveScene().buildIndex == 1){
            etapa.text = "Nivel 2";
            nivel.text = "2";
            bloqueo.gameObject.SetActive(false);
        }
        if(SceneManager.GetActiveScene().buildIndex == 2){
            etapa.text = "Nivel 3";
            nivel.text = "3";
            bloqueo.gameObject.SetActive(false);
            helice1.gameObject.SetActive(false);
            helice2.gameObject.SetActive(false);
        }        
        
    }

    void Update()
    {
        //permite continuar el juego, luego de perder 1 vida presiando la barra de espacio
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

    //Inicia la corutina para el cronometro y oculta el boton de inicio
    public void iniciarJuego(){
        //al presionar el boton iniciar, se oculta el boton
        iniciar.style.display = DisplayStyle.None;
        StartCoroutine(CuentaRegresiva());
    }

    //Crea una cuenta regresiva y luego inicia la partida
    IEnumerator CuentaRegresiva(){
        paletaPlayer.gameObject.SetActive(true);
        
        //muestra la barra blanca que se mueve en el nivel 2
        if(SceneManager.GetActiveScene().buildIndex == 1){
            bloqueo.gameObject.SetActive(true);
        }

        //muestra la barra blanca y las helices en el nivel 3
        if(SceneManager.GetActiveScene().buildIndex == 2){
            bloqueo.gameObject.SetActive(true);
            helice1.gameObject.SetActive(true);
            helice2.gameObject.SetActive(true);
        }        
        
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

        //Oculta el contador luego de llegar a cero
        contador.style.display = DisplayStyle.None;
        //Oculta el mensaje Nivel # que se muestra antes de iniciar el juego
        etapa.style.display = DisplayStyle.None;
        
        SpawnPelota();
    }

    //########  PARA MANEJAR LA CANTIDAD DE VIDAS ##########
    public void PelotaPerdida(){
        esperandoInput = true;
        //cantidadVidas--;
        DatosJuego.Instance.vidas--;
        vidasRestantes.text = DatosJuego.Instance.vidas.ToString();
        
        if(DatosJuego.Instance.vidas < 1){
            GameOver();
        }
        if(DatosJuego.Instance.vidas > 0){
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

        //Nivel 1
        if(SceneManager.GetActiveScene().buildIndex == 0){
            for(int i = 0; i < bloquePosX.Length; i++){
                Instantiate(ladrilloAzul, new Vector2(bloquePosX[i], 2.14f), Quaternion.identity);
                Instantiate(ladrilloVerde, new Vector2(bloquePosX[i], 2.88f), Quaternion.identity);
                Instantiate(ladrilloRojo, new Vector2(bloquePosX[i], 3.62f), Quaternion.identity);
            }
            //multiplicamos el largo del arreglo por la cantidad de instancias
            cuentaLadrillos = bloquePosX.Length * 3;
        }

        //Nivel 2
        if(SceneManager.GetActiveScene().buildIndex == 1){
            for(int i = 0; i < bloqueNivel2.Length; i++){
                Instantiate(ladrilloAzul, new Vector2(bloqueNivel2[i], 2.14f), Quaternion.identity);
                Instantiate(ladrilloVerde, new Vector2(bloqueNivel3[i], 2.88f), Quaternion.identity);
                Instantiate(ladrilloAzul, new Vector2(bloqueNivel2[i], 3.62f), Quaternion.identity);
                Instantiate(ladrilloVerde, new Vector2(bloqueNivel3[i], 4.36f), Quaternion.identity);
            }
            //multiplicamos el largo del arreglo por la cantidad de instancias
            cuentaLadrillos = bloqueNivel2.Length * 4;
        }        

        //Nivel 3
        if(SceneManager.GetActiveScene().buildIndex == 2){
            for(int i = 0; i < bloquePosX.Length; i++){
                Instantiate(ladrilloAzul, new Vector2(bloquePosX[i], 2.14f), Quaternion.identity);
                Instantiate(ladrilloVerde, new Vector2(bloquePosX[i], 2.88f), Quaternion.identity);
                Instantiate(ladrilloAzul, new Vector2(bloquePosX[i], 3.62f), Quaternion.identity);
                Instantiate(ladrilloVerde, new Vector2(bloquePosX[i], 4.36f), Quaternion.identity);
            }
            //multiplicamos el largo del arreglo por la cantidad de instancias
            cuentaLadrillos = bloquePosX.Length * 4;
        }        
        
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
        DatosJuego.Instance.puntaje1+=10;//se agregan 10 puntos al storage  

        //Actualiza el puntaje en la UI
        scoreText1.text = DatosJuego.Instance.puntaje1.ToString();
        
        //Si la cantidad de ladrillos es cero y la escena es cero, se pasa al otro nivel
        //if(cuentaLadrillos == 0 && escena == 0){
        if(cuentaLadrillos == 0 && escena == 0){
            SiguienteNivel();
        }
        if(cuentaLadrillos == 0 && escena == 1){
            SiguienteNivel();
        }

        //si la cantidad de ladrillos es cero y la escena es uno, se gana
        if(cuentaLadrillos == 0 && escena == 2){
            Victoria();
        }

        //cada 200 puntos damos una vida
        if(DatosJuego.Instance.puntaje1 % 200 == 0){
            DatosJuego.Instance.vidas++;
            vidasRestantes.text = DatosJuego.Instance.vidas.ToString();
        }
    }
    public void PuntoLadrilloVerde(){
        int escena = SceneManager.GetActiveScene().buildIndex;
        cuentaLadrillos--;
        playerAudio.PlayOneShot(sonidoLadrillo, 1.0f);
        DatosJuego.Instance.puntaje1 +=  20;
        scoreText1.text = DatosJuego.Instance.puntaje1.ToString();

        if(cuentaLadrillos == 0 && escena == 0){
            SiguienteNivel();
        }
        if(cuentaLadrillos == 0 && escena == 1){
            SiguienteNivel();
        }
        if(cuentaLadrillos == 0 && escena == 2){
            Victoria();
        }

        //cada 200 puntos damos una vida
        if(DatosJuego.Instance.puntaje1 % 200 == 0){
            DatosJuego.Instance.vidas++;
            vidasRestantes.text = DatosJuego.Instance.vidas.ToString();
        }

    }
    public void PuntoLadrilloRojo(){
        int escena = SceneManager.GetActiveScene().buildIndex;
        cuentaLadrillos--;
        playerAudio.PlayOneShot(sonidoLadrillo, 1.0f);
        DatosJuego.Instance.puntaje1 +=  30;
        scoreText1.text = DatosJuego.Instance.puntaje1.ToString();

        if(cuentaLadrillos == 0 && escena == 0){
            SiguienteNivel();
        }
        if(cuentaLadrillos == 0 && escena == 1){
            SiguienteNivel();
        }
        if(cuentaLadrillos == 0 && escena == 2){
            Victoria();
        } 

        //cada 200 puntos damos una vida
        if(DatosJuego.Instance.puntaje1 % 200 == 0){
            DatosJuego.Instance.vidas++;
            vidasRestantes.text = DatosJuego.Instance.vidas.ToString();
        }       
    }
    //######################################################

    //###########  PASAR AL SIGUIENTE NIVEL ################
    public void SiguienteNivel(){
                
         if (pelotaActual != null){
            Destroy(pelotaActual);
            pelotaActual = null;      
        }

        //muestra el boton "siguiente"
        siguiente.style.display = DisplayStyle.Flex;

        //Mostramos el mensaje de victoria
        victoria.style.display = DisplayStyle.Flex;

        //ocultamos la paleta antes de iniciar el juego
        paletaPlayer.gameObject.SetActive(false);
    }
    //######################################################
    
    //########### REINICIA EL JUEGO ########################
    void ReloadScene(){
        SceneManager.LoadScene(0);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);//Recarga la escena actual
        
        //Se resetea el puntaje
        DatosJuego.Instance.puntaje1 = 0;

        //Actualiza el puntaje en la UI
        scoreText1.text = DatosJuego.Instance.puntaje1.ToString();

        //Se resetean las vidas
        DatosJuego.Instance.vidas = 3;

        //Actualiza las vidas en la UI
        vidasRestantes.text = DatosJuego.Instance.vidas.ToString();
    }
    //######################################################

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
    }
    //######################################################
}
