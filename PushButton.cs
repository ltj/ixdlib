using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace IxDLib {
    /// <summary>
    /// Simple push-button/interrupt port driver.
    /// v0.1 by Lars Toft Jacobsen, ITU, IxDLab
    /// CC-BY-SA
    /// </summary>
    public class PushButton : IDisposable {
        
        protected InterruptPort m_interruptPort;
        
        /// <summary>
        /// PushButton constructor.
        /// </summary>
        /// <param name="pin">GPIO input pin</param>
        /// <param name="callback">Callback delegate</param>
        /// <param name="resmode">Resistor mode</param>
        public PushButton(Cpu.Pin pin, NativeEventHandler callback, Port.ResistorMode resmode) {
            Port.InterruptMode imode = resmode == Port.ResistorMode.PullUp ? 
                Port.InterruptMode.InterruptEdgeLow : Port.InterruptMode.InterruptEdgeHigh;
            m_interruptPort = new InterruptPort(pin, true, resmode, imode);

            if (callback != null) {
                m_interruptPort.OnInterrupt += callback;
            }
            // default behavior is to always clear interrupt
            m_interruptPort.OnInterrupt += new NativeEventHandler((uint port, uint state, DateTime time) => {
                m_interruptPort.ClearInterrupt();
            });
        }

        public PushButton(Cpu.Pin pin, NativeEventHandler callback)
            : this(pin, callback, Port.ResistorMode.PullUp) {
        }

        /// <summary>
        /// GPIO pin property
        /// </summary>
        public Cpu.Pin Id {
            get { return m_interruptPort.Id; }
        }


        /// <summary>
        /// Clean up button
        /// </summary>
        public void Dispose() {
            m_interruptPort.Dispose();
        }

    }
}
