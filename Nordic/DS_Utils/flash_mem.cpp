#include "stdafx.h"


#include <stdint.h>
#include <stdbool.h>
#include <string.h>
#include <sdk_errors.h>
#include "nrf_fstorage.h"

#include "nrf_log.h"
#include "nrf_log_ctrl.h"
#include "nrf_log_default_backends.h"


#include "flash_mem.h"


static void fstorage_evt_handler(nrf_fstorage_evt_t * p_evt);

NRF_FSTORAGE_DEF(nrf_fstorage_t fstorage) =
{
    /* Set a handler for fstorage events. */
    .evt_handler = fstorage_evt_handler,

    /* These below are the boundaries of the flash space assigned to this instance of fstorage.
     * You must set these manually, even at runtime, before nrf_fstorage_init() is called.
     * The function nrf5_flash_end_addr_get() can be used to retrieve the last address on the
     * last page of flash available to write data. */
    .start_addr = 0x3e000,
    .end_addr   = 0x3ffff,
};


void FlashMemRead(char* data, uint16_t length, uint32_t block_adr) {
	
	nrf_fstorage_read(&fstorage, 0x3e000, data, length);
	
}
/////////////////////////////////////////////////////////////////////////////


void FlashMemWrite(char* data, uint16_t length, uint32_t block_adr) {
	
//	ret_code_t rc;

//	rc = 
//		nrf_fstorage_write(&fstorage, 0x3e000, data, length, NULL);

	
}
/////////////////////////////////////////////////////////////////////////////

static void fstorage_evt_handler(nrf_fstorage_evt_t * p_evt)
{
    if (p_evt->result != NRF_SUCCESS)
    {
        NRF_LOG_INFO("--> Event received: ERROR while executing an fstorage operation.");
        return;
    }

    switch (p_evt->id)
    {
        case NRF_FSTORAGE_EVT_WRITE_RESULT:
        {
            NRF_LOG_INFO("--> Event received: wrote %d bytes at address 0x%x.",
                         p_evt->len, p_evt->addr);
        } break;

        case NRF_FSTORAGE_EVT_ERASE_RESULT:
        {
            NRF_LOG_INFO("--> Event received: erased %d page from address 0x%x.",
                         p_evt->len, p_evt->addr);
        } break;

        default:
            break;
    }
}
/////////////////////////////////////////////////////////////////////////////