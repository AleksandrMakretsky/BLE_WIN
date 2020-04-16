#ifndef _LEDS_PINS_H_
#define _LEDS_PINS_H_

#include "nrf_gpio.h"

#define LED_R          NRF_GPIO_PIN_MAP(0,29)
#define LED_G          NRF_GPIO_PIN_MAP(0,31)
#define LED_B          NRF_GPIO_PIN_MAP(0,30)
#define LED_IR         NRF_GPIO_PIN_MAP(0,2)

#define LED_R_ON       nrf_gpio_pin_write(LED_R, 0);
#define LED_R_OFF      nrf_gpio_pin_write(LED_R, 1);
#define LED_G_ON       nrf_gpio_pin_write(LED_G, 0);
#define LED_G_OFF      nrf_gpio_pin_write(LED_G, 1);
#define LED_B_ON       nrf_gpio_pin_write(LED_B, 0);
#define LED_B_OFF      nrf_gpio_pin_write(LED_B, 1);
#define LED_OFF        {LED_R_OFF; LED_G_OFF;LED_B_OFF;}

#define LED_IR_ON      nrf_gpio_pin_write(LED_IR, 1);
#define LED_IR_OFF     nrf_gpio_pin_write(LED_IR, 0);

#endif // _LEDS_PINS_H_
