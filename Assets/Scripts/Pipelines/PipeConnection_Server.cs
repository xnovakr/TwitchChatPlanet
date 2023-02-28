using System;
using System.IO;
using System.Text;
using System.IO.Pipes;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeConnection_Server : MonoBehaviour
{
    public static PipeConnection_Server Instasnce { get; private set; }

    private Thread serverReadThread;
    private Thread serverWriteThread;

    private StreamString streamReadString;
    private StreamString streamWriteString;

    private Queue<string> readQueue;
    private Queue<string> writeQueue;

    private object readLock;
    private object writeLock;

    private void Start()
    {
        Instasnce = this;

        serverReadThread = new Thread(ServerThread_Read);
        serverReadThread.Start();
        serverWriteThread = new Thread(ServerThread_Write);
        serverWriteThread.Start();

        readQueue = new Queue<string>();
        writeQueue = new Queue<string>();

        readLock = new object();
        writeLock = new object();

    }
    private void Update()
    {
        if (readQueue != null)
        {
            ReadMessages();
        }
    }
    private void ServerThread_Read()
    {
        NamedPipeServerStream namedPipeServerStream = new NamedPipeServerStream("ServerRead_ClientWrite", PipeDirection.In);

        namedPipeServerStream.WaitForConnection();
        Debug.Log("Client Read has connected!");
        try
        {
            streamReadString = new StreamString(namedPipeServerStream);

            while (true)
            {
                string message = streamReadString.ReadString();
                //Debug.Log("RCV: " + message);

                //lock to make it threadsafe
                lock (readLock)
                {
                    readQueue.Enqueue(message);
                }

                Thread.Sleep(10);
            }
        }
        catch(Exception e)
        {
            Debug.Log("ERROR: " + e);
        }
        namedPipeServerStream.Close();
    }
    public void ServerThread_Write()
    {
        NamedPipeServerStream pipeWriteServer = new NamedPipeServerStream("ServerWrite_ClientRead", PipeDirection.Out);

        //wait for connection
        pipeWriteServer.WaitForConnection();
        Debug.Log("Client Write Connected!");

        try
        {
            streamWriteString = new StreamString(pipeWriteServer);

            SendCommand("Hello from the Server!"); // this is creating owerflov becouse its not called from main thread

            while (true)
            {
                string messageQueue = null;

                lock (writeLock)
                {
                    if (writeQueue.Count > 0)
                    {
                        messageQueue = writeQueue.Dequeue();
                    }
                }
                if (messageQueue != null)
                {
                    //Debug.Log("SND: " + messageQueue);
                    streamWriteString.WriteString(messageQueue);
                }
                Thread.Sleep(100);
            }
        }
        catch(Exception e)
        {
            Debug.Log("ERROR: " + e);
        }
        Debug.Log("Pipe Write Closed!");
        pipeWriteServer.Close();
    }
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len = 0;

            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
    public void DestroySelf()
    {
        serverReadThread.Abort();
        serverWriteThread.Abort();
        Debug.Log("Server threads Aborted!");
    }
    private void OnApplicationQuit()
    {
        DestroySelf();
    }
    private void ReadMessages()
    {
        //hook onto the event to read messages
        lock (readLock)
        {   
            if (readQueue.Count > 0)
            {
                string message = readQueue.Dequeue();
                //Debug.Log(message);
                CommandFactory.SelectCommand(message);
            }
        }
    }
    public void SendCommand(string message)
    {
        writeQueue.Enqueue(message);
    }
}
