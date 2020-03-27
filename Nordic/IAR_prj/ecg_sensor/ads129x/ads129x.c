

#include <stdbool.h>
#include <stdlib.h>
#include <string.h>
#include <stdio.h>


#include "../ecg_sensor.h"
#include "ads129x.h"
#include "ecg_pin_config.h"


#include "nrf_drv_spi.h"
#include "nrf_gpio.h"
#include "nrf_delay.h"

#define SPI_INSTANCE  0 /**< SPI instance index. */
static const nrf_drv_spi_t spi = NRF_DRV_SPI_INSTANCE(SPI_INSTANCE);  /**< SPI instance. */
static volatile bool spi_xfer_done;  /**< Flag used to indicate that SPI instance completed the transfer. */
static uint8_t bytesToSend;

static uint8_t adc_registers[ADC_RESULT_LENGTH];
static uint32_t ecgResult[ADS_BUFFER_LENGTH][ADS_CHANNEL_COUNT];
static uint16_t historyIndexHead;
static uint16_t historyIndexTail;

//#define TEST_STRING "Nordic1234567890 123456789 123456789"
static uint8_t       m_tx_buf[32];// = TEST_STRING;           /**< TX buffer. */
static uint8_t       m_rx_buf[32];    /**< RX buffer. */


////////////////////////////////////////////////////////////////////////////////

const AdsRerister_t regilarSettings[] = {
	{0x01, 0x05}, // CONFIG1 = default + 0x5	fMOD = 500SPS
	{0x02, 0x00}, // 0x01 for test mode
	{0x03, (char)( BIT7 | BIT6 | BIT3 | BIT2 ) }, // CONFIG3
	{0x04, (char)( BIT7 | BIT6 | BIT1 | BIT0 ) }, // Address = 04h  LOFF: Lead-Off Control Register

	{0x05, 0x00}, // Address = 05h to 0Ch CHnSET: Individual Channel Settings (n = 1 : 8)
	{0x06, 0x00},
	{0x07, 0x00},
	{0x08, 0x00},
	{0x09, 0x00},
	{0x0a, 0x00},
	{0x0b, 0x00},
	{0x0c, 0x00},

	{0x0d, 0x03}, // Address = 0Dh RLD_SENSP Right led driver positive
	{0x0e, 0x01}, // Address = 0Eh  RLD_SENSN negative
	
	{0x0f, 0xff},
	{0x10, 0xff},
	{0x11, 0xff},

	{0x15, 0x00}, // Address = 15h PACE: PACE Detect Register
	{0x16, 0x20}, // Address = 16h RESP: Respiration Control Register
	{0x17, (char)(BIT1)}, // Address = 17h CONFIG4: bit1 = Lead-off comparators enabled
	{0x18, 0x08}, // Address = 18h WCT1: Wilson Central Terminal and Augmented Lead Control Register
	{0x19, 0xd1}, // Address = 19h WCT2: Wilson Central Terminal Control Register
};
////////////////////////////////////////////////////////////////////////////////


const AdsRerister_t internalTestSettings[] = {
	{0x01, 0x05}, // CONFIG1 = default + 0x5	fMOD = 500SPS
	{0x02, 0x10}, // 0x10 for test mode
	{0x03, (char)( BIT7 | BIT6 | BIT3 | BIT2 ) }, // CONFIG3
	{0x04, (char)( BIT7 | BIT6 | BIT1 | BIT0 ) }, // Address = 04h  LOFF: Lead-Off Control Register

	{0x05, 0x05}, // Address = 05h to 0Ch CHnSET: Individual Channel Settings (n = 1 : 8)
	{0x06, 0x05},
	{0x07, 0x05},
	{0x08, 0x05},
	{0x09, 0x05},
	{0x0a, 0x05},
	{0x0b, 0x05},
	{0x0c, 0x05},

	{0x0d, 0x03}, // Address = 0Dh RLD_SENSP Right led driver positive
	{0x0e, 0x01}, // Address = 0Eh  RLD_SENSN negative
	
	{0x0f, 0xff},
	{0x10, 0xff},
	{0x11, 0xff},

	{0x15, 0x00}, // Address = 15h PACE: PACE Detect Register
	{0x16, 0x20}, // Address = 16h RESP: Respiration Control Register
	{0x17, (char)(BIT1)}, // Address = 17h CONFIG4: bit1 = Lead-off comparators enabled
	{0x18, 0x08}, // Address = 18h WCT1: Wilson Central Terminal and Augmented Lead Control Register
	{0x19, 0xd1}, // Address = 19h WCT2: Wilson Central Terminal Control Register
};
///////////////////////////////////////////////////////////////////////////////

bool getEcgVector(int32_t* p_vector) {

	if ( historyIndexHead == historyIndexTail ) {
		return false;
	}
	
	memcpy(p_vector, &ecgResult[historyIndexTail][0], sizeof(int32_t)*ADS_CHANNEL_COUNT);
	historyIndexTail = (historyIndexTail+1) & (ADS_BUFFER_LENGTH-1);
	
	return true;
}
///////////////////////////////////////////////////////////////////////////////


void pinsOff(void) {

}
////////////////////////////////////////////////////////////////////////////////


void startConversion(void) {
	
	pinsInit();
	// config chip ship 
	
	AdsRerister_t reg = {0x00, 0x01}; // fix me or comment me 
	adsSetRegister(&reg);
	
	
	
	CLEAR_PENDING_INTERRUPTS;
	
	// set interrupt function interruptFromReadyPin
	ADS129X_ENABLE_INTERRUPT;
	START_ADS129X;
}
////////////////////////////////////////////////////////////////////////////////


void stopConversion(void) {

	ADS129X_DISNABLE_INTERRUPT;
	AdsRerister_t reg = {0x00, 0x00};
	adsSetRegister(&reg);
	pinsOff();
}
////////////////////////////////////////////////////////////////////////////////


void spi_event_handler(nrf_drv_spi_evt_t const * p_event,
                       void *                    p_context)
{
    spi_xfer_done = true;
	TEST_OFF;
}
////////////////////////////////////////////////////////////////////////////////


uint8_t sendByte(uint8_t data) {

	m_tx_buf[0] = data;
	bytesToSend = 1;
//	spi_xfer_done = false;
	APP_ERROR_CHECK(nrf_drv_spi_transfer(&spi, m_tx_buf, bytesToSend, m_rx_buf, bytesToSend));
//	while ( !spi_xfer_done ) ;

	return m_rx_buf[0];
}
///////////////////////////////////////////////////////////////////////////////


bool checkChipId(void) {

//	pinsInit();

	bool ret = false;
	m_tx_buf[0] = READ_REG_COMMAND  | 0; // 0 register address
	m_tx_buf[1] = 0;  // register count to read
	m_tx_buf[2] = 0;  // get result 
	bytesToSend = 3;
	
//	spi_xfer_done = false;
	TEST_ON;
	APP_ERROR_CHECK(nrf_drv_spi_transfer(&spi, m_tx_buf, bytesToSend, m_rx_buf, bytesToSend));
//	while ( !spi_xfer_done ) ;
	TEST_OFF;

	unsigned char id = m_rx_buf[2];
	if ( id == ADC1298_DEVICE_ID || id == ADC1298R_DEVICE_ID ) {
		ret = true;
	}
	if ( id == ADC1294_DEVICE_ID || id == ADC1294R_DEVICE_ID ) {
		ret = true;
	}
	
	return ret;
}
///////////////////////////////////////////////////////////////////////////////


void pinsInit(void) {

	memset(m_tx_buf, 0, sizeof(m_tx_buf));
	memset(m_rx_buf, 0, sizeof(m_rx_buf));
	
	nrf_gpio_cfg_output(ADS_PWDN);
	nrf_gpio_cfg_input(ADS_READY, NRF_GPIO_PIN_PULLDOWN);
	ADS_POWER_OFF;
	nrf_delay_ms(100);
	
	// init spi
    nrf_drv_spi_config_t spi_config = SPI_ADS1298_CONFIG;
//    APP_ERROR_CHECK(nrf_drv_spi_init(&spi, &spi_config, spi_event_handler, NULL));
    APP_ERROR_CHECK(nrf_drv_spi_init(&spi, &spi_config, NULL, NULL));
	
	ADS_POWER_ON;
	nrf_delay_ms(200);
	
	// send RESET command
	sendByte(RESET_COMMAND);
	nrf_delay_ms(100);
	
	sendByte(SDATAC_COMMAND);
}
////////////////////////////////////////////////////////////////////////////////


void debugSpi(void) {

	pinsInit();
	while(1) {
		nrf_delay_us(100);
		uint8_t OK = checkChipId();

		if ( OK ) {
			OK = 1;
		} else {
			OK = 0;
		}

	}
	
}
////////////////////////////////////////////////////////////////////////////////


void chipInit(EcgParams_t* _ecgParams) {
	
	pinsInit();
	
	// set ads registers
	uint8_t regCount;
	AdsRerister_t* p_registers;
	
	switch (_ecgParams->ecgMode) {
	case ECG_MODE:
		regCount = sizeof(regilarSettings) / sizeof(AdsRerister_t);
		p_registers = (AdsRerister_t*)&regilarSettings[0];
		break;
	case INTERNAL_TEST_MODE:
		regCount = sizeof(internalTestSettings) / sizeof(AdsRerister_t);
		p_registers = (AdsRerister_t*)&internalTestSettings[0];
		break;
	default:
		regCount = 0;
		break;
	}

	for ( int i = 0; i < regCount; i++ ) {
		adsSetRegister(p_registers++);
	}

	// Addirional settings
	if ( _ecgParams->samplingRate != DEFAULT_ADC_RATE ) {
		AdsRerister_t regRate = {1,0x05};
		switch (_ecgParams->samplingRate) {
			case 250: regRate.data = 0x06; break;
			case 1000: regRate.data = 0x04; break;
			default: regRate.data = 0x05; break;
		}
		adsSetRegister(&regRate);
	}
}
////////////////////////////////////////////////////////////////////////////////


void adsSetRegister(AdsRerister_t* p_register) {

	ADS129X_CS_ON;
		uint8_t command = WRITE_REG_COMMAND | p_register->address;
		adsSendByte(command);
		adsSendByte(0);
		adsSendByte(p_register->data);
	ADS129X_CS_OFF;

}
////////////////////////////////////////////////////////////////////////////////

uint8_t adsReadRegister(AdsRerister_t* p_register) {

	ADS129X_CS_ON;
		adsSendByte(SDATAC_COMMAND);
		adsSendByte(READ_REG_COMMAND | p_register->address);
		adsSendByte(0);  // bytes to be read
		p_register->data = adsSendByte(0);
	ADS129X_CS_OFF;

	return p_register->data;
}
////////////////////////////////////////////////////////////////////////////////


uint8_t adsSendByte(uint8_t data) {
	
	ADS129X_WAIT_TXDONE;   // wait previouse transfer
//	ADS129X_TXBUF = data;  // 
	ADS129X_WAIT_RXDATA;

	uint8_t ret = ADS129X_RXBUF; // dummy read
	return ret;
}
////////////////////////////////////////////////////////////////////////////////




void OffPins(void) {

//	OFF_ADS_PINS;
//	ADS129X_POWER_OFF;
}
////////////////////////////////////////////////////////////////////////////////


void readConversionResult() {

	ADS129X_CS_ON;
	adsSendByte(RDATA_COMMAND);

	for ( int i = 0; i < ADC_RESULT_LENGTH; i++ ) {
		adc_registers[i] = adsSendByte(0);
	}
	
	adsSendByte(RDATAC_COMMAND);
	ADS129X_CS_OFF;
}
////////////////////////////////////////////////////////////////////////////////


void parseConversionResult() {

	unsigned char ctemp;
	char* p_result = (char*)(&ecgResult[historyIndexHead][0]);
	char* reg = (char*)&adc_registers[3]; // skip status info

	for (int i = 0; i < ADS_CHANNEL_COUNT; i++, reg+=3 ) {
		*(p_result++) = reg[2];
		*(p_result++) = reg[1];

		ctemp = *(p_result++) = reg[0];
		if ( (ctemp & 0x80) != 0 ) {
			*p_result++ = 0xff;
		} else {
			*p_result++ = 0;
		}
	}
	
	historyIndexHead = (historyIndexHead+1) & (ADS_BUFFER_LENGTH-1);
}
////////////////////////////////////////////////////////////////////////////////


void interruptFromReadyPin(nrf_drv_gpiote_pin_t pin,
						 nrf_gpiote_polarity_t action) {

	// Crear intrerrupt flag

	// Read and parce cenversion result
	readConversionResult();
	parseConversionResult();
}
////////////////////////////////////////////////////////////////////////////////