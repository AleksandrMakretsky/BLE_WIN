#ifndef _ACC_SENSOR_H_
#define _ACC_SENSOR_H_

#include <stdint.h>
#include <types_porting.h>

#define LM75B_CHIP_ADR     0x19U // LIS3DH chip factory address 

// Common addresses definition for acc chip
#define R00_REG_TEMP       0x00U
#define R01_REG_CONF       0x01U
#define R02_REG_THYST      0x02U
#define R03_REG_TOS        0x03U

#define R1F_TEMP_CFG_REG   0x1FU

#define R20_CTRL_REG1      0x20U
#define R21_CTRL_REG2      0x21U
#define R22_CTRL_REG3      0x22U
#define R23_CTRL_REG4      0x23U
#define R24_CTRL_REG5      0x24U
#define R25_CTRL_REG6      0x25U

#define R30_INT1_CFG       0x30U
#define R31_INT1_SRC       0x31U
#define R32_INT1_THS       0x32U
#define R33_INT1_DURATION  0x33U
#define R34_INT2_CFG       0x34U
#define R35_INT2_SRC       0x35U
#define R36_INT2_THS       0x36U
#define R37_INT2_DURATION  0x37U
#define R38_CLICK_CFG      0x38U

#define R3A_CLICK_THS      0x3AU 
#define R3B_TIME_LIMIT     0x3BU
#define R3C_TIME_LATENCY   0x3CU
#define R3D_TIME_WINDOW    0x3DU

// R20_CTRL_REG1 values
#define Z_EN        0x04
#define Y_EN        0x02
#define X_EN        0x01
#define LP_MODE     0x08
#define ODR_50      0x40
#define ODR_100     0x50
#define ODR_200     0x60
#define ODR_400     0x70

// R22_CTRL_REG3 values
#define I1_CLICK    0x80

// R23_CTRL_REG4 values
#define S2G         0x00
#define S4G         0x10
#define S8G         0x20
#define S16G        0x30
#define HI_RES      0x08
#define BDU         0x80

// R24_CTRL_REG5 values
#define D4D_INT1    0x04

// R38_CLICK_CFG values
#define ZDOUDLE     0x20
#define ZSINGLE     0x10
#define XSINGLE     0x01

// click parameters
#define TIME_LIMIT_MS    50
#define TIME_LATENCY_MS  125
#define TIME_WINDOW_MS   500
#define ONE_TIC_MS       5
#define MAX_THRESHOLD    127
#define CLICK_THRESHOLD  (MAX_THRESHOLD/2)


bool accInit();

#endif // _ACC_SENSOR_H_
