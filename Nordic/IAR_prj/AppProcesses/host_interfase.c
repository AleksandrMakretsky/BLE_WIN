/*
host interfase
*/

#include <stdlib.h>
#include <string.h>
#include <stdio.h>

#include "../DsCommons/bit_operations.h"
#include "../DsCommons/net_level.h"
#include "../DsCommons/bee_data_types.h"
#include "../DsCommons/bee_command_codes.h"
#include "../DsCommons/bee_dev_info.h"
#include "../MyNordic/flash_mem.h"

#include "host_interfase.h"
#include "usb_wrapper.h"

extern char* received_msg_buffer;

#pragma data_alignment=2
char response_buffer[128];
#pragma data_alignment=2
char data_block_buffer[512+20];

extern char firmvare_version[VERSION_NAME_LENGTH];

device_id_st the_device_id = {NUMBER_NAME_DEFAULT, NUMBER_DEVICE_DEFAULT};


void parseIncomingMessage(char* received_msg);
void readDeviceId();
///////////////////////////////////////////////////////////////////////////////


void hostInterfaseInit() {
	NetLevelInit();
}
///////////////////////////////////////////////////////////////////////////////


void addIncomingData(char data)
{
	if ( NetLevelAddIncomingByte(data) >= 0 )	{
		parseIncomingMessage(received_msg_buffer);
	}
}
///////////////////////////////////////////////////////////////////////////////


void parseIncomingMessage(char* received_msg) {

	short opp_code = ((message_header_st*)received_msg)->opcode;
	bool responce = false;

	if ( !(NetLevelIsCrcCorrect(received_msg)) ) {
		opp_code = -1;
	}

	switch (opp_code ) {
		case CMD_GET_DEVICE_ID:
			readDeviceId();
			NetLevelCreateResponse(response_buffer, (char*)&the_device_id,
				sizeof(the_device_id), CMD_DEVICE_ID);
			responce = true;
		break;
		case CMD_GET_FIRMWARE_VERSION:
			NetLevelCreateResponse(response_buffer, firmvare_version,
				sizeof(firmvare_version), CMD_FIRMWARE_VERSION);
			responce = true;
		break;
	}

	if ( responce ) {
		uint16_t count;
		count = ((message_header_st*)response_buffer)->data_length +
			sizeof(message_header_st) + 1;
		UsbWrite((char*)response_buffer, count);
	}

}
///////////////////////////////////////////////////////////////////////////////


void readDeviceId() {
	
/*	
	FlashMemSegmentRead((char*)&the_device_id,
		sizeof(the_device_id), FLASH_DEVICEID_OFFSET);
	
	if ( the_device_id.device_class[0] == 0xff ) {
		the_device_id = (device_id_st){NUMBER_NAME_DEFAULT, NUMBER_DEVICE_DEFAULT};
		FlashMemSegmentWrite((char*)&the_device_id,
			sizeof(the_device_id), FLASH_DEVICEID_OFFSET);
	}
*/
	
	memset(&the_device_id, 0, sizeof(the_device_id));
	the_device_id = (device_id_st){NUMBER_NAME_DEFAULT, NUMBER_DEVICE_DEFAULT};
	
	the_device_id.device_class[DEVICE_ID_NAME_LENGTH-1] = 0;
	the_device_id.device_number[DEVICE_ID_NAME_LENGTH-1] = 0;
}
///////////////////////////////////////////////////////////////////////////////


