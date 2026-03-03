using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

public class ZeroMqManager : MonoBehaviorSingleton<ZeroMqManager>
{
    private const int PubPort = 5555;
    private const int PullPort = 5556;

    private PublisherSocket _pubSocket;
    private PullSocket _pullSocket;

    private Thread _listenerThread;
    private bool _isRunning;

    private readonly ConcurrentQueue<(string topic, string payload)> _messageQueue = new();

    public event Action<string, string> OnPythonEventReceived;

    private void Awake()
    {
        InitializeSockets();
    }

    private void InitializeSockets()
    {
        AsyncIO.ForceDotNet.Force();
        _isRunning = true;

        _pubSocket = new PublisherSocket();
        _pubSocket.Options.SendHighWatermark = 1000;
        _pubSocket.Bind($"tcp://*:{PubPort}");

        _pullSocket = new PullSocket();
        _pullSocket.Bind($"tcp://*:{PullPort}");

        _listenerThread = new Thread(ListenerWork);
        _listenerThread.Start();

        Debug.Log($"<color=green>ZMQ Manager Started.</color> PUB: {PubPort}, PULL: {PullPort}");

        SetInstance(this);
    }

    private void ListenerWork()
    {
        while (_isRunning)
        {
            try
            {
                if (_pullSocket.TryReceiveFrameString(out string message))
                {
                    _messageQueue.Enqueue(("Generic", message));
                }
                else
                {
                    Thread.Sleep(20);
                }
            }
            catch (Exception e)
            {
                if (_isRunning) Debug.LogError($"ZMQ Listener Error: {e.Message}");
            }
        }
    }

    private void Update()
    {
        while (_messageQueue.TryDequeue(out (string topic, string payload) msg))
        {
            try
            {
                OnPythonEventReceived?.Invoke(msg.topic, msg.payload);
                Debug.Log($"Получено от Python: {msg.payload}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing ZMQ message: {e.Message}");
            }
        }
    }

    /// <summary>
    /// Отправка команды скриптам.
    /// </summary>
    /// <param name="targetScript">Имя скрипта или "ALL"</param>
    /// <param name="command">Название команды</param>
    /// <param name="data">JSON данные</param>
    public void SendCommand(string targetScript, string command, object data = null)
    {
        if (!_isRunning) return;

        string jsonPayload = data != null ? JsonConvert.SerializeObject(data) : "{}";

        //"TARGET command"

        _pubSocket.SendMoreFrame(targetScript)
                  .SendMoreFrame(command)
                  .SendFrame(jsonPayload);
    }

    public void StopAllScripts()
    {
        SendCommand("ALL", "EXIT");
    }

    private void OnApplicationQuit()
    {
        EndConnection();
    }

    private void OnDestroy()
    {
        EndConnection();
    }

    private void EndConnection()
    {
        StopAllScripts();

        _isRunning = false;

        _listenerThread?.Join(500);

        _pubSocket?.Close();
        _pubSocket?.Dispose();

        _pullSocket?.Close();
        _pullSocket?.Dispose();

        NetMQConfig.Cleanup();
    }
}