using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// WRITTEN BY SAM

// KIND OF SKETCHY WAY TO SPEED UP WAV FILES

namespace NAudio.Wave
{
    /// <summary>
    /// Stream for looping playback
    /// </summary>
    public class LoopStream : WaveStream
    {
        WaveStream sourceStream;

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;

            this.BytesPerSample = this.WaveFormat.BitsPerSample / 8; // 8 Bits Per Byte
            this._Bytes = new Byte[this.BytesPerSample];
        }

        /// <summary>
        /// Return source stream's wave format
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        /// <summary>
        /// LoopStream simply returns
        /// </summary>
        public override long Length
        {
            get { return sourceStream.Length; }
        }

        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        private readonly int BytesPerSample;
        private Byte[] _Bytes = new Byte[0];

        private float _Count = 0;
        /// <summary>
        /// Sets speed of Loop Stream.  Can Only Go Faster
        /// </summary>
        public float Speed { set { this._Speed = Math.Min(1.0f, 1.0f / value); } }

        private volatile float _Speed = 1.0f;

        /// <summary>
        /// Call to Read
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(Byte[] buffer, int offset, int count)
        {
            if (false)
            {
                int totalBytesRead = 0;

                while (totalBytesRead < count)
                {
                    int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, Math.Min(2, count - totalBytesRead));
                    if (bytesRead == 0)
                    {
                        if (sourceStream.Position == 0) break;
                        sourceStream.Position = 0;
                    }
                    totalBytesRead += bytesRead;
                }
                return totalBytesRead;
            }
            else
            {
                int totalBytesRead = 0;

                while (totalBytesRead < count)
                {
                    if (sourceStream.Read(buffer, offset + totalBytesRead, this.BytesPerSample) == 0)
                    {
                        sourceStream.Position = 0;
                    }
                    else
                    {
                        this._Count += this._Speed;
                        
                        if (this._Count >= 1)
                        {
                            totalBytesRead += this.BytesPerSample;
                            this._Count -= 1;
                        }
                    }
                }
                return totalBytesRead;
            }
        }        
    }
}
