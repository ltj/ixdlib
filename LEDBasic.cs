using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace IxDLib {
    /// <summary>
    /// Simple LED wrapper class.
    /// Blinkenlights!!
    /// </summary>
    class LEDBasic : IDisposable {
        
        protected OutputPort led;

        /// <summary>
        /// LED on/off state property
        /// </summary>
        public bool State {
            get {
                return led.Read();
            }
            set {
                led.Write(value);
            }
        }

        /// <summary>
        /// LED constructor
        /// </summary>
        /// <param name="pin">GPIO pin</param>
        /// <param name="init">Initial state</param>
        public LEDBasic(Cpu.Pin pin, bool init) {
            led = new OutputPort(pin, init);
        }

        public LEDBasic(Cpu.Pin pin) : this(pin, false) { }

        /// <summary>
        /// Clean up LED
        /// </summary>
        public void Dispose() {
            this.led.Dispose();
        }

    }
}
