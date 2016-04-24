using System;
using System.Diagnostics;
using System.IO;

namespace Parcheador_Whats_Shenmue
{
    partial class Patcher
    {
        string[] ReadTracksNames(string GDIFile)
        {
            //Check if it's a valid GDI
            byte[] Data;
            using (FileStream fs = new FileStream(GDIFile, FileMode.Open, FileAccess.Read))
            {
                Data = new byte[2];
                fs.Read(Data, 0, Data.Length);
            }

            if ((Data[0] != 0x33 && Data[0] != 0x36 && Data[0] != 0x37 && Data[0] != 0x39) || (Data[1] != 0xA && Data[1] != 0xD))
            {
                string[] ErrorStr = { "Error" };
                return ErrorStr;
            }

            //It's a valid GDI, so read it
            string[] Lines = File.ReadAllLines(GDIFile);
            char[] DelimiterChars = { ' ' };

            int NumberOfLines = Convert.ToInt32(Lines[0]);
            string[] Tracks = new string[NumberOfLines];

            for (int n = 0; n < NumberOfLines; n++)
            {
                string[] Words = Lines[n + 1].Split(DelimiterChars);
                Tracks[n] = Path.Combine(Path.GetDirectoryName(GDIFile), Words[4]);
            }

            return Tracks;
        }

        int ReadLBA(string FileName)
        {
            string[] Lines = File.ReadAllLines(FileName);
            char[] DelimiterChars = { ' ' };

            int NumberOfLines = Convert.ToInt32(Lines[0]);

            string[] Words = Lines[NumberOfLines].Split(DelimiterChars);

            return Convert.ToInt32(Words[1]) + 150;
        }

        int StartProcess(string Process, string Arguments, string WorkingDir, bool Log)
        {
            Process process = new Process();

            process.StartInfo.FileName = Path.Combine(Globals.UtilsPath, Process);
            process.StartInfo.Arguments = Arguments;

            process.StartInfo.RedirectStandardOutput = Log;
            process.StartInfo.RedirectStandardInput = Log;
            process.StartInfo.RedirectStandardError = Log;

            if (string.IsNullOrEmpty(WorkingDir)) process.StartInfo.WorkingDirectory = Globals.AppPath;
            else process.StartInfo.WorkingDirectory = WorkingDir;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            LogStream.Write(process.StartInfo.FileName + " " + process.StartInfo.Arguments + Environment.NewLine);

            if (Log == true)
            {
                LogStream.Write("################# OUTPUT STREAM #################" + Environment.NewLine + process.StandardOutput.ReadToEnd() + "#################################################" + Environment.NewLine + Environment.NewLine);
                LogStream.Write("################# ERROR  STREAM #################" + Environment.NewLine + process.StandardError.ReadToEnd() + "#################################################" + Environment.NewLine + Environment.NewLine);
            }

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string Command = process.StartInfo.FileName + " " + process.StartInfo.Arguments;
                OutputMessage(String.Format(Localization.MsgBoxProcessError, process.ExitCode.ToString(), Command), MessageType.Error);
            }

            int ExitCode = process.ExitCode;

            process.Dispose();

            return ExitCode;
        }

        private bool ApplyXDelta(int DiscIndex, string FileName, string XDelta)
        {
            if (_GDI[DiscIndex].FileName != null)
            {
                if (StartProcess("xdelta.exe", String.Format("-d -s \"{0}\" \"{1}\" \"{0}2\"", FileName, XDelta), null, true) != 0) return false;
                File.Delete(FileName);
                File.Move(FileName + "2", FileName);
            }

            return true;
        }

        void GenerateDummyFile(string FileName, long Size)
        {
            using (FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                fs.SetLength(Size);
            }
        }

        void TrimFile(string SourceFile, string DestinationFile, int Threshold, int Align)
        {
            byte[] Data = File.ReadAllBytes(SourceFile);

            int StartOffset = -1, EndOffset = -1;

            for (int n = 0; n < Data.Length; n++)
            {
                if (Data[n] != 0)
                {
                    if (StartOffset == -1) StartOffset = n;
                    EndOffset = n;
                }
            }

            if (StartOffset == -1 && EndOffset == -1) //All the file contains zeroes.
            {
                File.Copy(SourceFile, DestinationFile);
                return;
            }

            StartOffset -= Threshold;
            StartOffset -= (StartOffset % Align);

            EndOffset += Threshold;
            EndOffset += Align - (EndOffset % Align);

            if (StartOffset < 0) StartOffset = 0;
            if (EndOffset > Data.Length) EndOffset = Data.Length;

            using (FileStream fs = new FileStream(DestinationFile, FileMode.Create, FileAccess.Write))
            {
                fs.Write(Data, StartOffset, EndOffset - StartOffset);
            }

            return;
        }
    }
}
