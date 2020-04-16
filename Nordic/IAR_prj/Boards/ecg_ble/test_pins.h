#ifndef _TEST_PINS_H_
#define _TEST_PINS_H_

#include "nrf_gpio.h"
//#include "nrf.h"
//#include "nrf_drv_gpiote.h"
//#include "ads1298_pin_config.h"

#define TEST_PIN       NRF_GPIO_PIN_MAP(0,30)
#define TEST_ON        nrf_gpio_pin_write(TEST_PIN, 1);
#define TEST_OFF       nrf_gpio_pin_write(TEST_PIN, 0);
#define TEST_INV       nrf_gpio_pin_toggle(TEST_PIN);

#define TEST2_PIN      NRF_GPIO_PIN_MAP(0,31)
#define TEST2_ON       nrf_gpio_pin_write(TEST2_PIN, 1);
#define TEST2_OFF      nrf_gpio_pin_write(TEST2_PIN, 0);
#define TEST2_INV      nrf_gpio_pin_toggle(TEST2_PIN);


#endif // _TEST_PINS_H_
