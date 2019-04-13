

#ifndef _SPIRO_PIN_CONFIG_H_
#define _SPIRO_PIN_CONFIG_H_

#include "nrf_gpio.h"
#include "nrf.h"
#include "nrf_drv_gpiote.h"


/*
#define LED_1          NRF_GPIO_PIN_MAP(0,13)
#define LED_2          NRF_GPIO_PIN_MAP(0,14)
#define LED_3          NRF_GPIO_PIN_MAP(0,15)
#define LED_4          NRF_GPIO_PIN_MAP(0,16)
*/
#define TEST_PIN          NRF_GPIO_PIN_MAP(1,0)

//nrf_gpio_pin_write
#define TEST_ON  nrf_gpio_pin_write(TEST_PIN, 1);
#define TEST_OFF nrf_gpio_pin_write(TEST_PIN, 0);
#define TEST_INV nrf_gpio_pin_toggle(TEST_PIN);


#define TEST2_ON  ;
#define TEST2_OFF ;
#define TEST2_INV ;

#define IN1          NRF_GPIO_PIN_MAP(1,9)
#define IN2          NRF_GPIO_PIN_MAP(1,11)





void initSpiroPins(nrfx_gpiote_evt_handler_t evt_handler);

#endif // _SPIRO_PIN_CONFIG_H_
