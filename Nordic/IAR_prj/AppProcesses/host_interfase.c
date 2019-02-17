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
#include "../DsCommons/monitoring_modes.h"

#include "../MyNordic/flash_mem.h"
#include "host_interfase.h"

#ifdef SPIRO
#include "spiro_process.h"
#endif
#ifdef ECG
#include "ecg_process.h"
#endif
#ifdef PHONE
#include "mic_process.h"
#endif

extern char* received_msg_buffer;
extern void (* CompressorBlockReadyCallBack)(char* buffer);

#pragma data_alignment=2
char response_buffer[128];
#pragma data_alignment=2
char data_block_buffer[512+20];
ChannelWriteFn_t channelWriteFn = NULL; //callback function to push data to channel 

extern char firmware_version[VERSION_NAME_LENGTH];
extern void (* CompressorBlockReadyCallBack)(char* buffer);

DsDeviceId dsDeviceId = {NUMBER_NAME_DEFAULT, NUMBER_DEVICE_DEFAULT};
static uint8_t monitoringMode = MONITORING_MODE_OFF;
static bool responceToSend = false;
static bool dataToSend = false;

void parseIncomingMessage(char* received_msg);
void readDeviceId();
void storeDeviceId();
void onCommandSetMonitoringMode(uint8_t modeValue);
void OnDataBlockReady(char* data);


///////////////////////////////////////////////////////////////////////////////

void sendDataToHost(char* data) {

	uint16_t count;
	count = ((message_header_st*)data)->data_length +
		sizeof(message_header_st) + 1;

	if ( channelWriteFn != NULL ) {
		NRF_LOG_INFO("send responce with length: %d", count);
		channelWriteFn((char*)data, count);
	}
}
///////////////////////////////////////////////////////////////////////////////

void hostInterfaseProcessPoll(bool _readyToSend) {

	if ( _readyToSend ) {
		if ( responceToSend ) {
			sendDataToHost(response_buffer);
			responceToSend = false;
		}
		if ( dataToSend ) {
			sendDataToHost(data_block_buffer);
			dataToSend = false;
		}
	}

}
///////////////////////////////////////////////////////////////////////////////

void OnDataBlockReady(char* data) {

	NetLevelCreateResponse(data_block_buffer, (char*)data, 512, CMD_DATA_BLOCK);
	dataToSend = true;
}
///////////////////////////////////////////////////////////////////////////////


void hostInterfaseInit() {
	NetLevelInit();
	monitoringMode = MONITORING_MODE_OFF;
	CompressorBlockReadyCallBack = OnDataBlockReady;
}
///////////////////////////////////////////////////////////////////////////////


void addIncomingData(char data) {

	if ( NetLevelAddIncomingByte(data) >= 0 )	{
		parseIncomingMessage(received_msg_buffer);
	}
}
///////////////////////////////////////////////////////////////////////////////


void parseIncomingMessage(char* received_msg) {

	uint16_t opp_code = ((message_header_st*)received_msg)->opcode;
//	bool responce = false;
	uint8_t tempChar;
	

	NRF_LOG_INFO("Got a message: 0x%.2x", opp_code);

	if ( !(NetLevelIsCrcCorrect(received_msg)) ) {
		opp_code = -1;
	}

	switch (opp_code ) {
		
		case CMD_GET_DEVICE_ID:
			readDeviceId();
			NetLevelCreateResponse(response_buffer, (char*)&dsDeviceId,
				sizeof(dsDeviceId), CMD_DEVICE_ID);
			responceToSend = true;
		break;
		
		case CMD_SET_DEVICE_ID:
			NetLevelGetMessageData(received_msg, (char*)&dsDeviceId,
				sizeof(dsDeviceId));
			storeDeviceId();
			NetLevelCreateResponse(response_buffer, (char*)&dsDeviceId,
				sizeof(dsDeviceId), CMD_DEVICE_ID);
			responceToSend = true;
		break;
		
		case CMD_GET_FIRMWARE_VERSION:
			NetLevelCreateResponse(response_buffer, firmware_version,
				sizeof(firmware_version), CMD_FIRMWARE_VERSION);
			responceToSend = true;
		break;
		
		case CMD_SET_MONITORING_MODE:
			NetLevelGetMessageData(received_msg, (char*)&tempChar,
				sizeof(tempChar));
			onCommandSetMonitoringMode(tempChar);
			NetLevelCreateResponse(response_buffer, (char*)&monitoringMode,
				sizeof(monitoringMode), CMD_MONITORING_MODE);
			responceToSend = true;
		break;
	}
}
///////////////////////////////////////////////////////////////////////////////


void onCommandSetMonitoringMode(uint8_t modeValue) {

//	uint8_t previouseMode = monitoringMode;
	if ( monitoringMode == modeValue ) {
		NRF_LOG_INFO("the same mode. ignoge it: %d", monitoringMode);
		return;
	}
	
	monitoringMode = modeValue;
	NRF_LOG_INFO("now monitoringMode will be: %d", monitoringMode);

	if ( monitoringMode == MONITORING_MODE_RUN ) {
		spiroProcessStart();
	}
	
	if ( monitoringMode == MONITORING_MODE_OFF ) {
		spiroProcessStop();
	}
}
///////////////////////////////////////////////////////////////////////////////


void readDeviceId() {
	
	flashMemSegmentRead((char*)&dsDeviceId,
		sizeof(dsDeviceId), FLASH_DEVICEID_OFFSET);
	
	if ( dsDeviceId.device_class[0] == 0xff ) {
		dsDeviceId = (DsDeviceId){NUMBER_NAME_DEFAULT, NUMBER_DEVICE_DEFAULT};
		flashMemSegmentWrite((char*)&dsDeviceId,
			sizeof(dsDeviceId), FLASH_DEVICEID_OFFSET);
	}
	
	dsDeviceId.device_class[DEVICE_ID_NAME_LENGTH-1] = 0;
	dsDeviceId.device_number[DEVICE_ID_NAME_LENGTH-1] = 0;
}
///////////////////////////////////////////////////////////////////////////////


void storeDeviceId() {

	flashMemSegmentWrite((char*)&dsDeviceId,
		sizeof(dsDeviceId), FLASH_DEVICEID_OFFSET);
}
///////////////////////////////////////////////////////////////////////////////
