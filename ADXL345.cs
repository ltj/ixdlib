using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace IxDLib {
    public class ADXL345 : I2CUnit {

        private const byte ADDR_ALT_HIGH = 0x1D;
        private const byte ADDR_ALT_LOW = 0x53;

        // gains defaults to +-2g @ 10bit resolution
        private double m_gain = 4.0/1024;
        private double[] m_gxyz = new double[3];

        /// <summary>
        /// ADXL345 register addresses
        /// </summary>
        public enum Registers : byte {
            DEVID = 0x00,
            THRESH_TAP = 0x1D,
            OFSX = 0x1E,
            OFSY = 0x1F,
            OFSZ = 0x20,
            DUR = 0x21,
            Latent = 0x22,
            Window = 0x23,
            THRESH_ACT = 0x24,
            THRESH_INACT = 0x25,
            TIME_INACT = 0x26,
            ACT_INACT_CTL = 0x27,
            THRESH_FF = 0x28,
            TIME_FF = 0x29,
            TAP_AXES = 0x2A,
            ACT_TAP_STATUS = 0x2B,
            BW_RATE = 0x2C,
            POWER_CTL = 0x2D,
            INT_ENABLE = 0x2E,
            INT_MAP = 0x2F,
            INT_SOURCE = 0x30,
            DATA_FORMAT = 0x31,
            DATAX0 = 0x32,
            DATAX1 = 0x33,
            DATAY0 = 0x34,
            DATAY1 = 0x35,
            DATAZ0 = 0x36,
            DATAZ1 = 0x37,
            FIFO_CTL = 0x38,
            FIFO_STATUS = 0x39
        }

        public ADXL345()
            : base(ADDR_ALT_LOW) {
        }

        public void PowerOn() {
            WriteToRegister((byte)Registers.POWER_CTL, 8);
        }

        public void readAccel() {
            byte[] buffer = new byte[6];
            short[] xyz = new short[3];
            ReadFromRegister((byte)Registers.DATAX0, buffer);
            xyz[0] = (short)((buffer[1] << 8) | buffer[0]);
            xyz[1] = (short)((buffer[3] << 8) | buffer[2]);
            xyz[2] = (short)((buffer[5] << 8) | buffer[4]);
            for (int i = 0; i < 3; i++) {
                this.m_gxyz[i] = xyz[i] * m_gain;
            }
        }

        /// <summary>
        /// The measured G value on all three axes 
        /// </summary>
        public double[] Gxyz {
            get {
                return m_gxyz;
            }
        }

        /// <summary>
        /// Measurement range property.
        /// Can be +-2, 4, 8 or 16g.
        /// </summary>
        public short Range {
            get {
                return getRange();
            }
            set {
                setRange(value);
            }
        }

        public double Gain {
            get { return m_gain; }
        }

        /// <summary>
        /// Sets the G measurement range (+-2, +-4, +-8 or +-16G)
        /// </summary>
        /// <param name="r">G range</param>
        private void setRange(short r) {
            byte[] buf = new byte[1];
            byte rb = 0;
            double s = 2.0 / 1024;
            switch (r) {
                    case 2:
                        rb = 0;
                        s = 2.0 * r / 1024;
                        break;
                    case 4:
                        rb = 1;
                        s = 2.0 * r / 1024;
                        break;
                    case 8:
                        rb = 2;
                        s = 2.0 * r / 1024;
                        break;
                    case 16:
                        rb = 3;
                        s = 2.0 * r / 1024;
                        break;
                    default:
                        break;
                }
            ReadFromRegister((byte)Registers.DATA_FORMAT, buf);
            rb |= (byte)(buf[0] & 236);
            WriteToRegister((byte)Registers.DATA_FORMAT, rb);
            this.m_gain = s;
        }

        private short getRange() {
            byte[] buf = new byte[1];
            ReadFromRegister((byte)Registers.DATA_FORMAT, buf);
            byte res = (byte)(buf[0] & 3);
            switch (res) {
                case 0:
                    return 2;
                case 1:
                    return 4;
                case 2:
                    return 8;
                case 3:
                    return 16;
                default:
                    break;
            }
            return -1;
        }
    }
}
