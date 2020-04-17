
#include <stdbool.h>
#include <stdlib.h>
#include <string.h>
#include <stdio.h>

#include "acc_sensor.h"
#include "nrf_gpio.h"
#include "nrf_delay.h"

#include "nrf_drv_twi.h"
#include "board_pins.h"
////////////////////////////////////////////////////////////////////////////////

// TWI instance ID.
#define TWI_INSTANCE_ID 1
////////////////////////////////////////////////////////////////////////////////


bool chipLis3Init();


#define REG_COUNT   8
const uint8_t LIS3DH_register_settings[REG_COUNT][2] = {
	
	// enable Z axe, odr 200Hz(T = 5ms)
	{R20_CTRL_REG1, ODR_200 + Z_EN + LP_MODE},

	// Click interrupt on INT1.
	{R22_CTRL_REG3, I1_CLICK},

	// BDU enabled, Full-scale selection 4g, High-resolution output mode ON
	{R23_CTRL_REG4, S4G},

	// Enable interrupt double click on XYZ-axis 
	{R38_CLICK_CFG, ZDOUDLE},
	
	// Click threshold max = 127)
	{R3A_CLICK_THS, CLICK_THRESHOLD},

	// Click time limit max = 127 LSB = 5ms
	{R3B_TIME_LIMIT, TIME_LIMIT_MS/ONE_TIC_MS},
	
	// Click time latency int1-active during that time. max = 255
	{R3C_TIME_LATENCY, TIME_LATENCY_MS/ONE_TIC_MS},
	
	// Click time window max255 should be greater than latency
	{R3D_TIME_WINDOW, TIME_WINDOW_MS/ONE_TIC_MS},
};

static uint8_t m_sample;
static uint8_t registr[2];

const nrf_drv_twi_xfer_desc_t twiTransfer = {
	.type     = NRF_DRV_TWI_XFER_TXRX,
	.address  = LM75B_CHIP_ADR,
	.primary_length = 1,
	.secondary_length = 1,
	.p_primary_buf = registr,
	.p_secondary_buf = (uint8_t *)&m_sample
};


// Indicates if operation on TWI has ended.
static volatile bool m_xfer_done = false;
static bool accChipOk = true;

// TWI instance.
static const nrf_drv_twi_t m_twi = NRF_DRV_TWI_INSTANCE(TWI_INSTANCE_ID);
////////////////////////////////////////////////////////////////////////////////


/**
 * @brief TWI events handler.
 */
void twi_handler(nrf_drv_twi_evt_t const * p_event, void * p_context) {

	m_xfer_done = true;
	switch (p_event->type) {
		case NRF_DRV_TWI_EVT_DONE:
//			if (p_event->xfer_desc.type == NRF_DRV_TWI_XFER_RX) ;
			break;
		case NRF_DRV_TWI_EVT_ADDRESS_NACK:
		case NRF_DRV_TWI_EVT_DATA_NACK:
			accChipOk = false;
			break;
		default:
		break;
	}
}
////////////////////////////////////////////////////////////////////////////////


void interruptFromAccPin(nrf_drv_gpiote_pin_t pin, nrf_gpiote_polarity_t action) {

	TEST_ON;
}
////////////////////////////////////////////////////////////////////////////////


void twi_init (void) {
	
	ret_code_t err_code;

	const nrf_drv_twi_config_t twi_lm75b_config = {
	   .scl                = SCL_PIN,
	   .sda                = SDA_PIN,
	   .frequency          = NRF_DRV_TWI_FREQ_100K,
	   .interrupt_priority = APP_IRQ_PRIORITY_HIGH,
	   .clear_bus_init     = true
	};

	err_code = nrf_drv_twi_init(&m_twi, &twi_lm75b_config, twi_handler, NULL);
	APP_ERROR_CHECK(err_code);

	nrf_drv_twi_enable(&m_twi);
	
	// ACC_INT_PIN init 
	if ( !nrfx_gpiote_is_init() ) {
		err_code = nrf_drv_gpiote_init();
		APP_ERROR_CHECK(err_code);
	}
	nrf_drv_gpiote_in_config_t in_config = NRFX_GPIOTE_CONFIG_IN_SENSE_HITOLO(true);
    in_config.pull = NRF_GPIO_PIN_PULLUP;
	err_code = nrf_drv_gpiote_in_init(ACC_INT_PIN, &in_config, interruptFromAccPin);
	APP_ERROR_CHECK(err_code);

	nrf_drv_gpiote_in_event_disable(ACC_INT_PIN); //  DISNABLE INTERRUPT;
}
////////////////////////////////////////////////////////////////////////////////


void writeTwi(uint8_t * data, uint8_t len) {

	m_xfer_done = false;
	nrf_drv_twi_tx(&m_twi, LM75B_CHIP_ADR, data, len, false);
	while (!m_xfer_done);
}
////////////////////////////////////////////////////////////////////////////////


bool chipLis3Init() {

	accChipOk = true;
	
	// init acc settings (registers)
	for( int i = 0; i < REG_COUNT; i++ ) {
		registr[0] = (LIS3DH_register_settings[i][0]);
		registr[1] = (LIS3DH_register_settings[i][1]);
		writeTwi(registr, 2);
	}

	return accChipOk;
}
////////////////////////////////////////////////////////////////////////////////


bool accInit() {

	bool chipOk = true;

	twi_init();
	chipOk = chipLis3Init();

	if ( chipOk ) {
		nrf_drv_gpiote_in_event_enable(ACC_INT_PIN, true);  // ENABLE INTERRUPT;
	}
		
	return chipOk;
}
////////////////////////////////////////////////////////////////////////////////

