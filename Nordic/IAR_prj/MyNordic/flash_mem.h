#ifndef _FLASH_MEM_H_
#define	_FLASH_MEM_H_

#include <stdint.h>
#include "nrf_fstorage.h"


#define FLASH_BLOCK0ADR     0x3e000
#define FLASH_BLOCK1ADR     0x3e100
#define FLASH_BLOCK2ADR     0x3e200
#define FLASH_BLOCK3ADR     0x3e300
#define FLASH_BLOCK4ADR     0x3e400
#define FLASH_BLOCK5ADR     0x3e500
#define FLASH_BLOCK6ADR     0x3e600
#define FLASH_BLOCK7ADR     0x3e700

#define ADR_DEV_ID          FLASH_BLOCK0ADR
#define ADR_VERSION         FLASH_BLOCK1ADR

ret_code_t FlashMemRead(char* data, uint16_t length, uint32_t block_adr);
ret_code_t FlashMemWrite(char* data, uint16_t length, uint32_t block_adr);
ret_code_t FlashMemErase(uint32_t block_adr);

#endif // _FLASH_MEM_H_
