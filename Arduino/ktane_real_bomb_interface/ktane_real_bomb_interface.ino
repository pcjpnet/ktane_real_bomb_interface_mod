
#include "DmxSimple.h"

volatile byte km_state = 0;

void dmx_off() {
  // AMERICAN DJ : Dotz Par (9ch mode @ 1ch)
  DmxSimple.write(1, 0);  // Red
  DmxSimple.write(2, 0);  // Green
  DmxSimple.write(3, 0);  // Blue
  DmxSimple.write(4, 0);  // Master
  DmxSimple.write(8, 0);  // Strobe
}

void dmx_red() {
  // AMERICAN DJ : Dotz Par (9ch mode @ 1ch)
  DmxSimple.write(1, 255);  // Red
  DmxSimple.write(2, 0);  // Green
  DmxSimple.write(3, 0);  // Blue
  DmxSimple.write(4, 255);  // Master
  DmxSimple.write(8, 0);  // Strobe
}

void dmx_white() {
  // AMERICAN DJ : Dotz Par (9ch mode @ 1ch)
  DmxSimple.write(1, 255);  // Red
  DmxSimple.write(2, 255);  // Green
  DmxSimple.write(3, 255);  // Blue
  DmxSimple.write(4, 255);  // Master
  DmxSimple.write(8, 0);  // Strobe
}

void dmx_white_strobe() {
  // AMERICAN DJ : Dotz Par (9ch mode @ 1ch)
  DmxSimple.write(1, 255);  // Red
  DmxSimple.write(2, 255);  // Green
  DmxSimple.write(3, 255);  // Blue
  DmxSimple.write(4, 255);  // Master
  DmxSimple.write(8, 180);  // Strobe
}

void dmx_red_strobe() {
  // AMERICAN DJ : Dotz Par (9ch mode @ 1ch)
  DmxSimple.write(1, 255);  // Red
  DmxSimple.write(2, 0);  // Green
  DmxSimple.write(3, 0);  // Blue
  DmxSimple.write(4, 255);  // Master
  DmxSimple.write(8, 180);  // Strobe
}

void dmx_white_strobe_slow() {
  // AMERICAN DJ : Dotz Par (9ch mode @ 1ch)
  DmxSimple.write(1, 255);  // Red
  DmxSimple.write(2, 255);  // Green
  DmxSimple.write(3, 255);  // Blue
  DmxSimple.write(4, 255);  // Master
  DmxSimple.write(8, 50);  // Strobe
}

void serial_read_line(char buf[], int n) {
  int i = 0;
  int input;
  while(1){
    if (Serial.available() > 0) {
      input = Serial.read();
      if ( input == -1 ) continue;
      if ( input == int('\n') ) {
        buf[i] = '\0';
        return;
      }
      buf[i] = input;
      i++;
      if ( i >= n-1 ) {
        buf[n-1] = '\0';
        return;
      }
    }
  }
}

void setup() {
  
  pinMode(2, OUTPUT); // DE
  pinMode(4, OUTPUT); // TX-io
  pinMode(6, OUTPUT); // ALARM
  pinMode(7, OUTPUT); // BOMB
  digitalWrite(2, HIGH);  // DE
  digitalWrite(6, LOW);  // ALARM
  digitalWrite(7, LOW);  // BOMB
  
  DmxSimple.usePin(4);
  DmxSimple.maxChannel(9);
  
  Serial.begin(9600);
}

void loop() {
  
  char str[11];
  serial_read_line(str, 11);
  if (!strcmp(str, "!KM-STATE0")) km_state = 0;
  else if (!strcmp(str, "!KM-STATE1")) {km_state = 1; digitalWrite(7, LOW); dmx_off();}
  else if (!strcmp(str, "!KM-STATE2")) {km_state = 2;}
  else if (!strcmp(str, "!KM-STATE3")) {km_state = 3;}
  else if (!strcmp(str, "!KM-STATE4")) {km_state = 4;}
  else if (!strcmp(str, "!LIGHTTRUE")) dmx_white();
  else if (!strcmp(str, "!LIGHTFALS")) {digitalWrite(7, LOW); dmx_off();}
  else if (!strcmp(str, "!ALARMTRUE")) digitalWrite(6, HIGH);
  else if (!strcmp(str, "!ALARMFALS")) digitalWrite(6, LOW);
  else if (!strcmp(str, "!BOMBEXPLD")) {digitalWrite(7, HIGH); dmx_red_strobe();}
  else if (!strcmp(str, "!BOMBSOLVD")) {digitalWrite(7, LOW); dmx_white_strobe_slow();}
  
}



