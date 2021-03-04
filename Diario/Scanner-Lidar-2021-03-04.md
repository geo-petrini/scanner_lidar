# [Scanner_Lidar] | Diario di lavoro
##### [Matteo Lupica, Daniele Cereghetti, Veljko Markovic, Isaac Gragasin]
### [Centro Professionale Trevano], [04.03.2021]

## Lavori svolti

|Orario        |Lavoro svolto                                           |Persone         |
|--------------|--------------------------------------------------------|----------------|
|08:20 - 15:45 |Implementazione Server e Client                         |Veljko & Matteo |
|08:20 - 11:35 |Riproduzione componenti della struttura HW su Easel     |Isaac & Daniele |
|13:30 - 14:00 |Ripasso di qualità schema elettrico                     |Isaac & Daniele |
|14:00 - 15:45 |Protocollo test inoltro json                            |Isaac & Daniele |

Veljko (e Matteo):
Ho iniziato gestendo alcune eccezioni che potevano essere genereate, come una IOException quando il Server si chiudeva inaspettatamente, mentre lo Stream e la connessione erano ancora attive. Ho anche gestito la corretta chiusura dell'applicazione (se ci sono Thread vive, le interrompe e sia lo Stream che la connessione con il Server vengono chiuse). Successivamente mi sono occupato di riordinare l'interfaccia grafica. Ho creato un bottone che mi permette di connettermi al Server. Abbiamo discusso con il docente, e abbiamo stabilito che il Client sceglierà da che punto richiedere i punti, e poi li renderizza. Ho creato un bottone che permette di disconnettersi dal Server. Ho messo l'applicazione in windowed mode. Successivamente abbiamo stabilito un protocollo, i dati che vanno e vengono saranno in formato JSON.

##  Problemi riscontrati e soluzioni adottate

##  Punto della situazione rispetto alla pianificazione
Siamo in anticipo rispetto al Gantt preventivo.

## Programma di massima per la prossima giornata di lavoro
Costruire la struttura HW e completare il protocollo test per json. Risolvere dei bug relativi alla GUI e implementare effettivamente il protocollo di raccolta/invio di dati.