//#define ARDUINO_USB_MODE 2
//#define SOC_USB_OTG_SUPPORTED

#include "USB.h"
#include "USBHIDKeyboard.h"
#include "USBHIDMouse.h"
USBHIDKeyboard Keyboard;
USBHIDAbsoluteMouse Mouse;
//USBHIDRelativeMouse Mouse;
int mx = 0;
int my = 0;
#define RESET_BUTTON_PIN 17
#define POWER_BUTTON_PIN 18

void setup() {
  if (RESET_BUTTON_PIN) {
    pinMode(RESET_BUTTON_PIN, OUTPUT);
    digitalWrite(RESET_BUTTON_PIN, 0);
  }
  if (POWER_BUTTON_PIN) {
    pinMode(POWER_BUTTON_PIN, OUTPUT);
    digitalWrite(POWER_BUTTON_PIN, 0);
  }

  Serial.begin(115200);
  Keyboard.begin();
  Mouse.begin();
  USB.begin();

  delay(1000);
  Serial.write("Started!");
}

void loop() {
    if (Serial.available() > 0) {
        String line = Serial.readStringUntil('\n');
        Serial.write(line.c_str());
        Serial.write("\n");
        if (line.endsWith("\r")) {
            line = line.substring(0, line.length() - 1);
        }

        if (line.startsWith("KD:")) {
            uint8_t keyCode = (uint8_t)line.substring(3).toInt();
            Keyboard.pressRaw(keyCode);
        }
        else if (line.startsWith("KU:")) {
            uint8_t keyCode = (uint8_t)line.substring(3).toInt();
            Keyboard.releaseRaw(keyCode);
        } 
        else if (line.startsWith("KP:")) {
          String params = line.substring(3);
          int pos = params.indexOf(',');
          if (pos >= 0) {
            uint8_t keyCode = (uint8_t)params.substring(0, pos).toInt();
            int Delay = params.substring(pos + 1).toInt();
            Keyboard.pressRaw(keyCode);
            delay(Delay);
            Keyboard.releaseRaw(keyCode);
          }
        } 
        else if (line.startsWith("MW:")) {
            char pos = (char)line.substring(3).toInt();
            Mouse.move(mx, my, pos);
        }
        else if (line.startsWith("MM:")) {
            String coords = line.substring(3);
            int pos = coords.indexOf(',');
            if (pos >= 0) {
                mx = coords.substring(0, pos).toInt();
                my = coords.substring(pos + 1).toInt();
                Mouse.move(mx, my);
            }
        }
        else if (line.startsWith("MD:")) {
            String coords = line.substring(3);
            int pos = coords.indexOf(',');
            if (pos >= 0) {
                int button = coords.substring(0, pos).toInt();
                coords = coords.substring(pos + 1);
                int pos = coords.indexOf(',');

                if (pos >= 0) {
                    mx = coords.substring(0, pos).toInt();
                    my = coords.substring(pos + 1).toInt();
                    Mouse.move(mx, my);
                    Mouse.press(button);
                }
            }
        } else if (line.startsWith("MU:")) {
            String coords = line.substring(3);
            int pos = coords.indexOf(',');
            if (pos >= 0) {
                int button = coords.substring(0, pos).toInt();
                coords = coords.substring(pos + 1);
                int pos = coords.indexOf(',');
                if (pos >= 0) {
                    int x = coords.substring(0, pos).toInt();
                    int y = coords.substring(pos + 1).toInt();
                    Mouse.move(x, y);
                    Mouse.release(button);
                }
            }
        } else if (line.startsWith("RB:")) {
          if (RESET_BUTTON_PIN) {
            digitalWrite(RESET_BUTTON_PIN, 1);
            delay(250);
            digitalWrite(RESET_BUTTON_PIN, 0);
          }
        } else if (line.startsWith("PW:")) {
          if (POWER_BUTTON_PIN) {
            digitalWrite(POWER_BUTTON_PIN, 1);
            delay(250);
            digitalWrite(POWER_BUTTON_PIN, 0);
          }
        }
    }
}

