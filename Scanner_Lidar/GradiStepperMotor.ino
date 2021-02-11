#include <Stepper.h>
 
const int stepsPerRevolution = 64 ;
Stepper myStepperX(stepsPerRevolution, 11,9,10,8);
Stepper myStepperY(stepsPerRevolution, 6,4,5,3);

const int degreesX = 6;
const int degreesY = 6;
const double ratio = 5.625*1.0;

int pos = 0;
bool clockwise = true;
int intervalX = 0;
int intervalY = 0;
int stepX = ratio * degreesX;
int stepY = ratio * degreesY;

boolean finish = false;

void setup() {
  Serial.begin(9600);
  myStepperX.setSpeed(300);
  myStepperY.setSpeed(300);
}
 
void loop() {
  if(!finish){
    intervalX = 0;
    while(intervalX * degreesX <= 360 + degreesX){
      Serial.println(intervalX * degreesX);
      if(clockwise){
        myStepperX.step(stepX);
        
      }else{
        myStepperX.step(-stepX);
      }
      intervalX++;
    }
  
    myStepperY.step(stepY);
    intervalY++;
    clockwise = !clockwise;
    
    if(intervalY * degreesY > 90){
      myStepperY.step(-stepY * intervalY);
      finish = true;
    }
  }else{
    delay(1);
  }
  



  

  

 
}
