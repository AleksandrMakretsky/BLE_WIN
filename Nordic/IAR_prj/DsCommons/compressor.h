/*
Beecardia firmware.
*/
#ifndef _COMPRESSOR_H_
#define _COMPRESSOR_H_

#include "types_porting.h"

#define COMPRESS_BUF_SIZE   256

#define ECG_RESULTING_BITS  12

/////////////////////////////////////////////////////////////////////////////
// Compressor interface
void CompressorInit(short channel_count);
void CompressorAddVector(short* ecg_vector);
void CompressorAddRaw32(__int32 data);
void CompressorPushRaw32();
void CompressorCreateCheckSum(char* buffer);
__int32 CompressorClose(void);
int GetCompressorDataLength(void);

#ifdef UNCOMPRESS_AVAILABLE
int CompressorUnpackBlock(unsigned short *pblock, short *p_ecg, int samples);
#endif

// call back prototype
extern void (* CompressorBlockReadyCallBack)(char* buffer);

#endif // _COMPRESSOR_H_
