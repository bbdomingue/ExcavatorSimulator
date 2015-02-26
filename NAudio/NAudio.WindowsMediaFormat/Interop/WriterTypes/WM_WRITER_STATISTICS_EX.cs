﻿#region Original License
//Widows Media Format Interfaces
//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER
//  REMAINS UNCHANGED.
//
//  Email:  yetiicb@hotmail.com
//
//  Copyright (C) 2002-2004 Idael Cardoso.
//
#endregion

#region Code Modifications Note
// Yuval Naveh, 2010
// Note - The code below has been changed and fixed from its original form.
// Changes include - Formatting, Layout, Coding standards and removal of compilation warnings

// Mark Heath, 2010 - modified for inclusion in NAudio
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NAudio.WindowsMediaFormat
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WM_WRITER_STATISTICS_EX
    {
        public uint dwBitratePlusOverhead;
        public uint dwCurrentSampleDropRateInQueue;
        public uint dwCurrentSampleDropRateInCodec;
        public uint dwCurrentSampleDropRateInMultiplexer;
        public uint dwTotalSampleDropsInQueue;
        public uint dwTotalSampleDropsInCodec;
        public uint dwTotalSampleDropsInMultiplexer;
    }
}
