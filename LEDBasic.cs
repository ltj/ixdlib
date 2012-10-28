using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace IxDLib {
    /// <summary>
    /// Simple LED driver.
    /// Blinkenlights!!
    /// v0.1 by Lars Toft Jacobsen, ITU, IxDLab
    /// CC-BY-SA
    /// </summary>
    public class LEDBasic : IDisposable {
        
        protected OutputPort led;
        private Timer blinktimer;

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

        public void On() {
            led.Write(true);
        }

        public void Off() {
            led.Write(false);
        }

        public void Blink(int interval, uint reps) {
            reps *= 2;
            blinktimer = new Timer(new TimerCallback((Object data) => {
                if (reps-- != 0) {
                    led.Write(!led.Read());
                }
                else {
                    this.blinktimer.Dispose();
                    Off();
                }
            }), null, 0, interval);
        }


        /// <summary>
        /// Clean up LED
        /// </summary>
        public void Dispose() {
            this.led.Dispose();
        }

    }
}
