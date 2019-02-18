/*
host interfase
*/

#include <stdlib.h>
#include <string.h>
#include <stdio.h>

#include "nrf_log.h"
#include "nrf_log_ctrl.h"
#include "nrf_log_default_backends.h"

#include "../DsCommons/bit_operations.h"
#include "../DsCommons/net_level.h"
#include "../DsCommons/bee_data_types.h"
#include "../DsCommons/bee_command_codes.h"
#include "../DsCommons/bee_dev_info.h"

#include "spiro_process.h"
#include "timestamp_timer.h"
#include "nrf_drv_timer.h"
#include "compressor.h"


const nrf_drv_timer_t TIMEOUT_TIMER_100MS = NRF_DRV_TIMER_INSTANCE(2);
static bool initDone = false;
////////////////////////////////////////////////////////////////////////////////


void spiroProcessResetTimeout() {

	nrf_drv_timer_clear(&TIMEOUT_TIMER_100MS);
}
////////////////////////////////////////////////////////////////////////////////


static void timeoutTimerHandler(nrf_timer_event_t event_type, void* p_context) {
	
	uint32_t  temp_ts = getTimestamp();
	for (int i = 0; i < 16; i++) {
		compressorAddRaw32(temp_ts);
	}

	NRF_LOG_INFO("no data from turbine timeout 100 ms");
}
////////////////////////////////////////////////////////////////////////////////


void initTimeoutTimer(){
	
	//Configure TIMER for data timeout
	nrf_drv_timer_config_t timer_cfg =
	{
		.frequency          = (nrf_timer_frequency_t)4, // 1 MHz
		.mode               = (nrf_timer_mode_t)NRFX_TIMER_DEFAULT_CONFIG_MODE,
		.bit_width          = (nrf_timer_bit_width_t)3, // 32 bit
		.interrupt_priority = NRFX_TIMER_DEFAULT_CONFIG_IRQ_PRIORITY,
		.p_context          = NULL
	};

	uint32_t err_code = NRF_SUCCESS;
	err_code = nrfx_timer_init(&TIMEOUT_TIMER_100MS, &timer_cfg, timeoutTimerHandler);
	APP_ERROR_CHECK(err_code);

	uint32_t timeMs = 500;
	uint32_t timeTicks = nrf_drv_timer_ms_to_ticks(&TIMEOUT_TIMER_100MS, timeMs);
	
	nrf_drv_timer_extended_compare(
		 &TIMEOUT_TIMER_100MS, NRF_TIMER_CC_CHANNEL0, timeTicks, NRF_TIMER_SHORT_COMPARE0_CLEAR_MASK, true);

	initDone = true;
}
////////////////////////////////////////////////////////////////////////////////


void spiroProcessInit() {
	
	initTimeoutTimer();
}
////////////////////////////////////////////////////////////////////////////////


void spiroProcessStart() {
	
	if ( !initDone ) {
		initTimeoutTimer();
	}
	nrf_drv_timer_clear(&TIMEOUT_TIMER_100MS);
	nrf_drv_timer_enable(&TIMEOUT_TIMER_100MS);
}
////////////////////////////////////////////////////////////////////////////////


void spiroProcessStop() {
	
	if ( !initDone ) { // need it in debug mode
		initTimeoutTimer();
	}
	nrf_drv_timer_disable(&TIMEOUT_TIMER_100MS);
}
////////////////////////////////////////////////////////////////////////////////
