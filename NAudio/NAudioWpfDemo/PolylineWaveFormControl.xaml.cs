﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NAudioWpfDemo
{
    /// <summary>
    /// Interaction logic for PolylineWaveFormControl.xaml
    /// </summary>
    public partial class PolylineWaveFormControl : UserControl, IWaveFormRenderer
    {
        int renderPosition;
        double yTranslate = 40;
        double yScale = 40;
        int blankZone = 10;
        
        Polyline topLine = new Polyline();
        Polyline bottomLine = new Polyline();
        
        public PolylineWaveFormControl()
        {
            this.SizeChanged += OnSizeChanged;
            InitializeComponent();
            topLine.Stroke = this.Foreground;
            bottomLine.Stroke = this.Foreground;
            topLine.StrokeThickness = 1;
            bottomLine.StrokeThickness = 1;
            mainCanvas.Children.Add(topLine);
            mainCanvas.Children.Add(bottomLine);
        }
        
        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // We will remove everything as we are going to rescale vertically
            renderPosition = 0;
            ClearAllPoints();

            this.yTranslate = this.ActualHeight / 2;
            this.yScale = this.ActualHeight / 2;
        }

        private void ClearAllPoints()
        {
            topLine.Points.Clear();
            bottomLine.Points.Clear();
        }

        public void AddValue(float maxValue, float minValue)
        {
            int pixelWidth = (int)ActualWidth;
            if (pixelWidth > 0)
            {
                CreatePoint(maxValue, minValue);

                if (renderPosition > ActualWidth)
                {
                    renderPosition = 0;
                }
                int erasePosition = (renderPosition + blankZone) % pixelWidth;
                if (erasePosition < topLine.Points.Count)
                {
                    double yPos = SampleToYPosition(0);
                    topLine.Points[erasePosition] = new Point(erasePosition, yPos);
                    bottomLine.Points[erasePosition] = new Point(erasePosition, yPos);
                }
            }
        }

        private double SampleToYPosition(float value)
        {
            return yTranslate + value * yScale;
        }

        private void CreatePoint(float topValue, float bottomValue)
        {
            double topLinePos = SampleToYPosition(topValue);
            double bottomLinePos = SampleToYPosition(bottomValue);
            if (renderPosition >= topLine.Points.Count)
            {
                topLine.Points.Add(new Point(renderPosition, topLinePos));
                bottomLine.Points.Add(new Point(renderPosition, bottomLinePos));
            }
            else
            {
                topLine.Points[renderPosition] = new Point(renderPosition, topLinePos);
                bottomLine.Points[renderPosition] = new Point(renderPosition, bottomLinePos);
            }
            renderPosition++;
        }

        /// <summary>
        /// Clears the waveform and repositions on the left
        /// </summary>
        public void Reset()
        {
            renderPosition = 0;
            ClearAllPoints();
        }
    }
}
