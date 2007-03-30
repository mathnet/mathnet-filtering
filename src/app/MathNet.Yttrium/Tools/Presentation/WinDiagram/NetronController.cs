using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Netron.Diagramming.Core;
using Netron.Diagramming.Win;

using MathNet.Symbolics.Mediator;
using MathNet.Symbolics.Presentation.Shapes;
using MathNet.Symbolics.Presentation.FlyweightShapes;

namespace MathNet.Symbolics.Presentation.WinDiagram
{
    public class NetronController : ControllerBase
    {
        private DiagramControl _presentation;
        private Bridge _bridge;
        private Dictionary<Guid, Bridge> _mathBridges;
        private Dictionary<Document, Bridge> _netronBridges;

        private IFlyweightShape<SignalShape> _defaultSignalFly;
        private Dictionary<MathIdentifier, IFlyweightShape<SignalShape>> _signalFlies;
        private IFlyweightShape<BusShape> _defaultBusFly;
        private Dictionary<MathIdentifier, IFlyweightShape<BusShape>> _busFlies;
        private IFlyweightShape<PortShape> _defaultPortFly;
        private Dictionary<MathIdentifier, IFlyweightShape<PortShape>> _portFlies;

        private bool maskMDrives, maskNDrives;

        public event EventHandler ModelChanged;

        public NetronController(DiagramControl netronDiagram)
            : base()
        {
            _presentation = netronDiagram;
            _mathBridges = new Dictionary<Guid, Bridge>();
            _netronBridges = new Dictionary<Document, Bridge>();
            _presentation.OnEntityAdded += NetronEntityAddedHandler;
            _presentation.OnEntityRemoved += NetronEntityRemovedHandler;

            _signalFlies = new Dictionary<MathIdentifier, IFlyweightShape<SignalShape>>();
            _busFlies = new Dictionary<MathIdentifier, IFlyweightShape<BusShape>>();
            _portFlies = new Dictionary<MathIdentifier, IFlyweightShape<PortShape>>();
            _defaultSignalFly = new DefaultSignalShape();
            _defaultBusFly = new DefaultBusShape();
            _defaultPortFly = new DefaultPortShape();
            _portFlies.Add(new MathIdentifier("Transition", "PetriNet"), new PetriNetPortShape());

            Service<MathNet.Symbolics.Library.IPackageLoader>.Instance.LoadPackage("PetriNet");
        }

        #region Bridge
        private class Bridge
        {
            private IMathSystem _system;
            private Document _document;
            private Dictionary<Guid, SignalShape> _signals;
            private Dictionary<Guid, BusShape> _buses;
            private Dictionary<Guid, PortShape> _ports;

            public Bridge(IMathSystem system, Document document)
            {
                _system = system;
                _document = document;
                _signals = new Dictionary<Guid, SignalShape>();
                _buses = new Dictionary<Guid, BusShape>();
                _ports = new Dictionary<Guid, PortShape>();
            }

            public IMathSystem System
            {
                get { return _system; }
            }
            public Document Document
            {
                get { return _document; }
            }
            public IModel Model
            {
                get { return _document.Model; }
            }

            public Dictionary<Guid, SignalShape> Signals
            {
                get { return _signals; }
            }
            public Dictionary<Guid, BusShape> Buses
            {
                get { return _buses; }
            }
            public Dictionary<Guid, PortShape> Ports
            {
                get { return _ports; }
            }
        }

        private Bridge CreateBridge(Document document)
        {
            IMathSystem sys = Binder.GetInstance<IMathSystem>();
            Bridge bridge = new Bridge(sys, document);
            _mathBridges.Add(sys.InstanceId, bridge);
            _netronBridges.Add(document, bridge);
            return bridge;
        }
        private Bridge CreateBridge(IMathSystem system)
        {
            Document doc = new Document();
            Bridge bridge = new Bridge(system, doc);
            _mathBridges.Add(system.InstanceId, bridge);
            _netronBridges.Add(doc, bridge);
            return bridge;
        }
        #endregion

        #region Flyweight Shapes

        private IFlyweightShape<SignalShape> GetFlyweightForSignal(ICustomData value)
        {
            IFlyweightShape<SignalShape> ret;
            if(value != null && _signalFlies.TryGetValue(value.TypeId, out ret))
                return ret;
            return _defaultSignalFly;
        }

        private IFlyweightShape<BusShape> GetFlyweightForBus(ICustomData value)
        {
            IFlyweightShape<BusShape> ret;
            if(value != null && _busFlies.TryGetValue(value.TypeId, out ret))
                return ret;
            return _defaultBusFly;
        }

        private IFlyweightShape<PortShape> GetFlyweightForPort(IEntity entity)
        {
            IFlyweightShape<PortShape> ret;
            if(entity != null && _portFlies.TryGetValue(entity.EntityId, out ret))
                return ret;
            return _defaultPortFly;
        }

        #endregion

        #region System/Document Loading

        protected override void LoadSystem(IMathSystem system)
        {
            base.LoadSystem(system);
            Bridge bridge;
            if(!_mathBridges.ContainsKey(system.InstanceId))
                bridge = CreateBridge(system);
            else
                bridge = _mathBridges[system.InstanceId];
            _presentation.AttachToDocument(bridge.Document);
            _bridge = bridge;
            _bridge.Model.OnConnectorAttached += Model_OnConnectorAttached;
            _bridge.Model.OnConnectorDetached += Model_OnConnectorDetached;

            if(ModelChanged != null)
                ModelChanged(this, EventArgs.Empty);
        }

        protected override void UnloadSystem(IMathSystem system)
        {
            _bridge.Model.OnConnectorAttached -= Model_OnConnectorAttached;
            _bridge.Model.OnConnectorDetached -= Model_OnConnectorDetached;
            base.UnloadSystem(system);
            _bridge = null;
        }

        public void Load(IMathSystem system)
        {
            LoadSystem(system);
        }

        public void Load(Document document)
        {
            if(_bridge != null)
                UnloadSystem(_bridge.System);
            Bridge bridge;
            if(!_netronBridges.ContainsKey(document))
                bridge = CreateBridge(document);
            else
                bridge = _netronBridges[document];
            base.LoadSystem(bridge.System);
            _presentation.AttachToDocument(document);
            _bridge = bridge;

            if(ModelChanged != null)
                ModelChanged(this, EventArgs.Empty);
        }

        //public IMathSystem CurrentSystem
        //{
        //    get { return base.c _bridge.System; }
        //}

        public Document CurrentDocument
        {
            get { return _bridge.Document; }
        }
        #endregion

        #region Netron Subscriptions
        void NetronEntityRemovedHandler(object sender, EntityEventArgs e)
        {
            IDiagramEntity entity = e.Entity;
            SignalShape ss = entity as SignalShape;
            if(ss != null)
            {
                if(!_bridge.Signals.ContainsValue(ss))
                    return;
                _bridge.Signals.Remove(ss.SignalReference.InstanceId);
                PostCommandRemoveSignal(ss.SignalReference, true);
                return;
            }
            BusShape bs = entity as BusShape;
            if(bs != null)
            {
                if(!_bridge.Buses.ContainsValue(bs))
                    return;
                _bridge.Buses.Remove(bs.BusReference.InstanceId);
                PostCommandRemoveBus(bs.BusReference);
                return;
            }
            PortShape ps = entity as PortShape;
            if(ps != null)
            {
                if(!_bridge.Ports.ContainsValue(ps))
                    return;
                _bridge.Ports.Remove(ps.PortReference.InstanceId);
                PostCommandRemovePort(ps.PortReference, true);
                return;
            }

        }
        void NetronEntityAddedHandler(object sender, EntityEventArgs e)
        {
        }

        void Model_OnConnectorDetached(object sender, ConnectorsEventArgs e)
        {
            if(maskNDrives)
                return;

            bool wasMasked = maskMDrives;
            try
            {
                maskMDrives = true;

                IConnection con = _bridge.Model.ConnectorHolders[e.SubjectConnector] as IConnection;
                IConnector cr1 = e.ObjectConnector;

                // get rid of uninteresting situations
                if(con == null)
                {
                    con = _bridge.Model.ConnectorHolders[e.ObjectConnector] as IConnection;
                    cr1 = e.SubjectConnector;
                    if(con == null)
                        return;
                }

                if(con.From.AttachedTo == null && con.To.AttachedTo == null)
                    return;

                IConnector cr2 = con.From.AttachedTo == null ? con.To.AttachedTo : con.From.AttachedTo;

                // find out who connects to whom
                IConnector portConnector = cr1;
                IShape portShape = (IShape)_bridge.Model.ConnectorHolders[portConnector];
                IShape valueShape = (IShape)_bridge.Model.ConnectorHolders[cr2];

                PortShape port = portShape as PortShape;
                if(port == null)
                {
                    port = valueShape as PortShape;
                    if(port == null)
                        return;
                    valueShape = portShape; //ensure valueShape should now be either signal or bus
                    portConnector = cr2;
                }
                SignalShape signal = valueShape as SignalShape;
                if(signal != null)
                {
                    // PORT <-> SIGNAL

                    int index = -1;
                    List<Connector> sl = port.InputConnectors;
                    for(int i = 0; i < sl.Count; i++)
                    {
                        if(sl[i] == portConnector)
                        {
                            index = i;
                            break;
                        }
                    }

                    if(index > -1)
                    {
                        PostCommandSignalDrivesPortNoLonger(port.PortReference, index);
                        return;
                    }

                    index = -1;
                    sl = port.OutputConnectors;
                    for(int i = 0; i < sl.Count; i++)
                    {
                        if(sl[i] == portConnector)
                        {
                            index = i;
                            break;
                        }
                    }

                    if(index > -1)
                        PostCommandPortDrivesSignalNoLonger(port.PortReference, index);

                    return;
                }
                BusShape bus = valueShape as BusShape;
                if(bus != null)
                {
                    // PORT <-> BUS

                    int index = -1;
                    List<Connector> bl = port.BusConnectors;
                    for(int i = 0; i < bl.Count; i++)
                    {
                        if(bl[i] == portConnector)
                        {
                            index = i;
                            break;
                        }
                    }

                    if(index > -1)
                        PostCommandBusDetachedFromPort(port.PortReference, index);

                    return;
                }
            }
            finally
            {
                maskMDrives = wasMasked;
            }
        }

        void Model_OnConnectorAttached(object sender, ConnectorsEventArgs e)
        {
            if(maskNDrives)
                return;

            bool wasMasked = maskMDrives;
            try
            {
                maskMDrives = true;

                IConnection con = _bridge.Model.ConnectorHolders[e.SubjectConnector] as IConnection;

                // get rid of uninteresting situations
                if(con != null)
                {
                    if(con.From.AttachedTo == null || con.To.AttachedTo == null)
                        return;
                }
                else
                {
                    con = _bridge.Model.ConnectorHolders[e.ObjectConnector] as IConnection;
                    if(con != null)
                    {
                        if(con.From.AttachedTo == null || con.To.AttachedTo == null)
                            return;
                    }
                    else
                        return;
                }

                // find out who connects to whom
                IConnector portConnector = con.From.AttachedTo;
                IShape portShape = (IShape)_bridge.Model.ConnectorHolders[portConnector];
                IShape valueShape = (IShape)_bridge.Model.ConnectorHolders[con.To.AttachedTo];

                PortShape port = portShape as PortShape;
                if(port == null)
                {
                    port = valueShape as PortShape;
                    if(port == null)
                        return;
                    valueShape = portShape; //ensure valueShape should now be either signal or bus
                    portConnector = con.To.AttachedTo;
                }
                SignalShape signal = valueShape as SignalShape;
                if(signal != null)
                {
                    // PORT <-> SIGNAL

                    int index = -1;
                    List<Connector> sl = port.InputConnectors;
                    for(int i = 0; i < sl.Count; i++)
                    {
                        if(sl[i] == portConnector)
                        {
                            index = i;
                            break;
                        }
                    }

                    if(index > -1)
                    {
                        PostCommandSignalDrivesPort(signal.SignalReference, port.PortReference, index);
                        return;
                    }

                    index = -1;
                    sl = port.OutputConnectors;
                    for(int i = 0; i < sl.Count; i++)
                    {
                        if(sl[i] == portConnector)
                        {
                            index = i;
                            break;
                        }
                    }

                    if(index > -1)
                        PostCommandPortDrivesSignal(signal.SignalReference, port.PortReference, index);

                    return;
                }
                BusShape bus = valueShape as BusShape;
                if(bus != null)
                {
                    // PORT <-> BUS

                    int index = -1;
                    List<Connector> bl = port.BusConnectors;
                    for(int i = 0; i < bl.Count; i++)
                    {
                        if(bl[i] == portConnector)
                        {
                            index = i;
                            break;
                        }
                    }

                    if(index > -1)
                        PostCommandBusAttachedToPort(bus.BusReference, port.PortReference, index);

                    return;
                }
            }
            finally
            {
                maskMDrives = wasMasked;
            }
        }
        #endregion

        #region Yttrium Subscriptions
        public override void OnSignalAdded(Signal signal, int index)
        {
            if(_bridge.Signals.ContainsKey(signal.InstanceId))
                return;
            SignalShape shape = new SignalShape(_bridge.Model, new CommandReference(signal.InstanceId, index), GetFlyweightForSignal(signal.Value));
            shape.Signal = signal;
            shape.Location = CreateRandomLocation();
            _bridge.Signals.Add(signal.InstanceId, shape);
            _bridge.Model.AddShape(shape);
            _presentation.Invalidate();
        }
        public override void OnBusAdded(Bus bus, int index)
        {
            if(_bridge.Buses.ContainsKey(bus.InstanceId))
                return;
            BusShape shape = new BusShape(_bridge.Model, new CommandReference(bus.InstanceId, index), GetFlyweightForBus(bus.Value));
            shape.Bus = bus;
            shape.Location = CreateRandomLocation();
            _bridge.Buses.Add(bus.InstanceId, shape);
            _bridge.Model.AddShape(shape);
            _presentation.Invalidate();
        }
        public override void OnPortAdded(Port port, int index)
        {
            if(_bridge.Ports.ContainsKey(port.InstanceId))
                return;
            PortShape shape = new PortShape(_bridge.Model, new CommandReference(port.InstanceId, index), GetFlyweightForPort(port.Entity));
            shape.Port = port;
            shape.Location = CreateRandomLocation();
            _bridge.Ports.Add(port.InstanceId, shape);
            _bridge.Model.AddShape(shape);
            _presentation.Invalidate();
        }

        public override void OnSignalRemoved(Signal signal, int index)
        {
            if(!_bridge.Signals.ContainsKey(signal.InstanceId))
                return;
            SignalShape shape = _bridge.Signals[signal.InstanceId];
            _bridge.Signals.Remove(signal.InstanceId);
            _bridge.Model.RemoveShape(shape);
            _presentation.Invalidate();
        }
        public override void OnBusRemoved(Bus bus, int index)
        {
            if(!_bridge.Buses.ContainsKey(bus.InstanceId))
                return;
            BusShape shape = _bridge.Buses[bus.InstanceId];
            _bridge.Buses.Remove(bus.InstanceId);
            _bridge.Model.RemoveShape(shape);
            _presentation.Invalidate();
        }
        public override void OnPortRemoved(Port port, int index)
        {
            if(!_bridge.Ports.ContainsKey(port.InstanceId))
                return;
            PortShape shape = _bridge.Ports[port.InstanceId];
            _bridge.Ports.Remove(port.InstanceId);
            _bridge.Model.RemoveShape(shape);
            _presentation.Invalidate();
        }

        public override void OnSignalMoved(Signal signal, int indexBefore, int indexAfter)
        {
            _bridge.Signals[signal.InstanceId].SignalReference = new CommandReference(signal.InstanceId, indexAfter);
        }
        public override void OnBusMoved(Bus bus, int indexBefore, int indexAfter)
        {
            _bridge.Buses[bus.InstanceId].BusReference = new CommandReference(bus.InstanceId, indexAfter);
        }
        public override void OnPortMoved(Port port, int indexBefore, int indexAfter)
        {
            _bridge.Ports[port.InstanceId].PortReference = new CommandReference(port.InstanceId, indexAfter);
        }

        public override void OnSignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
            if(maskMDrives)
                return;

            bool wasMasked = maskNDrives;
            try
            {
                maskNDrives = true;
                SignalShape ss = _bridge.Signals[signal.InstanceId];
                PortShape ps = _bridge.Ports[port.InstanceId];
                IConnector sc = ss.OutputConnector;
                IConnector pc = ps.InputConnectors[inputIndex];

                Connection cn = new Connection(Point.Empty, Point.Empty);
                _bridge.Model.AddConnection(cn);
                sc.AttachConnector(cn.From);
                pc.AttachConnector(cn.To);
            }
            finally
            {
                maskNDrives = wasMasked;
            }
        }

        public override void OnSignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex)
        {
            if(maskMDrives)
                return;

            bool wasMasked = maskNDrives;
            try
            {
                maskNDrives = true;
                SignalShape ss = _bridge.Signals[signal.InstanceId];
                PortShape ps = _bridge.Ports[port.InstanceId];
                IConnector sc = ss.OutputConnector;
                IConnector pc = ps.InputConnectors[inputIndex];

                IConnection connection = _bridge.Model.ConnectorHolders[pc.AttachedTo] as IConnection;
                sc.DetachConnector(connection.From);
                sc.DetachConnector(connection.To);
                pc.DetachConnector(connection.From);
                pc.DetachConnector(connection.To);
                _bridge.Model.Remove(connection);
            }
            finally
            {
                maskNDrives = wasMasked;
            }
        }

        public override void OnPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
            if(maskMDrives)
                return;

            bool wasMasked = maskNDrives;
            try
            {
                maskNDrives = true;
                SignalShape ss = _bridge.Signals[signal.InstanceId];
                PortShape ps = _bridge.Ports[port.InstanceId];
                IConnector sc = ss.InputConnector;
                IConnector pc = ps.OutputConnectors[outputIndex];

                Connection cn = new Connection(Point.Empty, Point.Empty);
                _bridge.Model.AddConnection(cn);
                pc.AttachConnector(cn.From);
                sc.AttachConnector(cn.To);

                ss.Location = new Point(pc.Point.X - 16, pc.Point.Y - 10);
                _bridge.Model.SendToFront(ss);
            }
            finally
            {
                maskNDrives = wasMasked;
            }
        }

        public override void OnPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex)
        {
            if(maskMDrives)
                return;

            bool wasMasked = maskNDrives;
            try
            {
                maskNDrives = true;
                SignalShape ss = _bridge.Signals[signal.InstanceId];
                PortShape ps = _bridge.Ports[port.InstanceId];
                IConnector sc = ss.InputConnector;
                IConnector pc = ps.OutputConnectors[outputIndex];

                IConnection connection = _bridge.Model.ConnectorHolders[pc.AttachedTo] as IConnection;
                sc.DetachConnector(connection.From);
                sc.DetachConnector(connection.To);
                pc.DetachConnector(connection.From);
                pc.DetachConnector(connection.To);
                _bridge.Model.Remove(connection);
            }
            finally
            {
                maskNDrives = wasMasked;
            }
        }

        public override void OnSignalValueChanged(Signal signal)
        {
            SignalShape ss = _bridge.Signals[signal.InstanceId];
            ss.AssignFly(GetFlyweightForSignal(signal.Value));
        }

        public override void OnBusValueChanged(Bus bus)
        {
            BusShape bs = _bridge.Buses[bus.InstanceId];
            bs.AssignFly(GetFlyweightForBus(bus.Value));
        }

        // ... TODO

        #endregion

        private System.Drawing.Point CreateRandomLocation()
        {
            return new System.Drawing.Point(
                30 + (int)((_presentation.Size.Width-60) * Config.Random.NextDouble()),
                30 + (int)((_presentation.Size.Height - 60) * Config.Random.NextDouble()));
        }
    }
}
