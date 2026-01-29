// Program.cs
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace KeyboardOrchestra
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        static IWavePlayer? waveOut;
        static MixingSampleProvider? mixer;
        static HashSet<string> activeNotes = new HashSet<string>();
        static Dictionary<int, double> PianoKeys = new Dictionary<int, double>();

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "Recursive Piano Engine";

                int[] vKeys = {
                    0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x30,
                    0x51, 0x57, 0x45, 0x52, 0x54, 0x59, 0x55, 0x49, 0x4F, 0x50,
                    0x41, 0x53, 0x44, 0x46, 0x47, 0x48, 0x4A, 0x4B, 0x4C,
                    0x5A, 0x58, 0x43, 0x56, 0x42, 0x4E, 0x4D
                };

                InitializeFullVirtualPianoRecursive(vKeys, 0);

                waveOut = new WaveOutEvent { DesiredLatency = 40 };
                var format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
                mixer = new MixingSampleProvider(format) { ReadFully = true };

                waveOut.Init(mixer);
                waveOut.Play();

                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==================================================");
                Console.WriteLine("    RECURSIVE PIANO ENGINE                        ");
                Console.WriteLine("==================================================");
                Console.WriteLine(" ESC за изход | Crystal Sine Wave                 ");
                Console.WriteLine("==================================================");

                while (true)
                {
                    if ((GetAsyncKeyState(0x1B) & 0x8000) != 0) break;

                    bool shiftPressed = (GetAsyncKeyState(0x10) & 0x8000) != 0;

                    foreach (var note in PianoKeys)
                    {
                        bool isKeyPressed = (GetAsyncKeyState(note.Key) & 0x8000) != 0;
                        string noteId = note.Key.ToString() + (shiftPressed ? "S" : "N");

                        if (isKeyPressed)
                        {
                            if (!activeNotes.Contains(noteId))
                            {
                                double finalFreq = note.Value;
                                if (shiftPressed) finalFreq *= Math.Pow(2, 1.0 / 12.0);

                                PlayTone(finalFreq);
                                activeNotes.Add(noteId);

                                char c = (char)note.Key;
                                if (shiftPressed)
                                {
                                    Console.BackgroundColor = ConsoleColor.White;
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.Write($"[{c}]");
                                }
                                else
                                {
                                    Console.BackgroundColor = (ConsoleColor)(note.Key % 14 + 1);
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write($" {char.ToLower(c)} ");
                                }
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            activeNotes.Remove(note.Key.ToString() + "N");
                            activeNotes.Remove(note.Key.ToString() + "S");
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                waveOut?.Stop();
                waveOut?.Dispose();
            }
        }

        static void InitializeFullVirtualPianoRecursive(int[] keys, int index)
        {
            if (index >= keys.Length) return;

            double baseFreq = 65.41;
            int[] intervals = { 0, 2, 4, 5, 7, 9, 11 };

            int octave = index / 7;
            int step = index % 7;
            int semitones = (octave * 12) + intervals[step];

            double freq = baseFreq * Math.Pow(2, semitones / 12.0);

            if (!PianoKeys.ContainsKey(keys[index]))
            {
                PianoKeys.Add(keys[index], freq);
            }

            InitializeFullVirtualPianoRecursive(keys, index + 1);
        }

        static void PlayTone(double frequency)
        {
            var signal = new SignalGenerator()
            {
                Gain = 0.18,
                Frequency = frequency,
                Type = SignalGeneratorType.Sin
            };

            var takeSignal = new OffsetSampleProvider(signal)
            {
                TakeSamples = (int)(44100 * 2 * 1.2)
            };

            var fadeSignal = new FadeInOutSampleProvider(takeSignal, false);
            fadeSignal.BeginFadeOut(700);

            mixer?.AddMixerInput(fadeSignal);
        }
    }
} 