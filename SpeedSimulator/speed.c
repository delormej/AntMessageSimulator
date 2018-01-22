//
// Bitbang mode on the FTDI USB to Serial Chipset.
// Reference: https://hackaday.com/2009/09/22/introduction-to-ftdi-bitbang-mode/ 
// Controls AD9850 Module DDS Signal Generator V2
// Reference: http://www.electrodragon.com/w/AD9850_Module_DDS_Signal_Generator_V2 
//

#include <stdint.h>
#include <stdbool.h>
#include <float.h>
#include <string.h>
#include <stdio.h>
#include <ftd2xx.h>

#define W_CLK 0x08  /* CTS (brown wire on FTDI cable) */
#define FQ_UD 0x01  /* TX  (orange) */
#define DATA  0x02  /* RX  (yellow) */
#define RESET 0x14  /* RTS (green on FTDI) + DTR (on SparkFun breakout) */

#define WORD_LENGTH 40

void writePin(int pin, bool high) {
    static char bit = 0;
    if (pin == DATA)
        printf("bit:%i == %i\r\n", bit++, high);
}

void pulseHigh(int pin) {
    writePin(pin, true);
    writePin(pin, false);
}

void writeRegister(int frequency) {
    char word[5] = {0xAA, 0xAA, 0xAA, 0xAA, 0xFF};
    for (int i = 0; i < WORD_LENGTH; i++) {
        char byte = word[i/8];
        writePin(DATA, byte & (1 << i%8));
        pulseHigh(W_CLK);
    }
    pulseHigh(FQ_UD);
}

void powerDown() {
    // Writes bit 34 to power down.
}

void startUp() {
    //  DEVICE START-UP IN SERIAL MODE, HARDWIRE PIN 2 AT 0, PIN 3 AT 1, AND PIN 4 AT 1

}

int mphToHz(float mph) {
    return 0;
}

void SS_SetSpeedMph(float mph) {
    int freq = mphToHz(mph);

}

void printDeviceCount() {
    FT_STATUS ftStatus;
    DWORD numDevices;
    ftStatus = FT_ListDevices(&numDevices, NULL, FT_LIST_NUMBER_ONLY);
    if (ftStatus == FT_OK) {
        printf("Num devices: %i\r\n", numDevices);
    }
    else {
        printf("ERROR: %i\r\n", ftStatus);
    }
}

int main() {
    printDeviceCount();

    // writeRegister(40);
    // return 0;

    // FT_DEVICE ft_device = FT_DEVICE_232R;
    // FT_HANDLE handle;

    // /* Initialize, open device, set bitbang mode w/5 outputs */
    // if(FT_Open(0, &handle) != FT_OK) {
    //     printf("Can't open device");
    //     return 1;
    // }
    // FT_SetBitMode(handle, W_CLK | FQ_UD | DATA | RESET, 1);
    // FT_SetBaudRate(handle, FT_BAUD_9600);  /* Actually 9600 * 16 */

    /* Endless loop: dump precomputed PWM data to the device */
    //for(;;) FT_Write(handle, &data, (DWORD)sizeof(data), &bytes);
}    
