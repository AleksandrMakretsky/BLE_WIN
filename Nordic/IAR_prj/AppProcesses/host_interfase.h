#ifndef _HOST_INTERFASE_H_
#define _HOST_INTERFASE_H_

void addIncomingData(char data);
void hostInterfaseInit();
void hostInterfaseProcessPoll(bool _readyToSend);


typedef void (*ChannelWriteFn_t)(char*data, uint16_t dataLength);

#define RESPONCE_BUFFER_LRNGTH     8
typedef struct
{
	message_header_st messageHeader;
	char data_block_buffer[512+24];
} Response;


extern ChannelWriteFn_t channelWriteFn;

#endif // _HOST_INTERFASE_H_
