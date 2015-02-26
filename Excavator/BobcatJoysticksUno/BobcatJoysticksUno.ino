
int sensorPinA = A0;    // select the input pin for the potentiometer
int sensorPinB = A1;    // select the input pin for the potentiometer
int sensorPinC = A2;    // select the input pin for the potentiometer
int sensorPinD = A3;    // select the input pin for the potentiometer
 
short sensorValueA = 0;  // variable to store the value coming from the sensor
short sensorValueB = 0;  // variable to store the value coming from the sensor
short sensorValueC = 0;  // variable to store the value coming from the sensor
short sensorValueD = 0;  // variable to store the value coming from the sensor

void setup()
{ 
  pinMode (sensorPinA, INPUT);
  pinMode (sensorPinB, INPUT);  
  pinMode (sensorPinC, INPUT);
  pinMode (sensorPinD, INPUT);  

  Serial.begin (57600);  
} 

void loop()
{ 
  sensorValueA = analogRead(sensorPinA);
  Serial.write(lowByte(sensorValueA));
  Serial.write(highByte(sensorValueA));
  
//  delay(1);

  sensorValueB = analogRead(sensorPinB);
  Serial.write(lowByte(sensorValueB));
  Serial.write(highByte(sensorValueB));

//  delay(1);

  sensorValueC = analogRead(sensorPinC);
  Serial.write(lowByte(sensorValueC));
  Serial.write(highByte(sensorValueC));

//  delay(1);

  sensorValueD = analogRead(sensorPinD);
  Serial.write(lowByte(sensorValueD));
  Serial.write(highByte(sensorValueD));

//  delay(1);
  
  Serial.write(255);
  Serial.write(127);  
}


