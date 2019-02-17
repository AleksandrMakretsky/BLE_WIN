#ifndef _HOST_INTERFASE_H_
#define _HOST_INTERFASE_H_

void addIncomingData(char data);
void hostInterfaseInit();
void hostInterfaseProcessPoll(bool _readyToSend);


typedef void (*ChannelWriteFn_t)(char*data, uint16_t dataLength);

extern ChannelWriteFn_t channelWriteFn;

#endif // _HOST_INTERFASE_H_
