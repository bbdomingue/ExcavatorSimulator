
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public class Sect
    {
        public Single[,] _Data = null;
        public SectType _Type;
        public int _Width;
        public int _Height;
        public System.Drawing.Size _Size { get { return new System.Drawing.Size(this._Width, this._Height); } }

        public Sect(SectType t, int w, int h) : this(new Single[h, w], t)
        {
        }

        public Sect(Single[,] data, SectType t)
        {
            this._Data = data;
            this._Type = t;
            this._Height = data.GetLength(0);
            this._Width = data.GetLength(1);
        }

        public Sect Clone()
        {
            /*                
                            int w = this._Width;
                            int h = this._Height;

                            Stopwatch st = new Stopwatch();

                            st.Restart();
                            var jar = this._Data.Select(a => a.ToArray()).ToArray();
                            Console.WriteLine("Elapsed 1: " + st.Elapsed.TotalSeconds.ToString());
                            st.Restart();
                            var gar = new float[h, ];
                            for (int y = 0; y < w; y++)
                            {
                                gar[y] = new float[w];

                                for (int x = 0; x < w; x++)
                                {
                                    gar[y, x] = this._Data[y, x];
                                }
                            }
                            Console.WriteLine("Elapsed 2: " + st.Elapsed.TotalSeconds.ToString());
                            st.Restart();
                            var har = new float[h, ];
                            for (int y = 0; y < h; y++)
                            {
                                har[y] = new float[w];
                                Array.Copy(this._Data[y], har[y], w);
                            }
                            Console.WriteLine("Elapsed 3: " + st.Elapsed.TotalSeconds.ToString());
             */

            var ret = new Single[this._Height, this._Width];

            Array.Copy(this._Data, ret, this._Width * this._Height);

            Sect s = new Sect(ret, this._Type);

            if (!this.setMinMaxAvg)
            {
                s.setMinMaxAvg = false;
                s.min = this.min;
                s.max = this.max;
                s.avg = this.avg;
            }

            return s;
        }



        internal void CopyTo(Sect s)
        {
            s.setMinMaxAvg = true;
            Array.Copy(this._Data, s._Data, this._Width * this._Height);
        }















        internal void setValue(Single v)
        {
            this.setMinMaxAvg = true;

            for (int y = 0; y < this._Height; y++)
            {
                for (int x = 0; x < this._Width; x++)
                {
                    this._Data[y, x] = v;
                }
            }
        }
        internal void add(Single v)
        {
            this.setMinMaxAvg = true;

            for (int y = 0; y < this._Height; y++)
            {
                for (int x = 0; x < this._Width; x++)
                {
                    this._Data[y, x] += v;
                }
            }
        }
        internal void multiply(Single v)
        {
            this.setMinMaxAvg = true;

            for (int y = 0; y < this._Height; y++)
            {
                for (int x = 0; x < this._Width; x++)
                {
                    this._Data[y, x] *= v;
                }
            }
        }




        private Single _min, _max, _avg;
        public Single min
        {
            get
            {
                if (this.setMinMaxAvg) this.statsSet(); 
                return this._min;
            }
            private set
            {
                this._min = value;
            }
        }
        public Single max
        {
            get
            {
                if (this.setMinMaxAvg) this.statsSet();
                return this._max;
            }
            private set
            {
                this._max = value;
            }
        }
        public Single avg
        {
            get
            {
                if (this.setMinMaxAvg) this.statsSet();
                return this._avg;
            }
            private set
            {
                this._avg = value;
            }
        }

        internal void resetStats()
        {
            this.setMinMaxAvg = true;
            this._std = -1;
        }

        private Boolean setMinMaxAvg = true;

        private void statsSet()
        {
            this._min = Single.MaxValue;
            this._max = Single.MinValue;
            this._avg = 0;

            Single val = 0;

            for (int y = 0; y < this._Height; y++)
            {
                for (int x = 0; x < this._Width; x++)
                {
                    val = this._Data[y, x];
                    this._min = Math.Min(val, this._min);
                    this._max = Math.Max(val, this._max);
                    this._avg += val;
                }
            }

            this._avg /= (this._Width * this._Height);

            this.setMinMaxAvg = false;
        }

        private Single _std = -1;
        public Single std
        {
            get
            {
                if (this._std < 0)
                {
                    Single f = 0, st = 0;

                    for (int y = 0; y < this._Height; y++)
                    {
                        for (int x = 0; x < this._Width; x++)
                        {
                            f = this._Data[y, x] - this.avg;
                            st += (f * f);
                        }
                    }

                    st /= this._Height * this._Width;
                    this._std = (Single)Math.Sqrt(st);
                }

                return this._std;
            }
        }


        /// <summary>
        /// UNSAFE.  Make sure non null and matching dims
        /// </summary>
        /// <param name="s"></param>
        internal void add(Sect s)
        {
            for (int y = 0; y < this._Height; y++)
            {
                for (int x = 0; x < this._Width; x++)
                {
                    this._Data[y, x] += s._Data[y, x];
                }
            }
        }
        /// <summary>
        /// UNSAFE.  Make sure non null and matching dims
        /// </summary>
        /// <param name="s"></param>
        internal void sub(Sect s)
        {
            for (int y = 0; y < this._Height; y++)
            {
                for (int x = 0; x < this._Width; x++)
                {
                    this._Data[y, x] -= s._Data[y, x];
                }
            }
        }
    }
}
