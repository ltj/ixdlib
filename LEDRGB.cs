using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace IxDLib {
    /// <summary>
    /// Simple RGB color struct holding red, green and blue
    /// components in 8bit resolution.
    /// </summary>
    public struct RGBColor {
        public byte Red, Green, Blue;

        public RGBColor(byte r, byte g, byte b) {
            Red = r;
            Green = g;
            Blue = b;
        }
    }

    /// <summary>
    /// Simple RGB LED class managing three PWM channels
    /// and color mixing using 8bit RGB color values.
    /// </summary>
    public class LEDRGB : IDisposable {

        private PWM red;
        private PWM green;
        private PWM blue;

        RGBColor color;
        private Timer mixtimer;
        private bool seqrunning;

        /// <summary>
        /// LEDRGB Constructor.
        /// </summary>
        /// <param name="r">Red LED PWM channel</param>
        /// <param name="g">Green LED PWM channel</param>
        /// <param name="b">Blkue LED PWM channel</param>
        public LEDRGB(Cpu.PWMChannel r, Cpu.PWMChannel g, Cpu.PWMChannel b) {
            red = new PWM(r, 255, 0, PWM.ScaleFactor.Microseconds, false);
            green = new PWM(g, 255, 0, PWM.ScaleFactor.Microseconds, false);
            blue = new PWM(b, 255, 0, PWM.ScaleFactor.Microseconds, false);
            color = new RGBColor(0, 0, 0);
        }

        /// <summary>
        /// Color property. Get or set the current color
        /// using an RGBColor value.
        /// </summary>
        public RGBColor Color {
            get {
                return color;
            }
            set {
                color = value;
                red.Duration = color.Red;
                green.Duration = color.Green;
                blue.Duration = color.Blue;
            }
        }

        /// <summary>
        /// Returns sequencing state
        /// </summary>
        public bool Sequencing {
            get { return seqrunning; }
        }

        /// <summary>
        /// Turn LED off. Sets all PWM pulse
        /// durations to 0.
        /// </summary>
        public void Off() {
            red.Duration = 0;
            green.Duration = 0;
            blue.Duration = 0;
        }

        /// <summary>
        /// Turn LED on. Sets all PWM pulse
        /// durations to color value.
        /// </summary>
        public void On() {
            red.Duration = color.Red;
            green.Duration = color.Green;
            blue.Duration = color.Blue;
        }

        /// <summary>
        /// Run a demo sequence
        /// </summary>
        public void StartSequence() {
            Off();
            red.Duration = 255;
            PWM[] p = {red, green, blue};
            int pointer = 1;
            bool up = true;
            mixtimer = new Timer(new TimerCallback((Object data) => {
                if (p[pointer].Duration < 255 && up) p[pointer].Duration++;
                if (p[pointer].Duration == 255) up = false;
                int prev = (pointer - 1 + p.Length) % p.Length;
                if (p[prev].Duration > 0 && !up) p[prev].Duration--;
                if (p[prev].Duration == 0) {
                    up = true;
                    pointer = (pointer + 1 + p.Length) % p.Length;
                }
            }), null, 10, 10);
            seqrunning = true;
        }

        /// <summary>
        /// Stop demo sequence
        /// </summary>
        public void StopSequence() {
            mixtimer.Dispose();
            seqrunning = false;
            On();
        }

        /// <summary>
        /// Clean up RGBLED
        /// </summary>
        public void Dispose() {
            Off();
            if (mixtimer != null) mixtimer.Dispose();
            red.Dispose();
            green.Dispose();
            blue.Dispose();
        }

    }
}
