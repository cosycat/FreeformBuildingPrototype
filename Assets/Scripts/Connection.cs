using System;

public class Connection {
    public ConnectionPoint StartConnectionPoint { get; }
    public ConnectionPoint EndConnectionPoint { get; }

    public Connection(ConnectionPoint startConnectionPoint, ConnectionPoint endConnectionPoint) {
        StartConnectionPoint = startConnectionPoint;
        EndConnectionPoint = endConnectionPoint;
    }

    public event Action OnConnectionRemoved;
    
    internal void Remove() {
        OnConnectionRemoved?.Invoke();
    }

}