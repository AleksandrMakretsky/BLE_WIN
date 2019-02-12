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
#include "../MyNordic/flash_mem.h"

#include "host_interfase.h"

extern char* received_msg_buffer;

#pragma data_alignment=2
char response_buffer[128];
#pragma data_alignment=2
char data_block_buffer[512+20];
ChannelWriteFn_t channelWriteFn = NULL; //callback function to push data to channel 

extern char firmware_version[VERSION_NAME_LENGTH];

DsDeviceId dsDeviceId = {NUMBER_NAME_DEFAULT, NUMBER_DEVICE_DEFAULT};

void parseIncomingMessage(char* received_msg);
void readDeviceId();
void storeDeviceId();

///////////////////////////////////////////////////////////////////////////////


void hostInterfaseInit() {
	NetLevelInit();
}
///////////////////////////////////////////////////////////////////////////////


void addIncomingData(char data) {

	if ( NetLevelAddIncomingByte(data) >= 0 )	{
		parseIncomingMessage(received_msg_buffer);
	}
}
///////////////////////////////////////////////////////////////////////////////


void parseIncomingMessage(char* received_msg) {

	short opp_code = ((message_header_st*)received_msg)->opcode;
	bool responce = false;

	NRF_LOG_INFO("Got a message: 0x%.2x", opp_code);

	if ( !(NetLevelIsCrcCorrect(received_msg)) ) {
		opp_code = -1;
	}

	switch (opp_code ) {
		case CMD_GET_DEVICE_ID:
			readDeviceId();
			NetLevelCreateResponse(response_buffer, (char*)&dsDeviceId,
				sizeof(dsDeviceId), CMD_DEVICE_ID);
			responce = true;
		break;
		case CMD_SET_DEVICE_ID:
			NetLevelGetMessageData(received_msg, (char*)&dsDeviceId,
				sizeof(dsDeviceId));
			storeDeviceId();
			NetLevelCreateResponse(response_buffer, (char*)&dsDeviceId,
				sizeof(dsDeviceId), CMD_DEVICE_ID);
			responce = true;
		break;
		case CMD_GET_FIRMWARE_VERSION:
			NetLevelCreateResponse(response_buffer, firmware_version,
				sizeof(firmware_version), CMD_FIRMWARE_VERSION);
			responce = true;
		break;
	}

	if ( responce ) {
		
		uint16_t count;
		count = ((message_header_st*)response_buffer)->data_length +
			sizeof(message_header_st) + 1;

		NRF_LOG_INFO("Responce length: %d", count);
		if ( channelWriteFn != NULL ) {
			channelWriteFn((char*)response_buffer, count);
		}
	}
}
///////////////////////////////////////////////////////////////////////////////


void readDeviceId() {
	
	FlashMemSegmentRead((char*)&dsDeviceId,
		sizeof(dsDeviceId), FLASH_DEVICEID_OFFSET);
	
	if ( dsDeviceId.device_class[0] == 0xff ) {
		dsDeviceId = (DsDeviceId){NUMBER_NAME_DEFAULT, NUMBER_DEVICE_DEFAULT};
		FlashMemSegmentWrite((char*)&dsDeviceId,
			sizeof(dsDeviceId), FLASH_DEVICEID_OFFSET);
	}
	
	dsDeviceId.device_class[DEVICE_ID_NAME_LENGTH-1] = 0;
	dsDeviceId.device_number[DEVICE_ID_NAME_LENGTH-1] = 0;
}
///////////////////////////////////////////////////////////////////////////////


void storeDeviceId() {

	FlashMemSegmentWrite((char*)&dsDeviceId,
		sizeof(dsDeviceId), FLASH_DEVICEID_OFFSET);
}
///////////////////////////////////////////////////////////////////////////////
