# [Scanner_Lidar] | Diario di lavoro
##### [Matteo Lupica, Daniele Cereghetti, Veljko Markovic, Isaac Gragasin]
### [Centro Professionale Trevano], [15.04.2021]

## Lavori svolti

|Orario        |Lavoro svolto                                       |Persone                  |
|--------------|----------------------------------------------------|-------------------------|
|08:20 - 15:00 |Documentazione                                      |Veljko & Isaac & Daniele |
|08:20 - 09:50 |Gestione Multi-Threading                            |Matteo                   |
|10:05 - 15:00 |Perfezionamento Log                                 |Matteo                   |
|15:00 - 15:30 |Consultazione su qual è il giusto calcolo dei punti |Tutti                    |

Veljko:
Abbiamo iniziato testando se il Server riesce a gestire il Multi-Threading, e abbiamo notato che se un Client invia al Server il comando Stop, il risultato viene propagato su tutti i Client. Matteo si occuperà di gestire questa cosa, mentre successivamente io, Daniele e Isaac abbiamo continuato la documentazione.

##  Problemi riscontrati e soluzioni adottate
Problemi di calcolo dei punti: abbiamo notato che i punti ricevuti non erano realistici, e questo è dovuto ad una gestione scorretta degli angoli, non del calcolo dei punti.


##  Punto della situazione rispetto alla pianificazione
Siamo in anticipo rispetto al Gantt preventivo.

## Programma di massima per la prossima giornata di lavoro
Ottenimento corretto dei punti da renderizzare.