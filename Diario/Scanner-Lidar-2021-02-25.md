# [Scanner_Lidar] | Diario di lavoro
##### [Matteo Lupica, Daniele Cereghetti, Veljko Markovic, Isaac Gragasin]
### [Centro Professionale Trevano], [25.02.2021]

## Lavori svolti

|Orario        |Lavoro svolto                                           |Persone         |
|--------------|--------------------------------------------------------|----------------|
|08:20 - 15:45 |Implementazione Server e Client                         |Veljko & Matteo |
|08:20 - 09:00 |Configurazione Scanner LIDAR                            |Isaac & Daniele |
|09:00 - 09:50 |Implementazione codice Scanner con codice Stepper Motor |Isaac & Daniele |
|10:05 - 13:20 |Commento codice Scanner LIDAR                           |Isaac           |
|10:05 - 15:45 |Design struttura hardware                               |Isaac & Daniele |


Veljko (e Matteo):
Ho iniziato a creare uno script che fa da client. Ho torvato un esempio di client che riceve dati da un server tramite TCP, il link è:
https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb#file-tcptestclient-cs-L26

Oltre ad adattare il codice alle mie esigenze, ho analizzato e commentato tutte le istruzioni per capirne il funzionamento. Leggendo il codice ho scoperto il concetto di Background Thread e Foreground Thread, la definizione l'ho trovato sulla documentazione di Microsoft, e dice:
"A managed thread is either a background thread or a foreground thread. Background threads are identical to foreground threads with one exception: a background thread does not keep the managed execution environment running. Once all foreground threads have been stopped in a managed process (where the .exe file is a managed assembly), the system stops all background threads and shuts down."

Una volta terminata di scrivere la lettura da parte del Client, abbiamo provato ad effettuare un test, solo che quando il Client prova ad effettuare una connessione con il Server, appare l'eccezione:
"Eccezione da parte del Socket: System.Net.Sockets.SocketException (0x80004005): Impossibile stabilire la connessione. Rifiuto persistente del computer di destinazione."

È un problema di Firewall, vedi la soluzione nella sezione sotto.

Il Client e il Server adesso comunicano, ma il Server non è completato, abbiamo deciso di rifarlo da zero utilizzando TCPListener (siccome per il Client abbiamo utilizzato TCPClient). Abbiamo scelto di dare l'indirizzo "0.0.0.0" al Server, così possiamo utilizzare tutti i suoi indirizzi IP e il Server non si preoccupa di selezionarne uno fisso. Successivamente abbiamo creato dei bottoni che dicono al Server "okay, puoi iniziare a mandare dati", oppure "okay, smetti di inviare dati".

##  Problemi riscontrati e soluzioni adottate
Soluzione Firewall --> Abbiamo disattivato il Firewall, ma idealmente sarebbe meglio aprire semplicemente la porta.

##  Punto della situazione rispetto alla pianificazione
Siamo in anticipo rispetto al Gantt preventivo.

## Programma di massima per la prossima giornata di lavoro
Realizzare la struttura hardware con Easel per definire finalmente le componenti da utilizzare. Terminare il Server, avere una GUI funzionante e rendering passaggio dei punti da renderizzare al Client.