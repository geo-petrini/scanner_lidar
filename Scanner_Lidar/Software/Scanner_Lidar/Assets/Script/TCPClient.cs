using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TCPClient : MonoBehaviour
{
    /// <summary>
    /// Questo codice è stato riadattato da questo link: https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb#file-tcptestclient-cs-L26
    /// Inserirò i commenti anche come dimostrazione di aver compreso quello che ho letto (Veljko)
    /// </summary>

    // Bottone che effettua il tentativo di connessione al Server
    public Button connectButton;

    // Bottone che si scollega dal Server
    public Button disconnectButton;

    // Bottone che dice al Server di inviare i dati al Client
    public Button receiveButton;

    // Bottone che dice al Server di non inviare i dati al Client
    public Button stopButton;

    // Oggetto che userò per connettermi al server tramite il protocllo TCP
    private TcpClient connection;

    // Indica se è stata stabilita una connessione con il Server
    private bool connected;

    // Thread che in background si occuperà di leggere dati in arrivo dal server
    private Thread listener;

    // Indica se ritornare al display di startup
    private bool returnToStartupDisplay;

    // Messaggio ricevuto dal Server
    private string serverMessage;

    // Messaggio da inviare al Server da parte del Client
    private string clientMessage;

    // Stream che verrà usato per la lettura dei dati dal Server e per la scrittura dei dati verso il Server
    private NetworkStream stream;

    // Text dove appariranno vari messaggi di info
    public Text infoText;

    // Messaggio di informazione a schermo
    private string infoMessage;

    // Colore del messaggio di informazione
    private Color infoColor;

    // Mi indica se c'è da cambiare il messaggio di info
    private bool changeInfoMessage;

    // Riferimento al testo situato alla sinistra dell'InputField dell'intervallo
    public Text intervalText;

    // InputField che stabilirà l'intervallo di punti che richiederà il Client per volta
    public InputField intervalInputField;

    // Script PointController
    private PointController pointController;

    // Text dove scriverò il numero di punti renderizzati
    public Text pointsText;

    // Indica se renderizzare un punto
    private bool renderAPoint = false;

    // Indica se ho ricevuto tutti i punti
    private bool receivedAllPoints = false;

    void Start()
    {
        // Ottengo lo script Point Controller dalla Main Camera
        pointController = GameObject.Find("Main Camera").GetComponent<PointController>();

        // Aggiungo i listener ai bottoni
        InitializeButtonsListeners();

        // Rendo inattivi quelli che non mi servono all'inizio
        InitializeStartupGUI();
    }

    void Update()
    {
        // Cambia il messaggio di informazione (se richiesto)
        if(changeInfoMessage)
        {
            infoText.color = infoColor;
            infoText.text = infoMessage;
            changeInfoMessage = false;
        }

        // Mostra la disposizione della GUI una volta effettuata la connessione con il Server
        if(connected)
        {
            ConnectedGUI();
        }

        // Ritorna alla disposizione dei bottoni di startup (se necessario)
        if (returnToStartupDisplay)
        {
            InitializeStartupGUI();
        }

        // Renderizza un punto (se necessario)
        if(renderAPoint)
        {
            renderAPoint = false;
            pointController.Render(serverMessage);
            pointsText.text = "Points: " + pointController.GetIndexOfLastPoint().ToString();
        }

        // Informa l'utente che ho ricevuto tutti i punti (se necessario)
        if(receivedAllPoints)
        {
            OnStopClick();
        }
    }

    // Aggiunge i listener a tutti i bottoni della GUI 
    public void InitializeButtonsListeners()
    {
        connectButton.onClick.AddListener(OnConnectButton);
        disconnectButton.onClick.AddListener(OnDisconnectButton);
        receiveButton.onClick.AddListener(OnReceiveClick);
        stopButton.onClick.AddListener(OnStopClick);
    }

    // Disposizione della GUI iniziale
    public void InitializeStartupGUI()
    {
        returnToStartupDisplay = false;

        connectButton.gameObject.SetActive(true);
        connectButton.interactable = true;

        disconnectButton.gameObject.SetActive(false);
        receiveButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(false);
        intervalText.gameObject.SetActive(false);
        intervalInputField.gameObject.SetActive(false);
        pointsText.gameObject.SetActive(false);
    }

    // Disposizione della GUI (dopo la connessione con il Server)
    public void ConnectedGUI()
    {
        DisplayInfo("Connection to the Server succesfully established!", Color.green);

        connected = false;
        connectButton.gameObject.SetActive(false);
        disconnectButton.gameObject.SetActive(true);

        receiveButton.gameObject.SetActive(true);

        intervalText.gameObject.SetActive(true);
        intervalInputField.gameObject.SetActive(true);
        pointsText.gameObject.SetActive(true);
        pointsText.text = "Points: " + pointController.GetIndexOfLastPoint().ToString();
    }

    private void Connect()
    {
        try
        {
            // Inizializzo la Thread e le passo una nuova ThreadStart (rappresenta il metodo che verrà eseguito quando parte la Thread) con il metodo GetPointsData
            listener = new Thread(new ThreadStart(GetPointsData));
            // Indico alla Thread che dovrà essere una Background Thread e non una Foreground Thread (se si vuole terminare l'esecuzione di tutto, la Thread lo permetterà)
            listener.IsBackground = true;
            // Faccio partire la Thread
            listener.Start();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Metodo che parte quando la Thread listener lavora in background. Legge i dati in arrivo dal Server
    /// </summary>
    private void GetPointsData()
    {
        try
        {
            // Tento di effettuare la connessione con il Server
            DisplayInfo("Attempting to connect to the Server, please wait...", Color.white);
            Debug.Log("Attempting to connect to the Server, please wait...");

            connection = new TcpClient("10.20.4.185", 12345);

            //Debug.Log("Tentativo di connessione al Server terminato con successo!");
            Debug.Log("Connection to the Server succesfully established!");
            connected = true;

            // Istanzio il buffer, dove manterrò i dati ricevuti dal Server
            byte[] bytes = new byte[1024];
            while (true)
            {
                // Ottengo un oggetto Stream per la lettura dei dati
                stream = connection.GetStream();
                // Variabile dove conserverò il numero di byte letti dal NetworkStream
                int length;
                while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) // Se length è diverso da 0, quindi sono stati letti dei byte, continua il ciclo
                {
                    // Istanzio l'array nel quale salevrò i dati contenuti nel Buffer
                    var incomingData = new byte[length];
                    // Copio i dati dal Buffer all'array incomingData
                    Array.Copy(bytes, 0, incomingData, 0, length);
                    // Converto l'array di byte in una stringa che rappresenta il messaggio ricevuto dal Server
                    serverMessage = Encoding.ASCII.GetString(incomingData);
                    Debug.Log(serverMessage);
                    if (serverMessage == "#,#,#")
                    {
                        receivedAllPoints = true;
                    }else
                    {
                        renderAPoint = true;
                    }
                }
            }
        }
        catch (SocketException)
        {
            DisplayInfo("Unable to establish a connection with the Server", Color.red);
            Debug.Log("Unable to establish a connection with the Server");
        }
        catch (IOException)
        {
            DisplayInfo("Connection with the Server interrupted", Color.red);
            Debug.Log("Connection with the Server interrupted");
            ConnectionInterrupted();
        }
        catch (ObjectDisposedException)
        {
            DisplayInfo("Connection with the Server interrupted", Color.red);
            Debug.Log("Connection with the Server interrupted");
        }
        returnToStartupDisplay = true;
    }

    /// <summary>
    /// Invia un messaggio al Server utilizzando una connessione Socket
    /// </summary>
    private void SendMessage()
    {
        // Se non c'è nessuna connessione con il Server non verrà inviato nessun messaggio e si uscirà dal metodo
        if (connection == null || !connection.Connected)
        {
            return;
        }

        try
        {
            // Ottengo un oggetto Stream per la scrittura dei dati
            stream = connection.GetStream();

            // Se lo Stream può scrivere, si può procedere
            if (stream.CanWrite)
            {
                // Converto il messaggio da stringa ad array di byte, in modo tale da poterlo inviare al Server
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Scrive l'array di byte tramite lo stream di connection
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("The Client sent a message to the Server");
            }
        }
        catch (IOException)
        {
            Debug.Log("Connection with the Server interrupted");
            ConnectionInterrupted();
        }
    }

    // Chiude lo Stream e la connessione con il Server (se ce ne sono)
    public void ConnectionInterrupted()
    {
        if(stream != null && (stream.CanRead || stream.CanWrite))
        {
            stream.Close();
        }
        if(connection != null && connection.Connected)
        {
            connection.Close();
        }
    }

    // Modifica il messaggio che appare all'utente
    public void DisplayInfo(string msg, Color color)
    {
        changeInfoMessage = true;
        infoMessage = msg;
        infoColor = color;
    }

    // Quando viene cliccato il bottone "Connect to server", verrà invocato il metodo Connect()
    private void OnConnectButton()
    {
        // Rendo non interagibile il bottone mentre viene effettuato il tentativo di connessione con il Server
        connectButton.interactable = false;

        // Invoco il metodo che si occuperà di fare partire la Thread
        Connect();
    }

    // Quando viene cliccato il bottone "Disconnect from server", vengono invocati i metodi OnDisconnectButton() e InitializeStartupGUI()
    private void OnDisconnectButton()
    {
        ConnectionInterrupted();
        InitializeStartupGUI();
    }

    // Quando viene cliccato il bottone "Receive", al Server verrà inviata una richiesta per ricevere i dati con l'intervallo stabilito dall'utente
    public void OnReceiveClick()
    {
        if (!string.IsNullOrEmpty(intervalInputField.text))
        {
            receiveButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);

            clientMessage = "Int:" + pointController.GetIndexOfLastPoint() + "," + intervalInputField.text;
            SendMessage();

            DisplayInfo("Request to the Server sent, I'm getting my points...", Color.yellow);
        }
        else
        {
            DisplayInfo("Enter an interval value!", Color.red);
        }
    }

    // Quando viene cliccato il bottone "Stop", al Server verrà inviato un messaggio che gli indicherà di smettere di inviare punti
    public void OnStopClick()
    {
        stopButton.gameObject.SetActive(false);
        receiveButton.gameObject.SetActive(true);

        if (receivedAllPoints)
        {
            receivedAllPoints = false;
            DisplayInfo("I have stopped receiving points", Color.green);
        }
        else
        {
            clientMessage = "Msg:Stop";
            SendMessage();

            DisplayInfo("STOP command sent to the Server", Color.green);
        }
    }

    // Quando viene chiusa l'applicazione, viene invocato il metodo ConnectionInterrupted()
    private void OnApplicationQuit()
    {
        ConnectionInterrupted();
    }
}
