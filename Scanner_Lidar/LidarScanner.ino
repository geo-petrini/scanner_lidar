// Inclusione delle librerie necessarie per operare i Stepper Motor e lo scanner LIDAR.
#include <Stepper.h>
#include "TFmini.h"
#include <ArduinoJson.h>

const int stepsPerRevolution = 64 ;  // Il numero di step che i Stepper Motor eseguono per compiere una rivoluzione completa.
Stepper myStepperX(stepsPerRevolution, 11,9,10,8); // Definizione dello Stepper Motor orizzontale utilizzato dal programma.
Stepper myStepperY(stepsPerRevolution, 6,4,5,3); // Definizione dello Stepper Motor verticale utilizzato dal programma.
TFmini tfmini; // Definizione dello scanner LIDAR utilizzato dal programma.

const int degreesX = 6; // Il numero di gradi eseguiti dal Stepper Motor dell'asse X, per step.
const int degreesY = 6; // Il numero di gradi eseguiti dal Stepper Motor dell'asse Y, per step.
const double ratio = 5.625*1.0; // Il rapporto tra 360° e stepsPerRevolution (hardcoded per eccesso di approssimazione da parte di Arduino).

bool clockwise = true; // Lo stato che stabilisce il senso in cui ruota lo Stepper Motor dell'asse X (true = senso orario; false = senso antiorario).
int intervalX = 0; // Il numero di rotazione attuale dello Stepper Motor dell'asse X.
int intervalY = 0; // Il numero di rotazione attuale dello Stepper Motor dell'asse Y.
int stepX = ratio * degreesX; // Il numero di gradi eseguiti dal Stepper Motor dell'asse X, per step, seguendo il rapporto associato al motore relativo.
int stepY = ratio * degreesY; // Il numero di gradi eseguiti dal Stepper Motor dell'asse Y, per step, seguendo il rapporto associato al motore relativo.

bool finish = false; // Lo stato che stabilisce il termine di una scansione (true = scansione terminata; false = scansione non terminata)
bool send = false;
void setup() {
  
  // Settaggio seriale Arduino - PC; Arduino - TFMini.
  Serial.begin(115200);
  Serial1.begin(TFmini::DEFAULT_BAUDRATE);
  tfmini.attach(Serial1);

  // Settaggio velocità dei motori.
  myStepperX.setSpeed(300);
  myStepperY.setSpeed(300);
}
 
void loop() {
  if(send){
      if(!finish){
  
      // Esecuzione delle rotazioni dei motori ed estrapolazione delle distanze prese dal LIDAR.
  
  
      intervalX = 0; // Inizializzazione del numero di rotazione.
      
      // Se lo stato del verso dello Stepper Motor stabilisce che è orario, gli step sono incrementali, altrimenti decrementali.
      if(clockwise){
          // Esecuzione delle rotazione sull'asse X finché il numero di rotazione utilizzato permetterebbe allo step fatto di eccedere 360° gradi.
          while(intervalX * degreesX <= 360 + degreesX){
            myStepperX.step(stepX);
            // Controlla se lo scanner LIDAR esiste ed è accessibile; 
            // in seguito vengono inoltrati gli angoli di rotazioni sugli assi, e la distanza ricavata dallo scanner.
            if (tfmini.available()){
                //sendJson(tfmini.getDistance());
                Serial.print(intervalX * degreesX);
                Serial.print(",");
                Serial.print(intervalY * degreesY);
                Serial.print(",");
                Serial.print(tfmini.getDistance());
            }
            intervalX++; // Incremento numero rotazione orizzontale attuale.
          }
        }else{
          myStepperX.step(-ratio*(degreesX/2)); // Mezzo step per sfasare righe pari sull'asse orizziontale.
          while(intervalX * degreesX <= 360){ //  Deve terminare una rotazione prima.
            myStepperX.step(-stepX);
            if (tfmini.available()){
                //sendJson(tfmini.getDistance());
                Serial.print(intervalX * degreesX);
                Serial.print(",");
                Serial.print(intervalY * degreesY);
                Serial.print(",");
                Serial.print(tfmini.getDistance());
            }
            intervalX++;
          }
          myStepperX.step(-ratio*(degreesX/2)); // Mezzo step per ri-allineamento per righe dispari sull'asse orizzontale
        }
    
  
      // Dopo aver completato un giro 360° sull'asse X, viene fatto uno step da parte dello Stepper Motor dell'asse Y,
      // incrementa il numero di rotazione verticale attuale, inverte il verso dello Stepper Motor dell'asse X.
      myStepperY.step(stepY);
      intervalY++;
      clockwise = !clockwise;
      
  
      // Se il nuovo numero di rotazione causa l'eccesso dei 90° da parte del prossimo step,
      // viene fatto il reset del Step Motor dell'asse Y 
      // e viene settato lo stato della scansione come terminata.
  
      if(intervalY * degreesY > 90){
        myStepperY.step(-stepY * intervalY);
        finish = true;
      }
    }else{
      delay(10);
    }
  }else{
    if(Serial.readString() == "OK"){
      send = true;
    }else{
      Serial.write("CIAO");
    }
  }
}


//Uso con JSON (opzionale)
void sendJson(int dist) {
  DynamicJsonDocument data(200);
  data["point"]["x"] = intervalX * degreesX;
  data["point"]["y"]   = intervalY * degreesY;
  data["point"]["distance"] = dist;
  
  serializeJson(data, Serial);
}
