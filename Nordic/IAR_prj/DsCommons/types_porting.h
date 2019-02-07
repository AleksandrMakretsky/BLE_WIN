// int32 and bool support

#ifndef _TYPES_PORTING_H_
#define _TYPES_PORTING_H_

#ifdef __GNUC__
#include <cstdlib>
#include "stdint.h"
#define __int32 int32_t
#define __int64 int64_t
#endif // __GNUC__

#ifdef __IAR_SYSTEMS_ICC__
#define __int32 long
#define __int64 long long

#ifndef    bool
#define    bool    short
#endif

#ifndef    false
#define    false   0
#endif

#ifndef    true
#define    true    1
#endif

#endif  // __IAR_SYSTEMS_ICC__ // bool support in IAR

typedef struct {
	int  x;
	int  y;
} point_st;
/////////////////////////////////////////////////////////////////////////////

// declare mul_div.
inline int mul_div(int number, int numerator, int denominator);
// then it's body
inline int mul_div(int number, int numerator, int denominator) {
    __int64 ret = number;
    ret *= numerator;
    ret /= denominator;
    return (int) ret;
}
/////////////////////////////////////////////////////////////////////////////

#ifndef _MSC_VER
 #ifndef   INT_MAX
 #define   INT_MAX 2147483647
 #endif // INT_MAX

 #ifndef   INT_MIN
 #define   INT_MIN (-2147483647-1)
 #endif
#else // win version
 #include <limits.h>
#endif  // _MSC_VER

#ifndef   SHORT_MAX
 #define   SHORT_MAX 32767
#endif // SHORT_MAX

#ifndef   SHORT_MIN
 #define   SHORT_MIN -32768
#endif // SHORT_MIN

#ifndef   BYTE_MAX
 #define   BYTE_MAX  255
#endif // BYTE_MAX

#ifndef MAX_ECG_LEADS_COUNT // should be define in the top level module (fix me later)
 #define MAX_ECG_LEADS_COUNT   16
#endif

#ifndef MAX_CHANNEL_COUNT
 #define MAX_CHANNEL_COUNT     8
#endif

void short_to_int(int* i_valueas, short* s_values, int count);
unsigned short bytes2short(char lsb, char msb);
void short2bytes(char* p_array, short value);

#endif // _TYPES_PORTING_H_
