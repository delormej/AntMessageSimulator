using System;

namespace AntMessageSimulator.ANT_NET
{
    public sealed class ANT_ReferenceLibrary
    {
        public const byte MAX_MESG_SIZE = 41;

        public enum ANTEventID : byte
        {
            RESPONSE_NO_ERROR_0x00 = 0,
            NO_EVENT_0x00 = 0,
            EVENT_RX_SEARCH_TIMEOUT_0x01 = 1,
            EVENT_RX_FAIL_0x02 = 2,
            EVENT_TX_0x03 = 3,
            EVENT_TRANSFER_RX_FAILED_0x04 = 4,
            EVENT_TRANSFER_TX_COMPLETED_0x05 = 5,
            EVENT_TRANSFER_TX_FAILED_0x06 = 6,
            EVENT_CHANNEL_CLOSED_0x07 = 7,
            EVENT_RX_FAIL_GO_TO_SEARCH_0x08 = 8,
            EVENT_CHANNEL_COLLISION_0x09 = 9,
            EVENT_TRANSFER_TX_START_0x0A = 10,
            EVENT_CHANNEL_ACTIVE_0x0F = 15,
            EVENT_TRANSFER_TX_COMPLETED_RSSI_0x10 = 16,
            EVENT_TRANSFER_TX_NEXT_MESSAGE_0x11 = 17,
            CHANNEL_IN_WRONG_STATE_0x15 = 21,
            CHANNEL_NOT_OPENED_0x16 = 22,
            CHANNEL_ID_NOT_SET_0x18 = 24,
            CLOSE_ALL_CHANNELS_0x19 = 25,
            TRANSFER_IN_PROGRESS_0x1F = 31,
            TRANSFER_SEQUENCE_NUMBER_ERROR_0x20 = 32,
            TRANSFER_IN_ERROR_0x21 = 33,
            TRANSFER_BUSY_0x22 = 34,
            MESSAGE_SIZE_EXCEEDS_LIMIT_0x27 = 39,
            INVALID_MESSAGE_0x28 = 40,
            INVALID_NETWORK_NUMBER_0x29 = 41,
            INVALID_LIST_ID_0x30 = 48,
            INVALID_SCAN_TX_CHANNEL_0x31 = 49,
            INVALID_RSSI_THRESHOLD_0x32 = 50,
            INVALID_PARAMETER_PROVIDED_0x33 = 51,
            EVENT_QUE_OVERFLOW_0x35 = 53,
            EVENT_CLK_ERROR_0x36 = 54,
            SCRIPT_FULL_ERROR_0x40 = 64,
            SCRIPT_WRITE_ERROR_0x41 = 65,
            SCRIPT_INVALID_PAGE_ERROR_0x42 = 66,
            SCRIPT_LOCKED_ERROR_0x43 = 67,
            NO_RESPONSE_MESSAGE_0x50 = 80,
            RETURN_TO_MFG_0x51 = 81,
            FIT_ACTIVE_SEARCH_TIMEOUT_0x60 = 96,
            FIT_WATCH_PAIR_0x61 = 97,
            FIT_WATCH_UNPAIR_0x62 = 98,
            USB_STRING_WRITE_FAIL_0x70 = 112
        }
        public enum ANTMessageID : byte
        {
            INVALID_0x00 = 0,
            EVENT_0x01 = 1,
            VERSION_0x3E = 62,
            RESPONSE_EVENT_0x40 = 64,
            UNASSIGN_CHANNEL_0x41 = 65,
            ASSIGN_CHANNEL_0x42 = 66,
            CHANNEL_MESG_PERIOD_0x43 = 67,
            CHANNEL_SEARCH_TIMEOUT_0x44 = 68,
            CHANNEL_RADIO_FREQ_0x45 = 69,
            NETWORK_KEY_0x46 = 70,
            RADIO_TX_POWER_0x47 = 71,
            RADIO_CW_MODE_0x48 = 72,
            SEARCH_WAVEFORM_0x49 = 73,
            SYSTEM_RESET_0x4A = 74,
            OPEN_CHANNEL_0x4B = 75,
            CLOSE_CHANNEL_0x4C = 76,
            REQUEST_0x4D = 77,
            BROADCAST_DATA_0x4E = 78,
            ACKNOWLEDGED_DATA_0x4F = 79,
            BURST_DATA_0x50 = 80,
            CHANNEL_ID_0x51 = 81,
            CHANNEL_STATUS_0x52 = 82,
            RADIO_CW_INIT_0x53 = 83,
            CAPABILITIES_0x54 = 84,
            STACKLIMIT_0x55 = 85,
            SCRIPT_DATA_0x56 = 86,
            SCRIPT_CMD_0x57 = 87,
            ID_LIST_ADD_0x59 = 89,
            ID_LIST_CONFIG_0x5A = 90,
            OPEN_RX_SCAN_0x5B = 91,
            EXT_CHANNEL_RADIO_FREQ_0x5C = 92,
            EXT_BROADCAST_DATA_0x5D = 93,
            EXT_ACKNOWLEDGED_DATA_0x5E = 94,
            EXT_BURST_DATA_0x5F = 95,
            CHANNEL_RADIO_TX_POWER_0x60 = 96,
            GET_SERIAL_NUM_0x61 = 97,
            GET_TEMP_CAL_0x62 = 98,
            SET_LP_SEARCH_TIMEOUT_0x63 = 99,
            SET_TX_SEARCH_ON_NEXT_0x64 = 100,
            SERIAL_NUM_SET_CHANNEL_ID_0x65 = 101,
            RX_EXT_MESGS_ENABLE_0x66 = 102,
            RADIO_CONFIG_ALWAYS_0x67 = 103,
            ENABLE_LED_FLASH_0x68 = 104,
            LED_OVERRIDE_0x69 = 105,
            AGC_CONFIG_0x6A = 106,
            CLOCK_DRIFT_DATA_0x6B = 107,
            RUN_SCRIPT_0x6C = 108,
            XTAL_ENABLE_0x6D = 109,
            ANTLIB_CONFIG_0x6E = 110,
            STARTUP_MESG_0x6F = 111,
            AUTO_FREQ_CONFIG_0x70 = 112,
            PROX_SEARCH_CONFIG_0x71 = 113,
            ADV_BURST_DATA_0x72 = 114,
            EVENT_BUFFER_CONFIG_0x74 = 116,
            SET_SEARCH_PRIORITY_LEVEL_0x75 = 117,
            HIGH_DUTY_SEARCH_CONFIG_0x77 = 119,
            ADV_BURST_CONFIG_0x78 = 120,
            EVENT_FILTER_CONFIG_0x79 = 121,
            SDU_CONFIG_0x7A = 122,
            SET_SDU_MASK_0x7B = 123,
            USER_NVM_CONFIG_0x7C = 124,
            ENABLE_ENCRYPTION_0x7D = 125,
            SET_ENCRYPTION_KEY_0x7E = 126,
            SET_ENCRYPTION_INFO_0x7F = 127,
            CUBE_CMD_0x80 = 128,
            SET_SEARCH_SHARING_CYCLES_0x81 = 129,
            ENCRYPTION_KEY_NVM_OPERATION_0x83 = 131,
            PIN_DIODE_CONTROL_0x8E = 142,
            FIT1_SET_AGC_0x8F = 143,
            SET_CHANNEL_INPUT_MASK_0x90 = 144,
            FIT1_SET_EQUIP_STATE_0x91 = 145,
            SET_CHANNEL_DATA_TYPE_0x91 = 145,
            READ_PINS_FOR_SECT_0x92 = 146,
            TIMER_SELECT_0x93 = 147,
            ATOD_SETTINGS_0x94 = 148,
            SET_SHARED_ADDRESS_0x95 = 149,
            ATOD_EXTERNAL_ENABLE_0x96 = 150,
            ATOD_PIN_SETUP_0x97 = 151,
            SETUP_ALARM_0x98 = 152,
            ALARM_VARIABLE_MODIFY_TEST_0x99 = 153,
            PARTIAL_RESET_0x9A = 154,
            OVERWRITE_TEMP_CAL_0x9B = 155,
            SERIAL_PASSTHRU_SETTINGS_0x9C = 156,
            READ_SEGA_0xA0 = 160,
            SEGA_CMD_0xA1 = 161,
            SEGA_DATA_0xA2 = 162,
            SEGA_ERASE_0xA3 = 163,
            SEGA_WRITE_0xA4 = 164,
            SEGA_LOCK_0xA6 = 166,
            FLASH_PROTECTION_CHECK_0xA7 = 167,
            UARTREG_0xA8 = 168,
            MAN_TEMP_0xA9 = 169,
            BIST_0xAA = 170,
            SELFERASE_0xAB = 171,
            SET_MFG_BITS_0xAC = 172,
            UNLOCK_INTERFACE_0xAD = 173,
            SERIAL_ERROR_0xAE = 174,
            IO_STATE_0xB0 = 176,
            CFG_STATE_0xB1 = 177,
            BLOWFUSE_0xB2 = 178,
            MASTERIOCTRL_0xB3 = 179,
            PORT_GET_IO_STATE_0xB4 = 180,
            PORT_SET_IO_STATE_0xB5 = 181,
            RSSI_POWER_0xC0 = 192,
            RSSI_BROADCAST_DATA_0xC1 = 193,
            RSSI_ACKNOWLEDGED_DATA_0xC2 = 194,
            RSSI_BURST_DATA_0xC3 = 195,
            RSSI_SEARCH_THRESHOLD_0xC4 = 196,
            SLEEP_0xC5 = 197,
            SET_USB_INFO_0xC7 = 199,
            DEBUG_0xF0 = 240
        }
        [Flags]
        public enum EventBufferConfig : byte
        {
            BUFFER_LOW_PRIORITY_EVENTS = 0,
            BUFFER_ALL_EVENTS = 1
        }
        public enum EncryptionNVMOp : byte
        {
            LOAD_KEY_FROM_NVM = 0,
            STORE_KEY_TO_NVM = 1
        }
        public enum EncryptionInfo : byte
        {
            ENCRYPTION_ID = 0,
            USER_INFO_STRING = 1,
            RANDOM_NUMBER_SEED = 2
        }
        public enum EncryptedChannelMode : byte
        {
            DISABLE = 0,
            ENABLE = 1,
            ENABLE_USER_INFO = 2
        }
        [Flags]
        public enum AdvancedBurstConfigFlags : uint
        {
            FREQUENCY_HOP_ENABLE = 1
        }
        [Flags]
        public enum LibConfigFlags
        {
            RADIO_CONFIG_ALWAYS_0x01 = 1,
            MESG_OUT_INC_TIME_STAMP_0x20 = 32,
            MESG_OUT_INC_RSSI_0x40 = 64,
            MESG_OUT_INC_DEVICE_ID_0x80 = 128
        }
        public enum USB_DescriptorString : byte
        {
            USB_DESCRIPTOR_VID_PID = 0,
            USB_DESCRIPTOR_MANUFACTURER_STRING = 1,
            USB_DESCRIPTOR_DEVICE_STRING = 2,
            USB_DESCRIPTOR_SERIAL_STRING = 3
        }
        public enum SensRcoreScriptCommandCodes : byte
        {
            SCRIPT_CMD_FORMAT_0x00 = 0,
            SCRIPT_CMD_DUMP_0x01 = 1,
            SCRIPT_CMD_SET_DEFAULT_SECTOR_0x02 = 2,
            SCRIPT_CMD_END_SECTOR_0x03 = 3,
            SCRIPT_CMD_END_DUMP_0x04 = 4,
            SCRIPT_CMD_LOCK_0x05 = 5
        }
        public enum StartupMessage : byte
        {
            RESET_POR_0x00 = 0,
            RESET_RST_0x01 = 1,
            RESET_WDT_0x02 = 2,
            RESET_CMD_0x20 = 32,
            RESET_SYNC_0x40 = 64,
            RESET_SUSPEND_0x80 = 128
        }
        public enum TransmitPower : byte
        {
            RADIO_TX_POWER_MINUS20DB_0x00 = 0,
            RADIO_TX_POWER_MINUS10DB_0x01 = 1,
            RADIO_TX_POWER_MINUS5DB_0x02 = 2,
            RADIO_TX_POWER_0DB_0x03 = 3
        }
        public enum BasicChannelStatusCode : byte
        {
            UNASSIGNED_0x0 = 0,
            ASSIGNED_0x1 = 1,
            SEARCHING_0x2 = 2,
            TRACKING_0x3 = 3
        }
        public enum MessagingReturnCode
        {
            Fail = 0,
            Pass = 1,
            Timeout = 2,
            Cancelled = 3,
            InvalidParams = 4
        }
        [Flags]
        public enum ChannelTypeExtended : byte
        {
            ADV_AlwaysSearch_0x01 = 1,
            ADV_IgnoreTransmissionType_0x02 = 2,
            ADV_FrequencyAgility_0x04 = 4,
            ADV_AutoSharedSlave_0x08 = 8,
            ADV_FastStart_0x10 = 16,
            ADV_AsyncTx_0x20 = 32
        }
        [Flags]
        public enum ChannelType : byte
        {
            BASE_Slave_Receive_0x00 = 0,
            BASE_Master_Transmit_0x10 = 16,
            ADV_Shared_0x20 = 32,
            ADV_TxRx_Only_or_RxAlwaysWildCard_0x40 = 64,
            ADV_Enable_RSSI_0x80 = 128
        }
        public enum FramerType : byte
        {
            basicANT = 0
        }
        public enum PortType : byte
        {
            USB = 0,
            COM = 1
        }
        public enum RequestMessageID : byte
        {
            VERSION_0x3E = 62,
            CHANNEL_ID_0x51 = 81,
            CHANNEL_STATUS_0x52 = 82,
            CAPABILITIES_0x54 = 84,
            SERIAL_NUMBER_0x61 = 97,
            USER_NVM_0x7C = 124
        }
        public enum USB_PID : ushort
        {
            ANT_INTERFACE_BOARD = 16642,
            ANT_ARCT = 16643
        }
    }
}