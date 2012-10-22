using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace IxDLib {
    /// <summary>
    /// I2C device abstraction based on the I2CPlug class by Jeroen Swart.
    /// Wire class for easy implementation of new I2C drivers.
    /// v0.1 by Lars Toft Jacobsen, ITU, IxDLab
    /// CC-BY-SA
    /// </summary>
    public class Wire {

        private const int DefaultClockRate = 400;
        private const int TransactionTimeout = 1000;

        private I2CDevice.Configuration i2cConfig;
        private I2CDevice i2cDevice;

        public byte Address { get; private set; }

        public Wire(byte address, int clockRateKhz) {
            this.Address = address;
            this.i2cConfig = new I2CDevice.Configuration(this.Address, clockRateKhz);
            this.i2cDevice = new I2CDevice(this.i2cConfig);
        }

        public Wire(byte address)
            : this(address, DefaultClockRate) {
        }

        /// <summary>
        /// Write to I2C device
        /// </summary>
        /// <param name="writeBuffer">buffer to write</param>
        private void Write(byte[] writeBuffer) {
            // create a write transaction containing the bytes to be written to the device
            I2CDevice.I2CTransaction[] writeTransaction = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateWriteTransaction(writeBuffer)
            };

            // write the data to the device
            int written = this.i2cDevice.Execute(writeTransaction, TransactionTimeout);

            while (written < writeBuffer.Length) {
                byte[] newBuffer = new byte[writeBuffer.Length - written];
                Array.Copy(writeBuffer, written, newBuffer, 0, newBuffer.Length);

                writeTransaction = new I2CDevice.I2CTransaction[]
                {
                    I2CDevice.CreateWriteTransaction(newBuffer)
                };

                written += this.i2cDevice.Execute(writeTransaction, TransactionTimeout);
            }

            // make sure the data was sent
            if (written != writeBuffer.Length) {
                throw new Exception("Could not write to device.");
            }
        }

        /// <summary>
        /// Read from I2C device
        /// </summary>
        /// <param name="readBuffer">read buffer</param>
        private void Read(byte[] readBuffer) {
            // create a read transaction
            I2CDevice.I2CTransaction[] readTransaction = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateReadTransaction(readBuffer)
            };

            // read data from the device
            int read = this.i2cDevice.Execute(readTransaction, TransactionTimeout);

            // make sure the data was read
            if (read != readBuffer.Length) {
                throw new Exception("Could not read from device.");
            }
        }

        /// <summary>
        /// Write to register
        /// </summary>
        /// <param name="register">Register</param>
        /// <param name="value">Value to be written</param>
        protected void WriteToRegister(byte register, byte value) {
            this.Write(new byte[] { register, value });
        }

        /// <summary>
        /// Burst write to multiple registers
        /// </summary>
        /// <param name="register">Start register</param>
        /// <param name="values">Values to be written</param>
        protected void WriteToRegister(byte register, byte[] values) {
            // create a single buffer, so register and values can be send in a single transaction
            byte[] writeBuffer = new byte[values.Length + 1];
            writeBuffer[0] = register;
            Array.Copy(values, 0, writeBuffer, 1, values.Length);

            this.Write(writeBuffer);
        }

        /// <summary>
        /// Read one or burst read multiple values from register
        /// </summary>
        /// <param name="register">Register</param>
        /// <param name="readBuffer">Read buffer</param>
        protected void ReadFromRegister(byte register, byte[] readBuffer) {
            this.Write(new byte[] { register });
            this.Read(readBuffer);
        }

    }
}
