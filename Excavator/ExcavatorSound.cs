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
        private static readonly object InstanceLock = new object();
        private static readonly object LockWave = new object();

        private static ExcavatorSound _Instance = null;
        internal static ExcavatorSound Instance
        {
            get
            {
                lock (InstanceLock)
                {
                    if (_Instance == null) _Instance = new ExcavatorSound();
                    return _Instance;
                }
            }
        }

        public static bool CanStop { get { lock (InstanceLock) return Instance == null; } }

        private WaveOut _WaveOut = null;
        private LoopStream _LoopStream = null;

        private ExcavatorSound()
        {
            FormBase.addThread();
            this._LoopStream = new LoopStream(new WaveFileReader(Properties.Resources.excavator_idle));
            lock (LockWave)            
            {
                this._WaveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
                this._WaveOut.Init(this._LoopStream);
                this._WaveOut.PlaybackStopped += new EventHandler<StoppedEventArgs>(this._WaveOut_PlaybackStopped);
                this._WaveOut.Volume = 0;
                this._WaveOut.Play();
            }
        }

        ~ExcavatorSound()
        {
        }

        private static bool _Mute;
        public static bool Mute
        {
            set
            {
                lock (LockWave)
                {
                    _Mute = value;
                }
            }
        }

        public void setVolume(float f)
        {
            lock (LockWave)            
                this._WaveOut.Volume = _Mute ? 0 : f;
        }

        public void setSpeed(float f)
        {
            lock (LockWave)
                this._LoopStream.Speed = f;
        }

        private void _WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            FormBase.subThread();
        }

        public static void Stop()
        {
            lock (InstanceLock)
            {
                if (_Instance != null)
                {
                    _Instance.StopP();
                    _Instance = null;
                }
            }           
        }

        private void StopP()
        {
            lock (LockWave)
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
}