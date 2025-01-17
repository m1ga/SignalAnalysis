﻿namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Saves data into a text-formatted file.
    /// </summary>
    /// <param name="FileName">Path (including name) of the text file</param>
    /// <param name="Data">Array of values to be saved</param>
    /// <param name="ArrIndexInit">Offset index of _signalData (used to compute the time data field)</param>
    /// <param name="SeriesName">Name of the serie data to be saved</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool SaveTextData(string FileName, double[] Data, int ArrIndexInit, string? SeriesName)
    {
        bool result = false;

        try
        {
            using var fs = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            // Append millisecond pattern to current culture's full date time pattern
            string fullPattern = _settings.AppCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", _settings.MillisecondsFormat);

            // Save the header text into the file
            string content = string.Empty;
            TimeSpan nTime = nStart.AddSeconds((Data.Length - 1) / nSampleFreq) - nStart; // At least there should be 1 point

            sw.WriteLine($"{StringResources.FileHeader01} ({_settings.AppCultureName})");
            sw.WriteLine($"{StringResources.FileHeader02}: {nStart.AddSeconds(ArrIndexInit / nSampleFreq).ToString(fullPattern, _settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader03}: {nStart.AddSeconds((Data.Length - 1 + ArrIndexInit) / nSampleFreq).ToString(fullPattern, _settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader04}: " +
                $"{nTime.Days} {StringResources.FileHeader22}, " +
                $"{nTime.Hours} {StringResources.FileHeader23}, " +
                $"{nTime.Minutes} {StringResources.FileHeader24}, " +
                $"{nTime.Seconds} {StringResources.FileHeader25} " +
                $"{StringResources.FileHeader26} " +
                $"{nTime.Milliseconds} {StringResources.FileHeader27}");
            sw.WriteLine($"{StringResources.FileHeader17}: 1");
            sw.WriteLine($"{StringResources.FileHeader05}: {Data.Length.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader06}: {nSampleFreq.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader07}: {Results.Average.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader08}: {Results.Maximum.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader09}: {Results.Minimum.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader10}: {Results.FractalDimension.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader11}: {Results.FractalVariance.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader12}: {Results.ApproximateEntropy.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader13}: {Results.SampleEntropy.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader14}: {Results.ShannonEntropy.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader15}: {Results.EntropyBit.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader16}: {Results.IdealEntropy.ToString(_settings.AppCulture)}");
            sw.WriteLine();
            sw.WriteLine($"{StringResources.FileHeader21}\t{SeriesName}");

            string time;
            // Save the numerical values
            for (int j = 0; j < Data.Length; j++)
            {
                time = nStart.AddSeconds((j+ ArrIndexInit) / nSampleFreq).ToString(fullPattern, _settings.AppCulture);
                content = $"{time}\t{Data[j].ToString(_settings.DataFormat, _settings.AppCulture)}";
                
                //trying to write data to csv
                sw.WriteLine(content);
            }

            // Success!
            result = true;
        }
        catch (Exception ex)
        {
            // Show error message
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(String.Format(StringResources.MsgBoxErrorSaveData, ex.Message),
                    StringResources.MsgBoxErrorSaveDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }

    /// <summary>
    /// Saves data into a SignalAnalysis-formatted file.
    /// </summary>
    /// <param name="FileName">Path (including name) of the sig file</param>
    /// <param name="Data">Array of values to be saved</param>
    /// <param name="ArrIndexInit">Offset index of _signalData (used to compute the time data field)</param>
    /// <param name="SeriesName">Name of the serie data to be saved</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool SaveSigData(string FileName, double[] Data, int ArrIndexInit, string? SeriesName)
    {
        bool result = false;

        try
        {
            using var fs = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            // Append millisecond pattern to current culture's full date time pattern
            string fullPattern = _settings.AppCulture.DateTimeFormat.FullDateTimePattern;
            fullPattern = System.Text.RegularExpressions.Regex.Replace(fullPattern, "(:ss|:s)", _settings.MillisecondsFormat);

            // Save the header text into the file
            string content = string.Empty;

            sw.WriteLine($"{StringResources.FileHeader01} ({_settings.AppCultureName})");
            sw.WriteLine($"{StringResources.FileHeader17}: 1");
            sw.WriteLine($"{StringResources.FileHeader05}: {Data.Length.ToString(_settings.AppCulture)}");
            sw.WriteLine($"{StringResources.FileHeader06}: {nSampleFreq.ToString(_settings.AppCulture)}");
            sw.WriteLine();
            sw.WriteLine($"{SeriesName}");

            // Save the numerical values
            for (int j = 0; j < Data.Length; j++)
                sw.WriteLine(Data[j].ToString(_settings.DataFormat, _settings.AppCulture));

            // Success!
            result = true;
        }
        catch (Exception ex)
        {
            // Show error message
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(String.Format(StringResources.MsgBoxErrorSaveData, ex.Message),
                    StringResources.MsgBoxErrorSaveDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        return result;
    }

    /// <summary>
    /// Saves data into a binary-formatted file. Adapts the text-format to a binary format.
    /// </summary>
    /// <param name="FileName">Path (including name) of the sig file</param>
    /// <param name="Data">Array of values to be saved</param>
    /// <param name="ArrIndexInit">Offset index of _signalData (used to compute the time data field)</param>
    /// <param name="SeriesName">Name of the serie data to be saved</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool SaveBinaryData(string FileName, double[] Data, int ArrIndexInit, string? SeriesName)
    {
        bool result = false;
        
        try
        {
            using var fs = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var bw = new BinaryWriter(fs, System.Text.Encoding.UTF8, false);
            
            string content = string.Empty;
            TimeSpan nTime = nStart.AddSeconds((Data.Length - 1) / nSampleFreq) - nStart; // At least there should be 1 point

            // Save the header text into the file
            bw.Write($"{StringResources.FileHeader01} ({_settings.AppCultureName})");
            bw.Write(nStart.AddSeconds(ArrIndexInit / nSampleFreq));
            bw.Write(nStart.AddSeconds((Data.Length - 1 + ArrIndexInit) / nSampleFreq));
            bw.Write(nTime.Days);
            bw.Write(nTime.Hours);
            bw.Write(nTime.Minutes);
            bw.Write(nTime.Seconds);
            bw.Write(nTime.Milliseconds);
            bw.Write(1);
            bw.Write(Data.Length);
            bw.Write(nSampleFreq);
            bw.Write(Results.Average);
            bw.Write(Results.Maximum);
            bw.Write(Results.Minimum);
            bw.Write(Results.FractalDimension);
            bw.Write(Results.FractalVariance);
            bw.Write(Results.ApproximateEntropy);
            bw.Write(Results.SampleEntropy);
            bw.Write(Results.ShannonEntropy);
            bw.Write(Results.EntropyBit);
            bw.Write(Results.IdealEntropy);

            bw.Write($"{StringResources.FileHeader21}\t{SeriesName}");

            // Save the numerical values
            for (int j = 0; j < Data.Length; j++)
            {
                bw.Write(nStart.AddSeconds((j + ArrIndexInit) / nSampleFreq));
                bw.Write(Data[j]);   
            }

            // Success!
            result = true;
        }
        catch (Exception ex)
        {
            // Show error message
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(String.Format(StringResources.MsgBoxErrorSaveData, ex.Message),
                    StringResources.MsgBoxErrorSaveDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        
        return result;
    }

    /// <summary>
    /// Default saving, reverting to SaveTextData function.
    /// </summary>
    /// <param name="FileName">Path (including name) of the sig file</param>
    /// <param name="Data">Array of values to be saved</param>
    /// <param name="ArrIndexInit">Offset index of _signalData</param>
    /// <param name="SeriesName">Name of the serie data to be saved</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool SaveDefaultData(string FileName, double[] Data, int ArrIndexInit, string? SeriesName)
    {
        return SaveTextData(FileName, Data, ArrIndexInit, SeriesName);
    }

    /// <summary>
    /// Results saving function
    /// </summary>
    /// <param name="FileName">Path (including name) of the results file</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private bool SaveResultsData(string FileName)
    {
        bool result = false;

        try
        {
            using var fs = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            using var sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            sw.WriteLine($"{StringResources.FileHeader01} ({_settings.AppCultureName})");
            sw.WriteLine(Results.ToString(_settings.AppCulture));
            sw.WriteLine();
            sw.WriteLine($"{StringResources.PlotFFTXLabel}\t{StringResources.PlotFFTYLabelMag}\t{StringResources.PlotFFTYLabelPow}");

            // Save the numerical values
            for (int j = 0; j < Results.FFTfrequencies.Length; j++)
            {
                //trying to write data to text file
                sw.WriteLine($"{Results.FFTfrequencies[j].ToString("0.########", _settings.AppCulture)}\t" +
                    $"{Results.FFTmagnitude[j].ToString("0.########", _settings.AppCulture)}\t" +
                    $"{Results.FFTpower[j].ToString("0.########", _settings.AppCulture)}"
                    );
            }

            // Success!
            result = true;

        }
        catch (Exception ex)
        {
            // Show error message
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(String.Format(StringResources.MsgBoxErrorSaveData, ex.Message),
                    StringResources.MsgBoxErrorSaveDataTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        return result;
    }
}
