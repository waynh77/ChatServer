using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
class Program
{
    static async Task Main(string[] args)
    {
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        int port = 5000;

        // 1. Inisialisasi TcpListener pada IP dan Port tertentu
        TcpListener server = new TcpListener(ip, port);
        server.Start();
        Console.WriteLine($"[SERVER] Standby di {ip}:{port}... Menunggu client terhubung...");

        // 2. Menerima koneksi masuk dari client (3-Way Handshake terjadi di sini)
        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("[SERVER] Client berhasil terhubung! (ESTABLISHED)");

        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        // 3. Background Task untuk membaca pesan yang dikirim oleh Client secara kontinu
        var receiveTask = Task.Run(async () => {
            try
            {
                string? incomingMessage;
                while ((incomingMessage = await reader.ReadLineAsync()) != null)
                {
                    Console.WriteLine($"\n[Client]: {incomingMessage}");
                    Console.Write("[Server]: ");
                }
            }
            catch
            {
                Console.WriteLine("\n[SERVER] Koneksi diputus oleh client.");
            }
        });

        // 4. Loop utama untuk mengirim pesan dari Server ke Client
        Console.WriteLine("[SKENARIO CHAT DIMULAI] Ketik pesan Anda, lalu tekan Enter. Ketik 'exit' untuk keluar.");
        Console.Write("[Server]: ");
        while (true)
        {
            string? message = Console.ReadLine();
            if (string.IsNullOrEmpty(message)) continue;

            if (message.ToLower() == "exit")
            {
                break;
            }

            await writer.WriteLineAsync(message);
            Console.Write("[Server]: ");
        }

        // 5. Tutup koneksi
        client.Close();
        server.Stop();
        Console.WriteLine("[SERVER] Sesi ditutup.");
    }
}