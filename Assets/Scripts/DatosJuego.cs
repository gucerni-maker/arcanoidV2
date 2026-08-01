using UnityEngine;

public class DatosJuego : MonoBehaviour
{
    public static DatosJuego Instance;

    public int puntaje1 = 0;
    public int vidas = 3;
    /*
    public int puntaje2 = 0;
    public int victoriaJ1 = 0;
    public int victoriaJ2 = 0;
    public int resultadoSet1P1 = 0;
    public int resultadoSet2P1 = 0;
    public int resultadoSet3P1 = 0;
    public int resultadoSet1P2 = 0;
    public int resultadoSet2P2 = 0;
    public int resultadoSet3P2 = 0;
    */

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
