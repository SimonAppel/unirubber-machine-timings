using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

public class KeyboardListenerService
{
    private static readonly ConcurrentQueue<char> KeyPressQueue = new();

    public KeyboardListenerService() { }

    private async Task CharListening()
    {
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true).KeyChar;
                KeyPressQueue.Enqueue(key);
                Console.WriteLine($"Key Pressed: {key}");
            }

            Thread.Sleep(10); // Small delay to reduce CPU usage
            Console.WriteLine("Keyboard listener stopped");
        }
    }

    private async Task MakeWord() { }

    public async Task StartKBListening()
    {
        await CharListening();
    }

    public async Task StopKBListening() { }
}
