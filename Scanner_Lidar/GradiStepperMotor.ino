//step x revolution --> 64
// velocità --> 300
// step --> 510
// gradi che fa --> 90°
#include <Stepper.h>
 
const int stepsPerRevolution = 64 ;
Stepper myStepperX(stepsPerRevolution, 11,9,10,8);
Stepper myStepperY(stepsPerRevolution, 6,4,5,3);

const int degreesX = 6;
const int degreesY = 6;
const double ratio = 17/3;

int pos = 0;
bool clockwise = true;
int intervalX = 0;
int intervalY = 0;
int stepX = ratio * degreesX;
int stepY = ratio * degreesY;

void setup() {
  Serial.begin(9600);
  myStepperX.setSpeed(300);
  myStepperY.setSpeed(300);
}
 
void loop() {
  
  intervalX = 0;
  while(intervalX * degreesX < 360){
    if(clockwise){
      myStepperX.step(stepX);
      
    }else{
      myStepperX.step(-stepX);
    }
    Serial.println(intervalX * degreesX);
    intervalX++;
    delay(1000);
  }

  myStepperY.step(stepY);
  intervalY++;
  clockwise = !clockwise;
  
  /*if(intervalY * degreesY < 90){
    clockwise = true;
    //myStepperX.step(-510 * stepX);
    myStepperY.step(-stepY * intervalY);
    return;
  }*/



  

  

 
}
