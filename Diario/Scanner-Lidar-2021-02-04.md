# [Scanner_Lidar] | Diario di lavoro
##### [Matteo Lupica, Daniele Cereghetti, Veljko Markovic, Isaac Gragasin]
### [Centro Professionale Trevano], [04.02.2021]

## Lavori svolti

|Orario        |Lavoro svolto                 |Persone        |
|--------------|------------------------------|---------------|
|08:20 - 10:15 |Installazione di Unity        |Veljko         |
|10:15 - 13:50 |Movimento libero della camera |Veljko         |
|13:50 - 15:30 |Prova rendering punti Random  |Veljko         |
|15:30 - 15:35 |Sistemazione della luce       |Veljko         |
|08:20 - 11:35 |Studio step motor             |Isaac & Daniele|
|12:30 - 15:00 |Gestione motori               |Isaac & Daniele|
|15:00 - 15:30 |Traduzione gradi in step      |Isaac & Daniele|
|08:20 - 11:35 |Utilizzo di NLog e logger     |Matteo         |
|12:30 - 15:30 |Lettura dati dalla seriale    |Matteo         |

Veljko:
Unity non si installa sulla macchina da scuola. Mi sono messo a cercare un setup offline. Ho trovato un possibile modo per scaricare Unity offline senza Hub, tramite il Unity Download Assistant.
L'apertura del Unity Download Assistant mi viene bloccata. Ho provato a fare partire Unity Download Assistant dalla macchina virtuale. Il download è partito. Sta venendo installato Unity 2020.2.3f1 (64-bit). Mi ha chiesto di inserire una licenza valida quando creo un nuovo progetto. Ho dovuto attivare la licenza manualmente, cliccando su Manual License. Procedura terminata. Licenza valida, Unity operativo.
Ho creato un nuovo progetto Unity (Scanner_Lidar). Ho iniziato a creare il movimento libero della camera. Ho implementato il movimento "Avanti", "Dietro", "Destra" e "Sinistra" tramite con i tasti "WASD".
Ho interrotto il movimento per iniziare a renderizzare i primi punti. Ho creato una prova che aggiunge dei punti a caso nello spazio premendo "UpArrow", e li rimuove premendo "DownArrow".
Ho creato un nuovo materiale nero, chiamato "Environment", successivamente su "Window -> Rendering -> Lightning -> Environment -> Skybox Material" ho impostato il materiale "Environment".
Ho inserito una "Point Light" al posto di una "Directional Light".

Daniele & Isaac:
Abbiamo parlato con il docente riguardo alla struttura Hardware, e ci ha esortati a non usare ingranaggi in quanto coinvolgono il concetto di gioco e la rotazione in gradi. Abbiamo deciso di concentrarci
sull'aspetto di programmazione e studio dei motori. Abbiamo avuto problemi riguardo la polarità dei motori. Abbiamo ricontrato problemi nella traduzione da gradi a step del motore.

Matteo:
Ho cambiato libreria di log (da Serilog a NLog) e configurazione di NLog. Ho letto i dati sulla seriale (Mockdata).

##  Problemi riscontrati e soluzioni adottate
Soluzione installazione Unity --> Installazione di Unity Download Assistant su VM.

Soluzione polarità motori --> Abbiamo inverito due cavi.

Soluzione da gradi a step del motore --> Abbiamo trovato una proporzione, ma con del margine di errore da risolvere (la prossima giornata di lavoro)

##  Punto della situazione rispetto alla pianificazione
Siamo in pari alla pianificazione

## Programma di massima per la prossima giornata di lavoro
Rimuovere le ombre dai pallini, riuscire ad ottenere i punti da renderizzare e renderizzarli, gestione motori e formula, rifare la struttura hardware
