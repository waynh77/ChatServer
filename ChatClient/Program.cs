using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("[CLIENT] Mencoba terhubung ke Server (127.0.0.1:5000)...");

            // 1. Inisialisasi koneksi TCP ke Server[cite: 1]
            using TcpClient client = new TcpClient("127.0.0.1", 5000);
            Console.WriteLine("[CLIENT] Terhubung ke Server (ESTABLISHED)!");

            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            // 2. Background Task untuk membaca pesan dari Server secara kontinu
            var receiveTask = Task.Run(async () => {
                try
                {
                    string? incomingMessage;
                    while ((incomingMessage = await reader.ReadLineAsync()) != null)
                    {
                        Console.WriteLine($"\n[Server]: {incomingMessage}");
                        Console.Write("[Client]: ");
                    }
                }
                catch
                {
                    Console.WriteLine("\n[CLIENT] Koneksi terputus dari server.");
                }
            });

            // 3. Loop utama untuk mengirim pesan dari Client ke Server
            Console.WriteLine("[SKENARIO CHAT DIMULAI] Ketik pesan Anda, lalu tekan Enter. Ketik 'exit' untuk keluar.");
            Console.Write("[Client]: ");
            while (true)
            {
                string? message = Console.ReadLine();
                if (string.IsNullOrEmpty(message)) continue;

                if (message.ToLower() == "exit")
                {
                    break;
                }

                await writer.WriteLineAsync(message);
                Console.Write("[Client]: ");
            }

            client.Close();
            Console.WriteLine("[CLIENT] Sesi ditutup.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] Gagal terhubung ke server: {e.Message}");
        }
    }
}