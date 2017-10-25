﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using ANT_Managed_Library;

namespace AntMessageSimulator
{
    public class MessageException : ApplicationException
    {
        public MessageException(string message) : base(message)
        {
        }

        public MessageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Represents a message recorded to the ANT device log.
    /// </summary>
    public class Message
    {
        #region String Constants
        const byte EVENT_ID_POSITION = 2;
        const byte CHANNEL_ID_POSITION = 3;
        const byte MESSAGE_ID_POSTITION = 4;

        const byte MAX_PAYLOAD_LENGTH = 8;
        const byte MESSAGE_HEADER_LENGTH = 4;
        const byte SET_NETWORK_KEY_EVENT = 0x46;
        const byte MESSAGE_LENGTH_POSITION = 1;
        const byte TRANSMIT_TYPE_START_INDEX = 24;
        const byte TRANSMIT_TYPE_LENGTH = 2;

        const byte DEVICE_TYPE_INDEX = 6;
        const byte DEVICE_ID_MSB_INDEX = 5;
        const byte DEVICE_ID_LSB_INDEX = 4;
        const byte POWER_METER_DEVICE_TYPE = 0x0B;
        const byte FEC_DEVICE_TYPE = 0x11;

        const string TRANSMIT_TX = "Tx";
        const string TRANSMIT_RX = "Rx";
        #endregion

        private float timestamp;
        private byte[] bytes;
        private string transmitType;
        private byte payloadLength;

        /// <summary>
        /// Timestamp generated by the ANT Device Log.
        /// </summary>
        public float Timestamp { get { return this.timestamp; } }

        /// <summary>
        /// Either Rx (received message) or Tx (transmitted message).
        /// </summary>
        public string TransmitType { get { return this.transmitType; } }

        /// <summary>
        /// The EventId on a channel, for example SET NETWORK KEY, BROADCAST MESSAGE, etc...
        /// not to be confused with the message id which is the specific command typically
        /// sent over a broadcast or acknowledged event.
        /// </summary>
        public byte EventId { get { return this.bytes[EVENT_ID_POSITION]; } }

        /// <summary>
        /// 0-8 Channel Id the message was sent on.
        /// </summary>
        public byte? GetChannelId()
        {
            if (IsParseableEvent())
                return this.bytes[CHANNEL_ID_POSITION];
            else
                return null;
        }

        /// <summary>
        /// The specific command of the payload.
        /// </summary>
        public byte GetMessageId()
        {
            if (IsDataMessage())
                return this.bytes[MESSAGE_ID_POSTITION];
            else
                return 0;
        }

        public ushort GetDeviceId()
        {
            if (IsChannelIdEvent())
                return (ushort)(Bytes[DEVICE_ID_MSB_INDEX] << 8 |
                            Bytes[DEVICE_ID_LSB_INDEX]);
            else
                return 0;
        }

        public bool IsChannelIdEvent()
        {
            return EventId == (byte)ANT_ReferenceLibrary.ANTMessageID.CHANNEL_ID_0x51;
        }
        
        public bool IsTransmit()
        {
            return transmitType == TRANSMIT_TX;
        }

        public bool IsPowerMeterIdEvent()
        {
            return IsChannelIdEvent() && 
                Bytes[DEVICE_TYPE_INDEX] == POWER_METER_DEVICE_TYPE;
        }

        public bool IsFecIdEvent()
        {
            return IsChannelIdEvent() &&
                Bytes[DEVICE_TYPE_INDEX] == FEC_DEVICE_TYPE;
        }

        public byte[] Bytes { get { return this.bytes; } }

        public byte PayloadLength {  get { return this.payloadLength; } }

        public string GetPayloadAsString()
        {
            StringBuilder payload = new StringBuilder();
            for (int i = EVENT_ID_POSITION; i < PayloadLength; i++)
                payload.AppendFormat("[{0:X2}]", Bytes[i]);

            return payload.ToString();
        }

        /// <summary>
        /// Parses a single line from an ANT device log and represents as a Message object.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Message MessageFromLine(string line)
        {
            return new Message(line);
        }

        private bool IsParseableEvent()
        {
            return (this.EventId != SET_NETWORK_KEY_EVENT);
        }

        public bool IsDataMessage()
        {
            switch ((ANT_ReferenceLibrary.ANTMessageID)this.EventId)
            {
                case ANT_ReferenceLibrary.ANTMessageID.BROADCAST_DATA_0x4E:
                case ANT_ReferenceLibrary.ANTMessageID.ACKNOWLEDGED_DATA_0x4F:
                case ANT_ReferenceLibrary.ANTMessageID.BURST_DATA_0x50:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Parses the timestampe from the left hand side of the '-' in the string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float GetTimestamp(string value)
        {
            // data =   135.031 { 673362484} Rx - [A4][09][4E][00][19][2A][FF][C0][00][39][00][31][E7]
            // time =   135.031
            int stopIndex = value.IndexOf('{');

            if (stopIndex <= 0)
                throw new MessageException("Timestamp not found in line: " + value);

            try
            {
                return float.Parse(value.Substring(0, stopIndex - 1));
            }
            catch (Exception parseException)
            {
                throw new MessageException("Timestamp not parseable in: " + value,
                    parseException);
            }
        }

        /// <summary>
        /// Parses the transmit type Tx (transmit) or Rx (receive).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetTransmitType(string value)
        {
            // can be Rx or Tx
            return value.Substring(TRANSMIT_TYPE_START_INDEX, TRANSMIT_TYPE_LENGTH);
        }

        /// <summary>
        /// Left side contains timestamp and transmit type.
        /// </summary>
        /// <param name="value"></param>
        private void ParseTransmitInfo(string value)
        {
            this.timestamp = GetTimestamp(value);
            this.transmitType = GetTransmitType(value);
        }

        /// <summary>
        /// Returns the acceptable payload length + ant header length from the 
        /// ANT message.
        /// </summary>
        /// <remarks>
        /// If the payload > 8 bytes, truncate at 8.
        /// </remarks>
        /// <param name="stringBytes"></param>
        /// <returns></returns>
        private byte GetPayloadLength(string[] stringBytes)
        {
            byte length = Convert.ToByte(stringBytes[MESSAGE_LENGTH_POSITION], 16);
            if (length > MAX_PAYLOAD_LENGTH)
                length = MAX_PAYLOAD_LENGTH + MESSAGE_HEADER_LENGTH;
            else
                length += MESSAGE_HEADER_LENGTH;

            return length;
        }

        /// <summary>
        /// Right side contains the bytes that were on the wire encoded as strings.
        /// </summary>
        /// <remarks>
        /// Don't read past 8 bytes.
        /// [A4][14][4E][01][01][10][01][00][00][00][02][4B][E0][E6][01][0B][01][10][00][6D][00][5A][42][CE]
        ///      |-- Length
        ///          |-- EventID
        ///              |-- ChannelID
        ///                  |-- MessageID (beginning of paylod)
        ///                        End of 8 byte payload --|
        /// </remarks>
        /// <param name="value"></param>
        private void ParseMessageBytes(string value)
        {
            // Strip into individual elements so we parse meaningful values.
            string[] stringBytes = Regex.Split(value, @"\]\[");
            this.payloadLength = GetPayloadLength(stringBytes);
            byte[] bytes = new byte[this.payloadLength];

            // Regex doesn't parse the first or last well, so just set to 0 since we don't use 1st byte.
            bytes[0] = 0;

            // Fix up the last byte.
            if (stringBytes[this.payloadLength - 1].EndsWith("]"))
                stringBytes[this.payloadLength - 1] = 
                    stringBytes[this.payloadLength - 1].Replace("]", "");

            // Convert string representation into byte array.
            for (int i = 1; i < this.payloadLength; i++)
                bytes[i] = Convert.ToByte(stringBytes[i], 16);

            this.bytes = bytes;
        }

        /// <summary>
        /// Sets the internal state of the message object by parsing the line.
        /// </summary>
        /// <param name="line"> Example:
        ///     33.609 {2319281343} Rx - [A4][14][4E][01][01][10][01][00][00][00][02][4B][E0][E6][01][0B][01][10][01][6D][00][3B][A2][4E]
        /// </param>
        /// <exception cref="MessageException">Check inner exception for additional details.</exception>
        private void ParseLine(string line)
        {
            string[] record = line.Split('-'); // Splits into timestamp and hex bytes ascii chars.

            if (record == null || record.Length != 2)
                throw new MessageException("Improperly formatted log line: " + line);

            ParseTransmitInfo(record[0]);
            ParseMessageBytes(record[1]);
        }

        /// <summary>
        /// Creates a message object from a string representing a line in a device log.
        /// </summary>
        /// <param name="line"></param>
        private Message(string line)
        {
            ParseLine(line);
        }
    }
}
