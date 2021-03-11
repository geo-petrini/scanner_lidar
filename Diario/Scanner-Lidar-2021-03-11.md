# [Scanner_Lidar] | Diario di lavoro
##### [Matteo Lupica, Daniele Cereghetti, Veljko Markovic, Isaac Gragasin]
### [Centro Professionale Trevano], [11.03.2021]

## Lavori svolti

|Orario        |Lavoro svolto                                           |Persone         |
|--------------|--------------------------------------------------------|----------------|
|08:20 - 15:30 |Assemblaggio struttura per Lidar                        |Isaac & Daniele |
|08:20 - 15:30 |Ridefinizione e risoluzione della GUI                   |Veljko          |
|08:20 - 15:30 |Salvataggio dei punti                                   |Matteo          |

Veljko (e Matteo):
Ho iniziando pulendo il mio codice e sistemando la GUI. Siccome Unity non permette di modificare gli elementi della GUI al di fuori dalla Main Thread, ho dovuto riadattare il codice per potere manipolare gli elementi della GUI dalla Main Thread. Succesivamente ho creato un InputField (Content Type l'ho settato su Integer Number) per l'intervallo di punti richiesti alla volta da parte del Client. Poi, abbiamo iniziato ad effettuare le richieste del Server, non ancora implementando l'effettiva ricevuta dei dati.

##  Problemi riscontrati e soluzioni adottate
Abbiamo riscontrato un problema con il file JSon, il quale era poco performante per l'aggiunta di nuovi elementi, siccome ogni volta bisognava leggere il file, eliminare l'ultimo carattere ('}'), aggiungere l'elemento e rimettere l'ultimo carattere ('}').
Soluzione --> per ovviare a questo problema abbiamo optato per l'utilizzo di SQLite per il salvataggio dei dati.

Abbiamo trovato due misure fatte male dei motori.
Soluzione --> abbiamo dovuto allargare due buchi con il trapano

Si è scaricato il trapano.
Soluzione --> abbiamo atteso che si caricasse

##  Punto della situazione rispetto alla pianificazione
Siamo in anticipo rispetto al Gantt preventivo.

## Programma di massima per la prossima giornata di lavoro
Finire l'assemblaggio HW. Integrazione delle varie parti del progetto (se possibile). Lettura di punti da parte del Client e inizio Rendering dei punti.