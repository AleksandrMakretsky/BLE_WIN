#ifndef _USB_WRAPPER_H_
#define	_USB_WRAPPER_H_

#include <stdint.h>
#include "app_usbd.h"
#include "app_usbd_cdc_acm.h"

#include "boards.h"
#include "bsp.h"


/**
 * @brief Enable power USB detection
 *
 * Configure if example supports USB port connection
 */
#ifndef USBD_POWER_DETECTION
#define USBD_POWER_DETECTION true
#endif

#define CDC_ACM_COMM_INTERFACE  0
#define CDC_ACM_COMM_EPIN       NRF_DRV_USBD_EPIN2

#define CDC_ACM_DATA_INTERFACE  1
#define CDC_ACM_DATA_EPIN       NRF_DRV_USBD_EPIN1
#define CDC_ACM_DATA_EPOUT      NRF_DRV_USBD_EPOUT1

#define LED_USB_RESUME      (BSP_BOARD_LED_0)
#define LED_CDC_ACM_OPEN    (BSP_BOARD_LED_1)
#define LED_CDC_ACM_RX      (BSP_BOARD_LED_2)
#define LED_CDC_ACM_TX      (BSP_BOARD_LED_3)

#define BTN_CDC_DATA_SEND       0
#define BTN_CDC_NOTIFY_SEND     1

#define BTN_CDC_DATA_KEY_RELEASE        (bsp_event_t)(BSP_EVENT_KEY_LAST + 1)


#define READ_SIZE 1

extern bool m_send_flag;

	 
static void cdc_acm_user_ev_handler(app_usbd_class_inst_t const * p_inst,
                                    app_usbd_cdc_acm_user_event_t event);
	 

ret_code_t UsbInit();
void UsbRead(char* data, uint16_t length);
void UsbWrite(char* data, uint16_t length);
void bsp_event_callback(bsp_event_t ev);
void UsbTestWR(int frame_counter);


#endif // _USB_WRAPPER_H_
