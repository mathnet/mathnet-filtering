using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Netron.NetronLight;
using Netron.NetronLight.Win;

using MathNet.Symbolics.Mediator;
using MathNet.Symbolics.Presentation.Shapes;

namespace MathNet.Symbolics.Presentation.WinDiagram
{
    public class NetronController : ControllerBase
    {
        private DiagramControl _presentation;
        private Bridge _bridge;
        private Dictionary<Guid, Bridge> _mathBridges;
        private Dictionary<Document, Bridge> _netronBridges;

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

        public NetronController(DiagramControl netronDiagram)
            : base()
        {
            _presentation = netronDiagram;
            _mathBridges = new Dictionary<Guid, Bridge>();
            _netronBridges = new Dictionary<Document, Bridge>();
            _presentation.OnEntityAdded += NetronEntityAddedHandler;
            _presentation.OnEntityRemoved += NetronEntityRemovedHandler;
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
        }
        protected override void UnloadSystem(IMathSystem system)
        {
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
        }

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
        #endregion

        #region Yttrium Subscriptions
        public override void OnSignalAdded(Signal signal, int index)
        {
            if(_bridge.Signals.ContainsKey(signal.InstanceId))
                return;
            SignalShape shape = new SignalShape(new CommandReference(signal.InstanceId, index));
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
            BusShape shape = new BusShape(new CommandReference(bus.InstanceId, index));
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
            PortShape shape = new PortShape(new CommandReference(port.InstanceId, index));
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
            SignalShape ss = _bridge.Signals[signal.InstanceId];
            PortShape ps = _bridge.Ports[port.InstanceId];
            IConnector sc = ss.OutputConnector;
            IConnector pc = ps.InputConnectors[inputIndex];
            
            Connection cn = new Connection(Point.Empty, Point.Empty);
            sc.AttachConnector(cn.From);
            pc.AttachConnector(cn.To);
            _bridge.Model.AddConnection(cn);
        }

        public override void OnPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
            SignalShape ss = _bridge.Signals[signal.InstanceId];
            PortShape ps = _bridge.Ports[port.InstanceId];
            IConnector sc = ss.InputConnector;
            IConnector pc = ps.OutputConnectors[outputIndex];

            Connection cn = new Connection(Point.Empty, Point.Empty);
            pc.AttachConnector(cn.From);
            sc.AttachConnector(cn.To);
            _bridge.Model.AddConnection(cn);

            ss.Location = new Point(pc.Point.X - 16, pc.Point.Y - 10);
            _bridge.Model.SendToFront(ss);
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
