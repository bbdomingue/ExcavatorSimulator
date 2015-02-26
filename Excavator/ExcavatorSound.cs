using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Excavator
{
    public class ExcavatorSound
    {
        public static bool CanStop { get { return ExcavatorSound.LiveSounds == 0; } }
        private static int LiveSounds = 0;

        private static Byte[] _Data = null;
        
        private WaveOut _WaveOut = null;
        private LoopStream _LoopStream = null;

        public ExcavatorSound()
        {
            if (ExcavatorSound.LiveSounds == 0) FormBase.addThread();

            ExcavatorSound.LiveSounds++;

            this._LoopStream = new LoopStream(new WaveFileReader(Properties.Resources.excavator_idle));
            this._WaveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
            this._WaveOut.Init(this._LoopStream);
            this._WaveOut.PlaybackStopped += new EventHandler<StoppedEventArgs>(this._WaveOut_PlaybackStopped);
            this._WaveOut.Volume = FormBase._BoolMute ? 0 : 1;
            this._WaveOut.Play();
        }

        ~ExcavatorSound()
        {
        }

        public void setVolume(float f)
        {
            this._WaveOut.Volume = f;
        }

        public void setSpeed(float f)
        {
            this._LoopStream.Speed = f;
        }

        private void _WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            ExcavatorSound.LiveSounds--;
            if (ExcavatorSound.LiveSounds == 0) FormBase.subThread();
        }

        public void Stop()
        {
            if (this._WaveOut != null)
            {
                this._WaveOut.Stop();
                this._WaveOut.Dispose();
                this._WaveOut = null;
            }
        }
    }
}