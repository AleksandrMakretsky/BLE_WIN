// from 
// ..\nRF5_SDK_15.2.0_9412b96\examples\ble_central\ble_app_uart_c
/*
led1 on = connect ble
led1 2000 no connect ble
led3 on = usb com connect
led3 off = no usb
led3 800 = usb not connected

scan filter
*/

#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>
//------------------------------------------------------------------------------

#include "nordic_common.h"
#include "app_error.h"
#include "app_uart.h"
#include "ble_db_discovery.h"
#include "app_timer.h"
#include "app_util.h"
#include "bsp_btn_ble.h"
#include "ble.h"
#include "ble_gap.h"
#include "ble_hci.h"
#include "nrf_sdh.h"
#include "nrf_sdh_ble.h"
#include "nrf_sdh_soc.h"
#include "ble_nus_c.h"
#include "nrf_ble_gatt.h"
#include "nrf_pwr_mgmt.h"
#include "nrf_ble_scan.h"
#include "nrf_drv_clock.h"
#include "nrf_drv_usbd.h"
#include "nrf_gpio.h"
#include "nrf_delay.h"
#include "nrf_drv_power.h"
//------------------------------------------------------------------------------

#include "app_util.h"
#include "app_usbd_core.h"
#include "app_usbd.h"
#include "app_usbd_string_desc.h"
#include "app_usbd_cdc_acm.h"
#include "app_usbd_cdc_acm_internal.h"
#include "app_usbd_serial_num.h" 
//--------------------------------------

#include "nrf_log.h"
#include "nrf_log_ctrl.h"
#include "nrf_log_default_backends.h"
////////////////////////////////////////////////////////////////////////////////

//------------------------------------------------------------------------------
char version_name[] = "UsbBle bridge V1.001";
//------------------------------------------------------------------------------
// app vars
#define RX_BUFFER_SIZE 256
char m_rx_usb[RX_BUFFER_SIZE];
char m_tx_usb[RX_BUFFER_SIZE];
char m_rx_ble[RX_BUFFER_SIZE];

uint16_t in_usb_index = 0;
uint16_t out_usb_index = 0;
uint16_t usb_buffer_length = 0;

char m_tx_ble[RX_BUFFER_SIZE];
uint16_t in_ble_index = 0;
uint16_t out_ble_index = 0;
uint16_t ble_buffer_length = 0;

bool usbReadyToSend = true;
bool bleReadyToSend = true;
bool isConnected = false;
//------------------------------------------------------------------------------

// app prototypes
void CheckIncommingData();
void ChannelWriteUsb(char*data, uint16_t dataLength);
void channelWriteBle(uint8_t* data, uint16_t dataLength);
void initRxTxbuffers();
//------------------------------------------------------------------------------


#define NUS_SERVICE_UUID_TYPE BLE_UUID_TYPE_VENDOR_BEGIN /**< UUID type for the Nordic UART Service (vendor specific). */

APP_TIMER_DEF(m_blink_ble);
APP_TIMER_DEF(m_blink_cdc);

#define LED_BLE_NUS_CONN (BSP_BOARD_LED_0)
#define LED_BLE_NUS_RX   (BSP_BOARD_LED_1)
#define LED_CDC_ACM_CONN (BSP_BOARD_LED_2)
#define LED_CDC_ACM_RX   (BSP_BOARD_LED_3)

#define LED_BLINK_INTERVAL 800

#define ENDLINE_STRING "\r\n"

// USB DEFINES START
static void cdc_acm_user_ev_handler(app_usbd_class_inst_t const * p_inst,
                                    app_usbd_cdc_acm_user_event_t event);

#define CDC_ACM_COMM_INTERFACE  0
#define CDC_ACM_COMM_EPIN       NRF_DRV_USBD_EPIN2

#define CDC_ACM_DATA_INTERFACE  1
#define CDC_ACM_DATA_EPIN       NRF_DRV_USBD_EPIN1
#define CDC_ACM_DATA_EPOUT      NRF_DRV_USBD_EPOUT1


/** @brief CDC_ACM class instance */
APP_USBD_CDC_ACM_GLOBAL_DEF(m_app_cdc_acm,
                            cdc_acm_user_ev_handler,
                            CDC_ACM_COMM_INTERFACE,
                            CDC_ACM_DATA_INTERFACE,
                            CDC_ACM_COMM_EPIN,
                            CDC_ACM_DATA_EPIN,
                            CDC_ACM_DATA_EPOUT,
                            APP_USBD_CDC_COMM_PROTOCOL_AT_V250);
#define DS_OPCODE_LENGTH 1
#define DS_HANDLE_LENGTH 2
#define DS_BLE_NUS_MAX_DATA_LEN (NRF_SDH_BLE_GATT_MAX_MTU_SIZE - DS_OPCODE_LENGTH - DS_HANDLE_LENGTH)
                            
static char m_cdc_data_array[DS_BLE_NUS_MAX_DATA_LEN];

#define APP_BLE_CONN_CFG_TAG    1                                       /**< Tag that refers to the BLE stack configuration set with @ref sd_ble_cfg_set. The default tag is @ref BLE_CONN_CFG_TAG_DEFAULT. */
#define APP_BLE_OBSERVER_PRIO   3                                       /**< BLE observer priority of the application. There is no need to modify this value. */

#define UART_TX_BUF_SIZE       256                                     /**< UART TX buffer size. */
#define UART_RX_BUF_SIZE       256                                     /**< UART RX buffer size. */

#define NUS_SERVICE_UUID_TYPE   BLE_UUID_TYPE_VENDOR_BEGIN              /**< UUID type for the Nordic UART Service (vendor specific). */

#define ECHOBACK_BLE_UART_DATA  1                                       /**< Echo the UART data that is received over the Nordic UART Service (NUS) back to the sender. */


BLE_NUS_C_DEF(m_ble_nus_c);                                             /**< BLE Nordic UART Service (NUS) client instance. */
NRF_BLE_GATT_DEF(m_gatt);                                               /**< GATT module instance. */
BLE_DB_DISCOVERY_DEF(m_db_disc);                                        /**< Database discovery module instance. */
NRF_BLE_SCAN_DEF(m_scan);                                               /**< Scanning Module instance. */

static uint16_t m_ble_nus_max_data_len = BLE_GATT_ATT_MTU_DEFAULT - OPCODE_LENGTH - HANDLE_LENGTH; /**< Maximum length of data (in bytes) that can be transmitted to the peer by the Nordic UART service module. */

/**@brief NUS UUID. */
static ble_uuid_t const m_nus_uuid =
{
    .uuid = BLE_UUID_NUS_SERVICE,
    .type = NUS_SERVICE_UUID_TYPE
};


/**@brief Function for handling asserts in the SoftDevice.
 *
 * @details This function is called in case of an assert in the SoftDevice.
 *
 * @warning This handler is only an example and is not meant for the final product. You need to analyze
 *          how your product is supposed to react in case of assert.
 * @warning On assert from the SoftDevice, the system can only recover on reset.
 *
 * @param[in] line_num     Line number of the failing assert call.
 * @param[in] p_file_name  File name of the failing assert call.
 */
void assert_nrf_callback(uint16_t line_num, const uint8_t * p_file_name)
{
    app_error_handler(0xDEADBEEF, line_num, p_file_name);
}


/**@brief Function for starting scanning. */
static void scan_start(void)
{
    ret_code_t ret;
    ret = nrf_ble_scan_start(&m_scan);
    APP_ERROR_CHECK(ret);

    ret = bsp_indication_set(BSP_INDICATE_SCANNING);
    APP_ERROR_CHECK(ret);
}


/**@brief Function for handling Scanning Module events.
 */
static void scan_evt_handler(scan_evt_t const * p_scan_evt)
{
    ret_code_t err_code;

    switch(p_scan_evt->scan_evt_id)
    {
         case NRF_BLE_SCAN_EVT_CONNECTING_ERROR:
         {
        
    printf("scan_evt_handler() : NRF_BLE_SCAN_EVT_CONNECTING_ERROR:.\r\n");      

           err_code = p_scan_evt->params.connecting_err.err_code;
              APP_ERROR_CHECK(err_code);
         } break;

         case NRF_BLE_SCAN_EVT_CONNECTED:
         {
   printf("scan_evt_handler() : NRF_BLE_SCAN_EVT_CONNECTED:.\r\n"); 
   isConnected=1;
           //NRF_LOG_INFO("scan_evt_handler() : NRF_BLE_SCAN_EVT_CONNECTED:");
           
              ble_gap_evt_connected_t const * p_connected =
                               p_scan_evt->params.connected.p_connected;
             // Scan is automatically stopped by the connection.
             NRF_LOG_INFO("Connecting to target %02x%02x%02x%02x%02x%02x",
                      p_connected->peer_addr.addr[0],
                      p_connected->peer_addr.addr[1],
                      p_connected->peer_addr.addr[2],
                      p_connected->peer_addr.addr[3],
                      p_connected->peer_addr.addr[4],
                      p_connected->peer_addr.addr[5]
                      );
         } break;

         case NRF_BLE_SCAN_EVT_SCAN_TIMEOUT:
         {
             NRF_LOG_INFO("Scan timed out.");
             scan_start();
         } break;

		case NRF_BLE_SCAN_EVT_FILTER_MATCH:
			{
             	NRF_LOG_INFO("Scan timed out.");
				   isConnected = 0;
			} break;
			
         default:
             break;
    }
}


/**@brief Function for initializing the scanning and setting the filters.
 */
static void scan_init(void)
{
    ret_code_t          err_code;
    nrf_ble_scan_init_t init_scan;

    memset(&init_scan, 0, sizeof(init_scan));

    init_scan.connect_if_match = true;
    init_scan.conn_cfg_tag     = APP_BLE_CONN_CFG_TAG;

    err_code = nrf_ble_scan_init(&m_scan, &init_scan, scan_evt_handler);
    APP_ERROR_CHECK(err_code);

    err_code = nrf_ble_scan_filter_set(&m_scan, SCAN_UUID_FILTER, &m_nus_uuid);
    APP_ERROR_CHECK(err_code);

    err_code = nrf_ble_scan_filters_enable(&m_scan, NRF_BLE_SCAN_UUID_FILTER, false);
    APP_ERROR_CHECK(err_code);
}


/**@brief Function for handling database discovery events.
 *
 * @details This function is a callback function to handle events from the database discovery module.
 *          Depending on the UUIDs that are discovered, this function forwards the events
 *          to their respective services.
 *
 * @param[in] p_event  Pointer to the database discovery event.
 */
static void db_disc_handler(ble_db_discovery_evt_t * p_evt)
{
    ble_nus_c_on_db_disc_evt(&m_ble_nus_c, p_evt);
}


/**@brief Function for handling characters received by the Nordic UART Service (NUS).
 *
 * @details This function takes a list of characters of length data_len and prints the characters out on UART.
 *          If @ref ECHOBACK_BLE_UART_DATA is set, the data is sent back to sender.
 */

static void ble_nus_chars_received_uart_print(uint8_t * p_data, uint16_t data_len)
{
   // ret_code_t ret_val;

    NRF_LOG_DEBUG("Receiving data.");
    NRF_LOG_HEXDUMP_DEBUG(p_data, data_len);
    


//ble_buffer_length = data_len;
//ptr_data_rx_ble = p_data;

printf("ble_nus_chars_received_uart_print().data_len=%d\r\n", data_len);

printf("ble_nus_chars_received_uart_print().b0=%02X b1=%02X b2=%02X b3=%02X\r\n", 
       p_data[0], p_data[1], p_data[2], p_data[3]);

	app_usbd_cdc_acm_write(&m_app_cdc_acm, p_data, data_len);
	usbReadyToSend = false;
/*
    for (uint32_t i = 0; i < data_len; i++)
    {
        do
        {
            ret_val = app_uart_put(p_data[i]);
            ChannelWriteUsb( (char* )p_data[i], 1);
            
            if ((ret_val != NRF_SUCCESS) && (ret_val != NRF_ERROR_BUSY))
            {
                NRF_LOG_ERROR("app_uart_put failed for index 0x%04x.", i);
                APP_ERROR_CHECK(ret_val);
            }
        } while (ret_val == NRF_ERROR_BUSY);
    }
    if (p_data[data_len-1] == '\r')
    {
        while (app_uart_put('\n') == NRF_ERROR_BUSY);
    }
  */  
   
    /*
    if (ECHOBACK_BLE_UART_DATA)
    {
printf(" Send data back to the peripheral.\r\n");      
        // Send data back to the peripheral.
        do
        {
            ret_val = ble_nus_c_string_send(&m_ble_nus_c, p_data, data_len);
            if ((ret_val != NRF_SUCCESS) && (ret_val != NRF_ERROR_BUSY))
            {
                NRF_LOG_ERROR("Failed sending NUS message. Error 0x%x. ", ret_val);
                APP_ERROR_CHECK(ret_val);
            }
        } while (ret_val == NRF_ERROR_BUSY);
    }*/
}


/**@brief   Function for handling app_uart events.
 *
 * @details This function receives a single character from the app_uart module and appends it to
 *          a string. The string is sent over BLE when the last character received is a
 *          'new line' '\n' (hex 0x0A) or if the string reaches the maximum data length.
 */
void uart_event_handle(app_uart_evt_t * p_event)
{
    static uint8_t data_array[BLE_NUS_MAX_DATA_LEN];
    static uint16_t index = 0;
    uint32_t ret_val;

    switch (p_event->evt_type)
    {
        /**@snippet [Handling data from UART] */
        case APP_UART_DATA_READY:
            UNUSED_VARIABLE(app_uart_get(&data_array[index]));
            index++;

            if ((data_array[index - 1] == '\n') || (index >= (m_ble_nus_max_data_len)))
            {
               // NRF_LOG_DEBUG("Ready to send data over BLE NUS");
                //NRF_LOG_HEXDUMP_DEBUG(data_array, index);
                
              printf("APP_UART_DATA_READY:.\r\n");

                do
                {
                    ret_val = ble_nus_c_string_send(&m_ble_nus_c, data_array, index);
                    if ( (ret_val != NRF_ERROR_INVALID_STATE) && (ret_val != NRF_ERROR_RESOURCES) )
                    {
                        APP_ERROR_CHECK(ret_val);
                    }
                } while (ret_val == NRF_ERROR_RESOURCES);

                index = 0;
            }
            break;

        /**@snippet [Handling data from UART] */
        case APP_UART_COMMUNICATION_ERROR:
         
            NRF_LOG_ERROR("Communication error occurred while handling UART.");
            APP_ERROR_HANDLER(p_event->data.error_communication);
            break;

        case APP_UART_FIFO_ERROR:
            NRF_LOG_ERROR("Error occurred in FIFO module used by UART.");
            APP_ERROR_HANDLER(p_event->data.error_code);
            break;

        default:
            break;
    }
}


/**@brief Callback handling Nordic UART Service (NUS) client events.
 *
 * @details This function is called to notify the application of NUS client events.
 *
 * @param[in]   p_ble_nus_c   NUS client handle. This identifies the NUS client.
 * @param[in]   p_ble_nus_evt Pointer to the NUS client event.
 */

/**@snippet [Handling events from the ble_nus_c module] */
static void ble_nus_c_evt_handler(ble_nus_c_t * p_ble_nus_c, ble_nus_c_evt_t const * p_ble_nus_evt)
{
    ret_code_t err_code;

    switch (p_ble_nus_evt->evt_type)
    {
        case BLE_NUS_C_EVT_DISCOVERY_COMPLETE:
            NRF_LOG_INFO("Discovery complete.");
            err_code = ble_nus_c_handles_assign(p_ble_nus_c, p_ble_nus_evt->conn_handle, &p_ble_nus_evt->handles);
            APP_ERROR_CHECK(err_code);

            err_code = ble_nus_c_tx_notif_enable(p_ble_nus_c);
            APP_ERROR_CHECK(err_code);
            NRF_LOG_INFO("Connected to device with Nordic UART Service.");
            break;

        case BLE_NUS_C_EVT_NUS_TX_EVT:
printf("ble_nus_c_evt_handler() : case BLE_NUS_C_EVT_NUS_TX_EVT:len=%d\r\n", p_ble_nus_evt->data_len);          
            ble_nus_chars_received_uart_print(p_ble_nus_evt->p_data, p_ble_nus_evt->data_len);
            break;
        case BLE_NUS_C_EVT_NUS_RX_EVT:
printf("ble_nus_c_evt_handler() : case BLE_NUS_C_EVT_NUS_RX_EVT:len=%d\r\n", p_ble_nus_evt->data_len);          
            //ble_nus_chars_received_uart_print(p_ble_nus_evt->p_data, p_ble_nus_evt->data_len);
            break;
        case BLE_NUS_C_EVT_DISCONNECTED:
             isConnected=0;
            NRF_LOG_INFO("Disconnected.");
            scan_start();
            break;
    }
}
/**@snippet [Handling events from the ble_nus_c module] */


/**
 * @brief Function for handling shutdown events.
 *
 * @param[in]   event       Shutdown type.
 */
static bool shutdown_handler(nrf_pwr_mgmt_evt_t event)
{
    ret_code_t err_code;

    err_code = bsp_indication_set(BSP_INDICATE_IDLE);
    APP_ERROR_CHECK(err_code);

    switch (event)
    {
        case NRF_PWR_MGMT_EVT_PREPARE_WAKEUP:
            // Prepare wakeup buttons.
            err_code = bsp_btn_ble_sleep_mode_prepare();
            APP_ERROR_CHECK(err_code);
            break;

        default:
            break;
    }

    return true;
}

NRF_PWR_MGMT_HANDLER_REGISTER(shutdown_handler, APP_SHUTDOWN_HANDLER_PRIORITY);


/**@brief Function for handling BLE events.
 *
 * @param[in]   p_ble_evt   Bluetooth stack event.
 * @param[in]   p_context   Unused.
 */
static void ble_evt_handler(ble_evt_t const * p_ble_evt, void * p_context)
{
    ret_code_t            err_code;
    ble_gap_evt_t const * p_gap_evt = &p_ble_evt->evt.gap_evt;

    switch (p_ble_evt->header.evt_id)
    {
        case BLE_GAP_EVT_CONNECTED:
printf("ble_evt_handler() : case BLE_GAP_EVT_CONNECTED:\r\n");
          NRF_LOG_INFO("ble_evt_handler() : case BLE_GAP_EVT_CONNECTED:");
          
            err_code = ble_nus_c_handles_assign(&m_ble_nus_c, p_ble_evt->evt.gap_evt.conn_handle, NULL);
            APP_ERROR_CHECK(err_code);

            err_code = bsp_indication_set(BSP_INDICATE_CONNECTED);
            APP_ERROR_CHECK(err_code);

            // start discovery of services. The NUS Client waits for a discovery result
            err_code = ble_db_discovery_start(&m_db_disc, p_ble_evt->evt.gap_evt.conn_handle);
            APP_ERROR_CHECK(err_code);
            break;

        case BLE_GAP_EVT_DISCONNECTED:
printf("Disconnected. conn_handle: BLE_GAP_EVT_DISCONNECTED:\r\n");
            NRF_LOG_INFO("Disconnected. conn_handle: 0x%x, reason: 0x%x",
                         p_gap_evt->conn_handle,
                         p_gap_evt->params.disconnected.reason);
            break;

        case BLE_GAP_EVT_TIMEOUT:
            if (p_gap_evt->params.timeout.src == BLE_GAP_TIMEOUT_SRC_CONN)
            {
                NRF_LOG_INFO("Connection Request timed out.");
            }
            break;

        case BLE_GAP_EVT_SEC_PARAMS_REQUEST:
            // Pairing not supported.
            err_code = sd_ble_gap_sec_params_reply(p_ble_evt->evt.gap_evt.conn_handle, BLE_GAP_SEC_STATUS_PAIRING_NOT_SUPP, NULL, NULL);
            APP_ERROR_CHECK(err_code);
            break;

        case BLE_GAP_EVT_CONN_PARAM_UPDATE_REQUEST:
            // Accepting parameters requested by peer.
            err_code = sd_ble_gap_conn_param_update(p_gap_evt->conn_handle,
                                                    &p_gap_evt->params.conn_param_update_request.conn_params);
            APP_ERROR_CHECK(err_code);
            break;

        case BLE_GAP_EVT_PHY_UPDATE_REQUEST:
        {
            NRF_LOG_DEBUG("PHY update request.");
            ble_gap_phys_t const phys =
            {
                .rx_phys = BLE_GAP_PHY_AUTO,
                .tx_phys = BLE_GAP_PHY_AUTO,
            };
            err_code = sd_ble_gap_phy_update(p_ble_evt->evt.gap_evt.conn_handle, &phys);
            APP_ERROR_CHECK(err_code);
        } break;

        case BLE_GATTC_EVT_TIMEOUT:
            // Disconnect on GATT Client timeout event.
            NRF_LOG_DEBUG("GATT Client Timeout.");
            err_code = sd_ble_gap_disconnect(p_ble_evt->evt.gattc_evt.conn_handle,
                                             BLE_HCI_REMOTE_USER_TERMINATED_CONNECTION);
            APP_ERROR_CHECK(err_code);
            break;

        case BLE_GATTS_EVT_TIMEOUT:
            // Disconnect on GATT Server timeout event.
            NRF_LOG_DEBUG("GATT Server Timeout.");
            err_code = sd_ble_gap_disconnect(p_ble_evt->evt.gatts_evt.conn_handle,
                                             BLE_HCI_REMOTE_USER_TERMINATED_CONNECTION);
            APP_ERROR_CHECK(err_code);
            break;

        default:
            break;
    }
}


/**@brief Function for initializing the BLE stack.
 *
 * @details Initializes the SoftDevice and the BLE event interrupt.
 */
static void ble_stack_init(void)
{
    ret_code_t err_code;

    err_code = nrf_sdh_enable_request();
    APP_ERROR_CHECK(err_code);

    // Configure the BLE stack using the default settings.
    // Fetch the start address of the application RAM.
    uint32_t ram_start = 0;
    err_code = nrf_sdh_ble_default_cfg_set(APP_BLE_CONN_CFG_TAG, &ram_start);
    APP_ERROR_CHECK(err_code);

    // Enable BLE stack.
    err_code = nrf_sdh_ble_enable(&ram_start);
    APP_ERROR_CHECK(err_code);

    // Register a handler for BLE events.
    NRF_SDH_BLE_OBSERVER(m_ble_observer, APP_BLE_OBSERVER_PRIO, ble_evt_handler, NULL);
}


/**@brief Function for handling events from the GATT library. */
void gatt_evt_handler(nrf_ble_gatt_t * p_gatt, nrf_ble_gatt_evt_t const * p_evt)
{
    if (p_evt->evt_id == NRF_BLE_GATT_EVT_ATT_MTU_UPDATED)
    {
        NRF_LOG_INFO("ATT MTU exchange completed.");
printf("gatt_evt_handler():ATT MTU exchange completed.\r\n");


        m_ble_nus_max_data_len = p_evt->params.att_mtu_effective - OPCODE_LENGTH - HANDLE_LENGTH;
        NRF_LOG_INFO("Ble NUS max data length set to 0x%X(%d)", m_ble_nus_max_data_len, m_ble_nus_max_data_len);
    }
}


/**@brief Function for initializing the GATT library. */
void gatt_init(void)
{
    ret_code_t err_code;

    err_code = nrf_ble_gatt_init(&m_gatt, gatt_evt_handler);
    APP_ERROR_CHECK(err_code);

    err_code = nrf_ble_gatt_att_mtu_central_set(&m_gatt, NRF_SDH_BLE_GATT_MAX_MTU_SIZE);
    APP_ERROR_CHECK(err_code);
}


/**@brief Function for handling events from the BSP module.
 *
 * @param[in] event  Event generated by button press.
 */
void bsp_event_handler(bsp_event_t event)
{
    ret_code_t err_code;

    switch (event)
    {
        case BSP_EVENT_SLEEP:
            nrf_pwr_mgmt_shutdown(NRF_PWR_MGMT_SHUTDOWN_GOTO_SYSOFF);
            break;

        case BSP_EVENT_DISCONNECT:
            err_code = sd_ble_gap_disconnect(m_ble_nus_c.conn_handle,
                                             BLE_HCI_REMOTE_USER_TERMINATED_CONNECTION);
            if (err_code != NRF_ERROR_INVALID_STATE)
            {
                APP_ERROR_CHECK(err_code);
            }
            break;

        default:
            break;
    }
}

/**@brief Function for initializing the UART. */
static void uart_init(void)
{
    ret_code_t err_code;

    app_uart_comm_params_t const comm_params =
    {
        .rx_pin_no    = RX_PIN_NUMBER,
        .tx_pin_no    = TX_PIN_NUMBER,
        .rts_pin_no   = RTS_PIN_NUMBER,
        .cts_pin_no   = CTS_PIN_NUMBER,
        .flow_control = APP_UART_FLOW_CONTROL_DISABLED,
        .use_parity   = false,
        .baud_rate    = UART_BAUDRATE_BAUDRATE_Baud115200
    };

    APP_UART_FIFO_INIT(&comm_params,
                       UART_RX_BUF_SIZE,
                       UART_TX_BUF_SIZE,
                       uart_event_handle,
                       APP_IRQ_PRIORITY_LOWEST,
                       err_code);

    APP_ERROR_CHECK(err_code);
}

/**@brief Function for initializing the Nordic UART Service (NUS) client. */
static void nus_c_init(void)
{
    ret_code_t       err_code;
    ble_nus_c_init_t init;

    init.evt_handler = ble_nus_c_evt_handler;

    err_code = ble_nus_c_init(&m_ble_nus_c, &init);
    APP_ERROR_CHECK(err_code);
}


/**@brief Function for initializing buttons and leds. */
static void buttons_leds_init(void)
{
    ret_code_t err_code;
    bsp_event_t startup_event;

    err_code = bsp_init(BSP_INIT_LEDS, bsp_event_handler);
    APP_ERROR_CHECK(err_code);

    err_code = bsp_btn_ble_init(NULL, &startup_event);
    APP_ERROR_CHECK(err_code);
}


/**@brief Function for initializing the timer. */
/*
static void timer_init(void)
{
    ret_code_t err_code = app_timer_init();
    APP_ERROR_CHECK(err_code);
}
*/

/**@brief Function for initializing the nrf log module. */
static void log_init(void)
{
    ret_code_t err_code = NRF_LOG_INIT(NULL);
    APP_ERROR_CHECK(err_code);

    NRF_LOG_DEFAULT_BACKENDS_INIT();
}
/////////////////////////////////////////////////////////////////////////////

// USB CODE START
static bool m_usb_connected = false;
void blink_handler(void * p_context)
{
    bsp_board_led_invert((uint32_t) p_context);
}
/////////////////////////////////////////////////////////////////////////////


void usbd_user_ev_handler(app_usbd_event_type_t event)
{
    switch (event)
    {
        case APP_USBD_EVT_DRV_SUSPEND:
            break;

        case APP_USBD_EVT_DRV_RESUME:
            break;

        case APP_USBD_EVT_STARTED:
            break;

        case APP_USBD_EVT_STOPPED:
            app_usbd_disable();
            break;

        case APP_USBD_EVT_POWER_DETECTED:
            NRF_LOG_INFO("USB power detected");

            if (!nrf_drv_usbd_is_enabled())
            {
                app_usbd_enable();
            }
            break;

        case APP_USBD_EVT_POWER_REMOVED:
        {
            NRF_LOG_INFO("USB power removed");
            ret_code_t err_code = app_timer_stop(m_blink_cdc);
            APP_ERROR_CHECK(err_code);
            //bsp_board_led_off(LED_CDC_ACM_CONN);
            m_usb_connected = false;
            app_usbd_stop();
        }
            break;

        case APP_USBD_EVT_POWER_READY:
        {
            NRF_LOG_INFO("USB ready");
            ret_code_t err_code = app_timer_start(m_blink_cdc,
                                                  APP_TIMER_TICKS(LED_BLINK_INTERVAL),
                                                  (void *) LED_CDC_ACM_CONN);
            APP_ERROR_CHECK(err_code);
            m_usb_connected = true;
            app_usbd_start();
        }
            break;

        default:
            break;
    }
}
/////////////////////////////////////////////////////////////////////////////


/** @brief User event handler @ref app_usbd_cdc_acm_user_ev_handler_t */
static void cdc_acm_user_ev_handler(app_usbd_class_inst_t const * p_inst,
                                    app_usbd_cdc_acm_user_event_t event)
{
  
    app_usbd_cdc_acm_t const * p_cdc_acm = app_usbd_cdc_acm_class_get(p_inst);

    switch (event)
    {
        case APP_USBD_CDC_ACM_USER_EVT_PORT_OPEN:
        {
            /*Set up the first transfer*/
          ret_code_t ret;
            ret = app_usbd_cdc_acm_read(&m_app_cdc_acm,
                                                   m_cdc_data_array,
                                                   1);
         
         
            UNUSED_VARIABLE(ret);
            
            ret = app_timer_stop(m_blink_cdc);
            APP_ERROR_CHECK(ret);
            bsp_board_led_on(LED_CDC_ACM_CONN);
            NRF_LOG_INFO("CDC ACM port opened");
            break;
        }

        case APP_USBD_CDC_ACM_USER_EVT_PORT_CLOSE:
            NRF_LOG_INFO("CDC ACM port closed");
            if (m_usb_connected)
            {
                ret_code_t ret = app_timer_start(m_blink_cdc,
                                                 APP_TIMER_TICKS(LED_BLINK_INTERVAL),
                                                 (void *) LED_CDC_ACM_CONN);
                APP_ERROR_CHECK(ret);
            }
            break;

        case APP_USBD_CDC_ACM_USER_EVT_TX_DONE:
			usbReadyToSend = true;
            break;

        case APP_USBD_CDC_ACM_USER_EVT_RX_DONE:
          { // need it for vars declaration inside
            ret_code_t ret;
            size_t size = 0;
                        
            do
            {
                  size++;
                  m_rx_usb[in_usb_index++] = m_cdc_data_array[0];
                  in_usb_index &= (RX_BUFFER_SIZE-1);
                  usb_buffer_length++;

                // Fetch data until internal buffer is empty
                ret = app_usbd_cdc_acm_read(&m_app_cdc_acm, &m_cdc_data_array[0], 1);
            } while (ret == NRF_SUCCESS);
			
			//NRF_LOG_INFO("Usb got bytes: %lu ", size);
printf("APP_USBD_CDC_ACM_USER_EVT_RX_DONE:Usb got bytes: %lu.\r\n", size);                        
            break;
          }
        default:
            break;
    }
}
/////////////////////////////////////////////////////////////////////////////


void ChannelWriteUsb(char*data, uint16_t dataLength) {

	//app_usbd_cdc_acm_write(&m_app_cdc_acm, data, dataLength);
	usbReadyToSend = false;
}
////////////////////////////////////////////////////////////////////////////////


void channelWriteBle(uint8_t* data, uint16_t dataLength) {

//	app_usbd_cdc_acm_write(&m_app_cdc_acm, data, dataLength);
        //ble_nus_data_send(&m_nus, (uint8_t *)data, &dataLength, m_conn_handle_br_c);//m_conn_handle);
	//NRF_LOG_INFO("BLE channelWriteBle bytes count: %d", dataLength);


 printf("channelWriteBle():ble_nus_c_string_send.\r\n");

        //ble_nus_c_string_send(&m_ble_nus_c, (uint8_t *)data, dataLength);
 ble_nus_c_string_send(&m_ble_nus_c, data, dataLength);
        bleReadyToSend = false;
	// finish can be in the on_hvx_tx_complete()
}
////////////////////////////////////////////////////////////////////////////////

void CheckIncommingData() {
	
	//if ( !isConnected ) return;
	
	// check USB incoming data
	uint16_t count = 0;
	while ( usb_buffer_length > 0 ) {
		m_tx_ble[count++] = m_rx_usb[out_usb_index];
		//addIncomingData(m_rx_buffer_fifo[out_index]);
		out_usb_index = (out_usb_index+1) & (RX_BUFFER_SIZE-1);
		usb_buffer_length--;
	}
        
	if ( count ) channelWriteBle((uint8_t* )m_tx_ble, count);
	
	// check BLE incoming data
	count = 0;
        
        if(ble_buffer_length)
        {
          printf("CheckIncommingData():ble_buffer_length=%d \r\n", ble_buffer_length);
        }
        
	while ( ble_buffer_length > 0 ) {
              m_tx_usb[count++] = m_rx_ble[out_usb_index];
		//m_tx_usb[count++] = *(ptr_data_rx_ble+out_ble_index);//m_rx_ble[out_usb_index];
		//addIncomingData(m_rx_buffer_fifo[out_index]);
		out_ble_index = (out_ble_index+1) & (RX_BUFFER_SIZE-1);
		ble_buffer_length--;
	}
	if ( count ) ChannelWriteUsb(m_tx_usb, count);
  
	
}
/////////////////////////////////////////////////////////////////////////////

/**@brief Function for initializing power management.
 */
/*
static void power_management_init(void)
{
    ret_code_t err_code;
    err_code = nrf_pwr_mgmt_init();
    APP_ERROR_CHECK(err_code);
}
*/

/**@brief Function for handling the idle state (main loop). If there is no pending log operation,
          then sleeps until the next event occurs.
 */
/*
static void idle_state_handle(void)
{
    if (NRF_LOG_PROCESS() == false)
    {
        nrf_pwr_mgmt_run();
    }
}
*/

/** @brief Function for initializing the timer module. */

static void timers_init(void)
{
    ret_code_t err_code = app_timer_init();
    APP_ERROR_CHECK(err_code);
    err_code = app_timer_create(&m_blink_ble, APP_TIMER_MODE_REPEATED, blink_handler);
    APP_ERROR_CHECK(err_code);
    err_code = app_timer_create(&m_blink_cdc, APP_TIMER_MODE_REPEATED, blink_handler);
    APP_ERROR_CHECK(err_code);
}
////////////////////////////////////////////////////////////////////////////////


void initRxTxbuffers() {

	in_usb_index = 0;
	out_usb_index = 0;
	usb_buffer_length = 0;

	in_ble_index = 0;
	out_ble_index = 0;
	ble_buffer_length = 0;

	isConnected = 0;
}
////////////////////////////////////////////////////////////////////////////////


/**@brief Function for initializing power management.
 */
static void power_management_init(void)
{
    ret_code_t err_code;
    err_code = nrf_pwr_mgmt_init();
    APP_ERROR_CHECK(err_code);
}


/** @brief Function for initializing the database discovery module. */
static void db_discovery_init(void)
{
    ret_code_t err_code = ble_db_discovery_init(db_disc_handler);
    APP_ERROR_CHECK(err_code);
}


/**@brief Function for handling the idle state (main loop).
 *
 * @details Handles any pending log operations, then sleeps until the next event occurs.
 */
static void idle_state_handle(void)
{
    if (NRF_LOG_PROCESS() == false)
    {
        nrf_pwr_mgmt_run();
    }
}

////////////////////////////////////////////////////////////////////////////////

int main(void) {
  
    static const app_usbd_config_t usbd_config = {
        .ev_state_proc = usbd_user_ev_handler
    };
    
	log_init();
	timers_init();
	uart_init();
	buttons_leds_init();
	db_discovery_init();
	power_management_init();
//------------------------------------------------------------------------------

	ret_code_t ret;
	ret = nrf_drv_clock_init();
	APP_ERROR_CHECK(ret);
	NRF_LOG_INFO("USBD BLE UART example started.");

	app_usbd_serial_num_generate();
	ret = app_usbd_init(&usbd_config);
	APP_ERROR_CHECK(ret);

	app_usbd_class_inst_t const * class_cdc_acm = app_usbd_cdc_acm_class_inst_get(&m_app_cdc_acm);
	ret = app_usbd_class_append(class_cdc_acm);
	APP_ERROR_CHECK(ret);  

	printf("USBD BLE UART example start \r\n");   
//------------------------------------------------------------------------------

	ble_stack_init();
	gatt_init();
	nus_c_init();
	scan_init();
	scan_start();
	NRF_LOG_INFO("BLE scan_start()");

	ret = app_usbd_power_events_enable();
	APP_ERROR_CHECK(ret);

	initRxTxbuffers();

	// Start execution.
	NRF_LOG_INFO("USBD BLE bridge Start execution");
	NRF_LOG_INFO("Version %s %s", version_name, __DATE__);

	int count = 0;
	for (;;) {
		if (count++ > 100 ) {
			CheckIncommingData();
			count = 0;
		
		}
		CheckIncommingData();
		while (app_usbd_event_queue_process()) {
			/* Nothing to do */
		}
		idle_state_handle();
    }
}
////////////////////////////////////////////////////////////////////////////////
