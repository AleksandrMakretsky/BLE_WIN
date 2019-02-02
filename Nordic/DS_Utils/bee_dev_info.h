/*
Copyright (C) 2013
*/

#ifndef _BEE_DEV_INFO_H_
#define _BEE_DEV_INFO_H_


#define DEVICE_NAME_ECG                   "BeeW"
#define DEVICE_NAME_SPIRO                 "BeeSp"
#define DEVICE_NAME_STETHO                "BeeSt"

#ifdef  ECG
#define NUMBER_NAME_DEFAULT               DEVICE_NAME_ECG
#elif   SPIRO
#define NUMBER_NAME_DEFAULT               DEVICE_NAME_ECG
#elif   OTG_LEADS_12
#define NUMBER_NAME_DEFAULT               DEVICE_NAME_ECG
#else
#define NUMBER_NAME_DEFAULT               "NoDevClass"
#endif

#define NUMBER_DEVICE_DEFAULT			  "0000"



#endif // _BEE_DEV_INFO_H_
