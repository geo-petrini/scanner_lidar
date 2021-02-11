# [Scanner_Lidar] | Diario di lavoro
##### [Matteo Lupica, Daniele Cereghetti, Veljko Markovic, Isaac Gragasin]
### [Centro Professionale Trevano], [11.02.2021]

## Lavori svolti

|Orario        |Lavoro svolto                           |Persone        |
|--------------|----------------------------------------|---------------|
|08:20 - 08:50 |Setup live share                        |Veljko & Matteo|
|08:50 - 09:30 |Calibrazione luci                       |Veljko         |
|09:30 - 09:50 |Discussione con Matteo                  |Veljko & Matteo|
|10:05 - 10:35 |Configurazione di NLog Unity            |Veljko & Matteo|
|10:35 - 11:20 |Risoluzione System.IO.Ports             |Veljko & Matteo|
|11:20 - 14:15 |Script trasferimento di dati            |Veljko & Matteo|
|14:15 - 15:30 |Creazione server e richieste            |Veljko & Matteo|
|08:20 - 09:50 |Ottimizzazione rapporto stepper motor   |Isaac & Daniele|
|10:05 - 13:30 |Riprogettazione struttura hardware base |Isaac & Daniele|
|13:30 - 15:30 |Annotazione dimensioni delle componenti |Isaac & Daniele|

Veljko (e Matteo):
Io e Matteo abbiamo provato ad installare un'estensione per la condivisione del progetto con Visual Studio 2019 (Microsoft.VisualStudio.LiveShare). Una volta installata l'estensione, in alto a destra appare il
tasto "Live Share", ho effettuato l'accesso con il mio account di GitHub, ho inviato il link per lo share a Matteo e siamo riusciti a rendere il lavoro condiviso. Successivamente, ho inserito più di una Point Light,
più precisamente, ne ho inserite 7: Center, Up, Down, Left, Right, Foreward, Backward. Ho dato ad ognuna colore bianco e range 200, tutte distano di 50 a dal centro. Successivamente abbiamo discusso riguardo a che formato usare per i dati dei punti da renderizzare, e abbiamo concordato che verrà passata una lista di Vector3. Ho iniziazo a collaborare con Matteo per finire di configurare NLog. Abbiamo ricontrato un problema con Unity e i pacchetti NuGet, abbiamo deciso di risolvere in seguito la faccenda NLog e di iniziare a fare il trasferimento dei dati. Abbiamo ricontrato un problema con la SerialPort, l'output ottenuto era:

"Type or Namespace 'Ports' does not exist in System.IO"
La soluzione si trova nella sezione sotto.


Unity continua a dare messaggio di errore di compilazione.
La soluzione si trova nella sezione sotto.


Dopo aver fatto finito uno script di prova che prende i dati del SerialPort e dopo aver lanciato l'applicazione, Unity ci ha dato il seguente errore:

PlatformNotSupportedException: System.IO.Ports is currently only supported on Windows.
System.IO.Ports.SerialPort..ctor () (at <9eedbed1e64b4c2d97edd8d4a1e07964>:0)
(wrapper remoting-invoke-with-check) System.IO.Ports.SerialPort..ctor()

Sotto spunto del docente, abbiamo deciso di creare un Server asincrono, che si occuperà di distribuire i dati del Lidar a chiunque faccia una richiesta HTTP. Perciò, ho rimosso dal progetto Unity il System.IO.Ports e lo script per lo trasferimento dei dati. Matteo ha inizato a creare il server, io mi sto documentando su come fare delle richieste HTTP in Unity.

Daniele & Isaac:
Mentre stavamo cercando di eliminare il margine di errore prodotto dal rapporto che utilizzavamo per la rotazione orizzontale abbiamo scoperto che le frazioni che calcolavamo venivano arrotondati per eccesso alla nostra insaputa.

##  Problemi riscontrati e soluzioni adottate
Soluzione SerialPort --> In Unity, sotto "Edit > ProjectSettings > Player > ApiCompatibilityLevel" ho cambiato da ".NET 4.X" a ".NET Standard 2.0"
Soluzione Unity & compilazione --> Abbiamo installato la DLL di System.IO.Ports e l'abbiamo messa in "Assets/Plugins"
Soluzione rapporto rotazioni --> Invece di utilizzare un calcolo per determinare il rapporto l’abbiamo imposto con valore 5.625 * 1.0, che è il rapporto tra una rivoluzione completa del motore stepper e i passi necessari per compierla.

##  Punto della situazione rispetto alla pianificazione
Siamo in anticipo rispetto al Gantt preventivo, però abbiamo riscontrato diversi problemi che ci hanno rallentati, ma stiamo comunque procedendo bene.

## Programma di massima per la prossima giornata di lavoro
Avere il server funzionante, avere lo script per le richieste al server, rendering dei punti avendo i dati ricevuti dal server. Completamento dell'annotazione delle dimensioni dei componenti e iniziare a lavorare sulla configurazione dello scanner LIDAR stesso.
