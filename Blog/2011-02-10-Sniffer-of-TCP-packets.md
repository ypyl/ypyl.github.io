---
layout: post
title: Sniffer of TCP packets
date: 2011-02-10

tags: dotnet
categories: programming
---
Поставновка задачи: Необходимо создать сниффер, который позволяет получить инфрмацию, которая хранится внутри tcp-пакета (например, мы снифферим загрузку html-страниц). Следует отметить, что tcp-пакеты могут приходить на машину назначение беспорядочно. Таким образом класс Sniffer, используя библиотеку WinPcap упорядочивает все пакеты.

Проект на VS2008 [SharkTrace](https://docs.google.com/open?id=0BwVmorgjT-W1OWVmY2M1MzktNGRhNS00MjEwLWIyOTktMWMzODI3YmM3Mzc3).

Важно! Библиотека SharpPcap уже давно имеет новый интерфейс.

Далее в тексте, под соединением я понимаю соответствие адресов и портов на клиенте и сервере.

Для работы необходима открытая библиотека SharpPcap, она предоставляет удобный интерфейс для работы с приложением WinPcap ([Sharppcap](http://sourceforge.net/projects/sharppcap/)).

Класс соединения используется для хранения информации об адресе клиента и сервера, о портах, а так же об текущем ожидаемом tcp-пакете.

Больше о флагах tcp-пакета можно посмотреть [http://www.firewall.cx.](http://www.firewall.cx/)

```cs
///<summary>
///class for connection
///</summary>
internal class Connection
{
    public long ClientAddress; // client initiating the connection
    public int ClientPort;
    public long HostAddress; // host receiving the connection
    public int HostPort;
    public long ClientSyn; // starting syn sent from client
    public long HostSyn; // starting syn sent from host;
    public long NextClientSeq; // this must be in SequenceNumber field of TCP packet if it is from client
    public long NextHostSeq; // this must be in SequenceNumber field of TCP packet if it is from host
    public bool HostClosed;
    public bool ClientClosed;
    public long TimeIdentifier;
    public bool ThreeWayCompleted = false; // three way connection is completed

    // Fragments , used when we get newer packets that expected.
    // so we need to wait for expected before adding them.
    public SortedDictionary<long, TcpPacket> HostFragments = new SortedDictionary<long, TcpPacket>();
    public SortedDictionary<long, TcpPacket> ClientFragments = new SortedDictionary<long, TcpPacket>();

    // returns client ip:client port as a string
    public string GetClientAddressPort()
    {
        return string.Format("{0}:{1}", new IPAddress(ClientAddress).ToString(), ClientPort);
    }

    // returns host ip:host port as a string
    public string GetHostAddressPort()
    {
        return string.Format("{0}:{1}", new IPAddress(HostAddress).ToString(), HostPort);
    }

    // packet is from host
    public bool IsFromHost(TcpPacket tcp)
    {
        return ClientAddress == ((IpPacket)tcp.ParentPacket).DestinationAddress.Address &&
        ClientPort == tcp.DestinationPort &&
        HostAddress == ((IpPacket)tcp.ParentPacket).SourceAddress.Address &&
        HostPort == tcp.SourcePort;
    }

    // packet is from client
    public bool IsFromClient(TcpPacket tcp)
    {
        return ClientAddress == ((IpPacket)tcp.ParentPacket).SourceAddress.Address &&
        ClientPort == tcp.SourcePort &&
        HostAddress == ((IpPacket)tcp.ParentPacket).DestinationAddress.Address &&
        HostPort == tcp.DestinationPort;
    }

    public Connection(long clientAddress, int clientPort, long hostAddress, int hostPort, long clientSyn)
    {
        this.ClientAddress = clientAddress;
        this.ClientPort = clientPort;
        this.HostAddress = hostAddress;
        this.HostPort = hostPort;
        this.ClientSyn = clientSyn;
    }
}
```

Итак данный класс хранит информацию о соединении. В сниффере я пропускаю процесс установления соединения (так называемое тройное рукопожатие) так как возможно мы запустим сниффер когда уже соединения было установлено (о нем можно почитать [wikipedia.org](http://en.wikipedia.org/wiki/Transmission_Control_Protocol#Connection_establishment)). Поэтому происходит попытка создания соединения, если оно еще не было создано. Ловятся пакеты, которые имеют "полезную" информацию в себе (payload data).

Больше о TCP пакетах [wikipedia.org](http://ru.wikipedia.org/wiki/TCP).

Методы RunSniffer и StopSniffer - запускают и останавливают сниффер соответственно. Метод AssemblePacket на вход получает tcp-пакет, и проверяет существует ли соединение, которому "принадлежит" этот пакет. Если нет - создается, если да - то работает логика по упорядочиванию пакетов. Два абстрактных метода позволяют получить доступ к "полезным" данным последовательно (AddHostData и AddClientData)

```cs
/// <summary>
/// Main class for sniffering
/// </summary>
/// <remarks>
/// 1. I see that there is new version of SharpPcap library with new interface (29/11/2011)
/// So i just try to update some useful information about this class
///
/// 2. I believe that SynchronizatedConnection class is just
/// class SynchronizatedConnection : Connection
/// {
///     private object syncObject = new object();
///
///     public Synchro {get {return syncObject;}}
/// }
///
/// 3. Coordinator is class with constants.
/// </remarks>
internal abstract class Sniffer
{
    private Timer timer;
    private object synchronizationObjectForConnection = new object();
    private List<SynchronizatedConnection> Connections = new List<SynchronizatedConnection>();
    private LibPcapLiveDevice _device;
    private readonly List<System.Net.IPAddress> _hosts;

    // when connected
    public abstract void Connected(Object conn);
    // when disconnected
    public abstract void Disconnected(Object conn);
    public abstract void AddHostData(byte[] data, string host);
    public abstract void AddClientData(byte[] data, string client);

    private void ClearConnections(object source)
    {
        //sometimes purely the collection, just in case
        lock (synchronizationObjectForConnection)
        {
            foreach (var key in Connections.Where(k => (DateTime.Now.ToFileTimeUtc() - k.TimeIdentifier) > 600000000 * Coordinator.Config.HowLongWeSaveTransaction.TotalMinutes).ToList())
            {
                Connections.Remove(key);
            }
        }
    }

    /// <summary>
    /// Create the exemplare of the class
    /// </summary>
    public Sniffer(List<System.Net.IPAddress> hosts)
    {
        timer = new Timer(ClearConnections, null, Coordinator.Config.HowLongWeSaveTransaction, Coordinator.Config.HowLongWeSaveTransaction);
        _hosts = hosts;
    }

    /// <summary>
    /// Start the tracing
    /// </summary>
    /// <param name="filter">See http://www.cs.ucr.edu/~marios/ethereal-tcpdump.pdf </param>
    public void RunSniffer(string filter)
    {
        var devices = LibPcapLiveDeviceList.Instance;
        //
        _device = devices[Coordinator.Config.ListeningInterface];
        //Register our handler function to the 'packet arrival' event
        _device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
        //Open the device for capturing
        int readTimeoutMilliseconds = 1000;
        _device.Open(DeviceMode.Normal, readTimeoutMilliseconds);
        // tcpdump filter to capture only TCP/IP packets
        _device.Filter = filter;
        // Start capture packets
        _device.StartCapture();
    }

    /// <summary>
    /// Stop the tracing
    /// </summary>
    public void StopSniffer()
    {
        _device.StopCapture();
        _device.Close();
    }

    /// <summary>
    /// Catch the packet
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void device_OnPacketArrival(object sender, CaptureEventArgs e)
    {
        try
        {
            // try to get TCP packet from Ip packet
            var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            if (packet is PacketDotNet.EthernetPacket)
            {
                var eth = ((PacketDotNet.EthernetPacket)packet);
                var ip = PacketDotNet.IpPacket.GetEncapsulated(packet);
                if (ip != null)
                {
                    var tcp = PacketDotNet.TcpPacket.GetEncapsulated(packet);
                    if (tcp != null)
                    {
                        AssemblePacket(tcp);
                    }
                }
            }
        }

        // sometimes converting doesn't work - don't worry about it
        catch (InvalidOperationException ex)
        {
        }
    }

    /// <summary>
    /// Parse TCP packet
    /// </summary>
    /// <param name="tcp">TCP packet</param>
    privDescription. There are modules that have the input and output parameters (type and number of parameters may be different). The goal is to select some first modules that have no input parameters, then the modules whose inputs are the output parameters of the previously selected modules and so on. Until the last module will not output parameters.

The database stores all displayed modules and displays of all parameters where the parameter mapping to the mapping of modules is many-to-one.
Initially, we choose to do all the settings using a simple SQL query.ate void AssemblePacket(TcpPacket tcp)
    {
        // pass the packets that :
        // 1. tcp.Syn && tcp.PayloadData.Length == 0 - sent for synchronization
        // 2. tcp.PayloadData.Length > 0 - no useful data in packet
        // 3. tcp.Fin || tcp.Rst - connection is finished or reseted
        if (!(tcp.Syn && tcp.PayloadData.Length == 0) && (tcp.PayloadData.Length > 0) && !(tcp.Fin || tcp.Rst))
        {
            SynchronizatedConnection conn;
            // try to find connection in collection
            bool? res = IsTcpFromClient(tcp, out conn);

            if (res == null)
            {
                // connection is new
                SynchronizatedConnection con;
                if (_hosts.Contains(((IpPacket)tcp.ParentPacket).SourceAddress))
                {
                    // packet from host to client
                    conn = new SynchronizatedConnection(((IpPacket)tcp.ParentPacket).DestinationAddress.Address,
                    tcp.DestinationPort, ((IpPacket)tcp.ParentPacket).SourceAddress.Address, tcp.SourcePort, tcp.SequenceNumber);
                    conn.ClientSyn = tcp.AcknowledgmentNumber;
                    conn.HostSyn = tcp.SequenceNumber;
                    conn.NextClientSeq = tcp.AcknowledgmentNumber;
                    conn.NextHostSeq = tcp.SequenceNumber;
                    res = false;
                }
        Description. There are modules that have the input and output parameters (type and number of parameters may be different). The goal is to select some first modules that have no input parameters, then the modules whose inputs are the output parameters of the previously selected modules and so on. Until the last module will not output parameters.

The database stores all displayed modules and displays of all parameters where the parameter mapping to the mapping of modules is many-to-one.
Initially, we choose to do all the settings using a simple SQL query.        else
                {
                    // packet from host to client
                    conn = new SynchronizatedConnection(((IpPacket)tcp.ParentPacket).SourceAddress.Address,
                    tcp.SourcePort, ((IpPacket)tcp.ParentPacket).DestinationAddress.Address, tcp.DestinationPort, tcp.SequenceNumber);
                    conn.ClientSyn = tcp.SequenceNumber;
                    conn.HostSyn = tcp.AcknowledgmentNumber;
                    conn.NextHostSeq = tcp.AcknowledgmentNumber;
                    conn.NextClientSeq = tcp.SequenceNumber;
                    res = true;
                }
                conn.TimeIdentifier = DateTime.Now.ToFileTimeUtc();
                conn.ThreeWayCompleted = true;
                lock (synchronizationObjectForConnection)
                {
                    Connections.Add(conn);
                }
            }
            if (res == true)
            {
                // from client
                lock (conn.Synchro)
                {
                    if (tcp.SequenceNumber < conn.NextClientSeq)
                    // old packet
                    {
                        // just drop these for now
                        return;
                    }
                    if (tcp.SequenceNumber > conn.NextClientSeq)
                    // out of order data
                    {
                        if (!conn.ClientFragments.ContainsKey(tcp.SequenceNumber))
                        {
                            conn.ClientFragments.Add(tcp.SequenceNumber, tcp);
                        }
                        else
                        // expect new data to be better?
                        {
                            conn.ClientFragments[tcp.SequenceNumber] = tcp;
                        }
                    }
                    else
                    {
                        while (tcp.SequenceNumber == conn.NextClientSeq)
                        {
                            conn.ClientFragments.Remove(tcp.SequenceNumber);
                            // remove fragment
                            if (tcp.PayloadData.Length == 0)
                                break;
                            // new NextClientSeq for client packet
                            conn.NextClientSeq = conn.NextClientSeq + tcp.PayloadData.Length;
                            // data should be valid here.
                            AddClientData(GetUsefulData(tcp), GetIdOfConnection(conn));
                            if (conn.ClientFragments.ContainsKey(conn.NextClientSeq))
                            // check if we have newer fragments which will now fit.
                            {
                                tcp = conn.ClientFragments[conn.NextClientSeq];
                            }
                            else
                                break;
                        }
                    }
                }
            }
            else
            {
                //from host
                lock (conn.Synchro)
                {
                    if (tcp.SequenceNumber < conn.NextHostSeq)
                    // old packet
                    {
                        // just drop these for now
                        return;
                    }
                    if (tcp.SequenceNumber > conn.NextHostSeq)
                    // newer out of order data
                    {
                        if (!conn.HostFragments.ContainsKey(tcp.SequenceNumber))
                        {
                            conn.HostFragments.Add(tcp.SequenceNumber, tcp);
                        }
                        else
                        {
                            conn.HostFragments[tcp.SequenceNumber] = tcp;
                        }
                    }
                    else
                    //
                    {
                        while (tcp.SequenceNumber == conn.NextHostSeq)
                        // on time
                        {
                            conn.HostFragments.Remove(tcp.SequenceNumber);
                            // remove fragment
                            if (tcp.PayloadData.Length == 0)
                                break;
                            conn.NextHostSeq = conn.NextHostSeq + tcp.PayloadData.Length;
                            // data should be valid here
                            AddHostData(GetUsefulData(tcp), GetIdOfConnection(conn));
                            if (conn.HostFragments.ContainsKey(conn.NextHostSeq))
                            // check if we have newer fragments which will now fit.
                            {
                                tcp = conn.HostFragments[conn.NextHostSeq];
                            }
                            else
                                break;
                        }
                    }
                }
            }
        }
    }

    private static string GetIdOfConnection(SynchronizatedConnection conn)
    {
        return conn.ClientAddress.ToString() + conn.ClientPort.ToString() + conn.HostAddress.ToString()
        + conn.HostPort.ToString();
    }

    private bool? IsTcpFromClient(TcpPacket tcp, out SynchronizatedConnection conn)
    {
        conn = null;
        lock (synchronizationObjectForConnection)
        {
            foreach (var connection in Connections)
            {
                if (connection.IsFromClient(tcp))
                {
                    conn = connection;
                    return true;
                }
                if (connection.IsFromHost(tcp))
                {
                    conn = connection;
                    return false;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Get Payload Data from tcp packet
    /// </summary>
    /// <param name="tcp">TCP packet</param>
    /// <returns>bytes of useful data</returns>
    protected static byte[] GetUsefulData(TcpPacket tcp)
    {
        var data = new byte[tcp.Bytes.Length - tcp.DataOffset * 4];
        for (int i = tcp.DataOffset * 4; i < tcp.Bytes.Length; i++)
        {
            data[i - tcp.DataOffset * 4] = tcp.Bytes[i];
        }
        return data;
    }
}
```

Проблемы данного класса/решения:
1. Если "словится" первый пакет, который является неправильным - возникает проблема с получением дальнейшей информации, которая идет по данному соединению;
2. Не проверяется контрольная сумма принятого пакета;
3. Все манипуляции (приведение, извлечение TCP пакета из IP пакета, работа с этим пакетом) довольно времяемкие операции. Необходимо задавать хороший фильтр

```cs
(public void RunSniffer(string filter))
```

или советую отказаться от этого решения для наблюдения за высоконагруженным сетевым траффиком.
