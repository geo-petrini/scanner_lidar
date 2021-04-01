# [Scanner_Lidar] | Diario di lavoro
##### [Matteo Lupica, Daniele Cereghetti, Veljko Markovic, Isaac Gragasin]
### [Centro Professionale Trevano], [01.04.2021]

## Lavori svolti

|Orario        |Lavoro svolto                                               |Persone         |
|--------------|------------------------------------------------------------|----------------|
|08:20 - 09:00 |Test progetto complessivo (gordo)                           |Tutti           |
|09:00 - 09:50 |Aggiunta degli assi                                         |Veljko          |
|10:05 - 14:45 |Aggiunta del Mouse Look                                     |Veljko          |
|08:20 - 11:30 |Bug fix, gestione trasporto dat Arduino --> Client          |Matteo          |
|12:30 - 14:00 |Reso il server Multi-Client con in aggiunta una pagina web  |Matteo          |
|14:15 - 15:30 |Gestione buffer stream della seriale                        |Matteo          |
|08:20 - 10:50 |Test rotazioni della struttura HW                           |Isaac & Daniele |
|11:00 - 14:00 |Miglioramento operazione di scannerizzazione                |Isaac & Daniele |
|14:15 - 15:00 |Test e correzioni minori                                    |Tutti           |

Veljko:
Abbiamo iniziato effettuando dei test. Succesivamente io ho aggiunto gli assi, in modo tale da poterci orientare un minimo. Ho trovato la soluzione a questo link:
https://docs.unity3d.com/ScriptReference/GL.html

Successivamente ho iniziato ad implementare il Mouse Look. Ho trovato uno script d'aiuto su questo link:
https://answers.unity.com/questions/1344322/free-mouse-rotating-camera.html

##  Problemi riscontrati e soluzioni adottate
Problemi di rotazione: abbiamo risolto il fatto che mancava una rotazione, ma abbiamo notato che il buco dove si trova il perno è usurato -> abbiamo chiesto una nuova base che sarà realizzata dal professore in plexiglas, materiale più resistente.
Nuovo metodo di presa dati: per prendere dei dati in modo più significativo, abbiamo dovuto fare delle modifiche al motore orizzontale, però si sfasava -> abbiamo messo su lavagna il problema trovato e siamo riusciti ad intuire come rimediare all’errore.


##  Punto della situazione rispetto alla pianificazione
Siamo in anticipo rispetto al Gantt preventivo.

## Programma di massima per la prossima giornata di lavoro
Gestione StreamBuffer più lettura dati Adruino. Pulizia codice software. Documentazione, supporto morale, riflessione su possibili miglioramenti o aggiunte.