﻿using System;

namespace FftSharp.Windows
{
    public class Cosine : Window, IWindow
    {
        public override string Name => "Cosine";
        public override string Description =>
            "This window is simply a cosine function. It reaches zero on both sides and is similar to " +
            "Blackman, Hamming, Hanning, and flat top windows, but probably should not be used in practice.";

        public override double[] Create(int size, bool normalize = false)
        {
            double[] window = new double[size];

            for (int i = 0; i < size; i++)
                window[i] = Math.Sin(i * Math.PI / (size - 1));

            if (normalize)
                NormalizeInPlace(window);

            return window;
        }
    }
}
