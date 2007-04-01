using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Backend;
//using MathNet.Symbolics.Backend.Channels;
//using MathNet.Symbolics.Backend.Channels.Commands;
using MathNet.Symbolics.Mediator;

namespace MathNet.Symbolics.Presentation
{
    public abstract class ControllerBase : ISystemObserver
    {
        private ISystemMediator _sysMediator;

        protected ControllerBase()
        {
        }

        public ISystemMediator CurrentMediator
        {
            get { return _sysMediator; }
        }
        public IMathSystem CurrentSystem
        {
            get { return _sysMediator.CurrentSystem; }
        }
        //public CommandChannel CurrentCommands
        //{
        //    get { return _sysMediator.CurrentSystem.Mediator.Commands; }
        //}

        #region Load/Unload System
        protected virtual void LoadSystem(IMathSystem system)
        {
            // reuses mediator if system already has one
            _sysMediator = Binder.GetInstance<ISystemMediator, IMathSystem>(system);
            _sysMediator.AttachObserver(this);
        }
        protected virtual void UnloadSystem(IMathSystem system)
        {
            if(_sysMediator != null)
            {
                _sysMediator.DetachObserver(this);
                _sysMediator = null;
            }
        }
        public virtual bool AutoDetachOnSystemChanged
        {
            get { return false; }
        }
        public virtual bool AutoInitialize
        {
            get { return true; }
        }
        void ISystemObserver.AttachedToSystem(IMathSystem system)
        {
            LoadSystem(system);
        }
        void ISystemObserver.DetachedFromSystem(IMathSystem system)
        {
            UnloadSystem(system);
        }
        void ISystemObserver.BeginInitialize()
        {
            BeginInitializeCore();
        }
        protected virtual void BeginInitializeCore() {}
        void ISystemObserver.EndInitialize()
        {
            EndInitializeCore();
        }
        protected virtual void EndInitializeCore() { }
        #endregion

        #region MathSystem Observer
        public virtual void OnSignalAdded(Signal signal, int index)
        {
        }
        public virtual void OnSignalRemoved(Signal signal, int index)
        {
        }
        public virtual void OnSignalMoved(Signal signal, int indexBefore, int indexAfter)
        {
        }
        public virtual void OnBusAdded(Bus bus, int index)
        {
        }
        public virtual void OnBusRemoved(Bus bus, int index)
        {
        }
        public virtual void OnBusMoved(Bus bus, int indexBefore, int indexAfter)
        {
        }
        public virtual void OnPortAdded(Port port, int index)
        {
        }
        public virtual void OnPortRemoved(Port port, int index)
        {
        }
        public virtual void OnPortMoved(Port port, int indexBefore, int indexAfter)
        {
        }
        public virtual void OnInputAdded(Signal signal, int index)
        {
        }
        public virtual void OnInputRemoved(Signal signal, int index)
        {
        }
        public virtual void OnInputMoved(Signal signal, int indexBefore, int indexAfter)
        {
        }
        public virtual void OnOutputAdded(Signal signal, int index)
        {
        }
        public virtual void OnOutputRemoved(Signal signal, int index)
        {
        }
        public virtual void OnOutputMoved(Signal signal, int indexBefore, int indexAfter)
        {
        }
        public virtual void OnPortDrivesSignal(Signal signal, Port port, int outputIndex)
        {
        }
        public virtual void OnPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex)
        {
        }
        public virtual void OnSignalDrivesPort(Signal signal, Port port, int inputIndex)
        {
        }
        public virtual void OnSignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex)
        {
        }
        public virtual void OnBusAttachedToPort(Bus bus, Port port, int busIndex)
        {
        }
        public virtual void OnBusDetachedFromPort(Bus bus, Port port, int busIndex)
        {
        }
        public virtual void OnSignalValueChanged(Signal signal)
        {
        }
        public virtual void OnBusValueChanged(Bus bus)
        {
        }
        #endregion

        #region Command Helper
        [System.Diagnostics.DebuggerStepThrough]
        public void PostCommand(ICommand command)
        {
            _sysMediator.PostCommand(command);
        }

        public virtual void PostCommandNewSignal()
        {
            PostCommand(new NewSignalCommand());
        }
        public virtual void PostCommandNewBus()
        {
            PostCommand(new NewBusCommand());
        }

        public virtual void PostCommandNewPort(MathIdentifier entityId, int inputCount, int busCount)
        {
            NewPortCommand cmd = new NewPortCommand();
            cmd.EntityId = entityId;
            cmd.NumberOfInputs = inputCount;
            cmd.NumberOfBuses = busCount;
            PostCommand(cmd);
        }
        public virtual void PostCommandRemoveSignal(CommandReference reference, bool isolate)
        {
            RemoveSignalCommand cmd = new RemoveSignalCommand();
            cmd.SignalReference = reference;
            cmd.Isolate = isolate;
            PostCommand(cmd);
        }
        public virtual void PostCommandRemoveBus(CommandReference reference)
        {
            RemoveBusCommand cmd = new RemoveBusCommand();
            cmd.BusReference = reference;
            PostCommand(cmd);
        }
        public virtual void PostCommandRemovePort(CommandReference reference, bool isolate)
        {
            RemovePortCommand cmd = new RemovePortCommand();
            cmd.PortReference = reference;
            cmd.Isolate = isolate;
            PostCommand(cmd);
        }

        public virtual void PostCommandBusAttachedToPort(CommandReference bus, CommandReference port, int index)
        {
            BusDrivePortCommand cmd = new BusDrivePortCommand();
            cmd.BusReference = bus;
            cmd.PortReference = port;
            cmd.Index = index;
            PostCommand(cmd);
        }
        public virtual void PostCommandSignalDrivesPort(CommandReference signal, CommandReference port, int index)
        {
            SignalDrivePortCommand cmd = new SignalDrivePortCommand();
            cmd.SignalReference = signal;
            cmd.PortReference = port;
            cmd.Index = index;
            PostCommand(cmd);
        }
        public virtual void PostCommandPortDrivesSignal(CommandReference signal, CommandReference port, int index)
        {
            PortDriveSignalCommand cmd = new PortDriveSignalCommand();
            cmd.SignalReference = signal;
            cmd.PortReference = port;
            cmd.Index = index;
            PostCommand(cmd);
        }
        public virtual void PostCommandBusDetachedFromPort(CommandReference port, int index)
        {
            BusUndrivePortCommand cmd = new BusUndrivePortCommand();
            cmd.PortReference = port;
            cmd.Index = index;
            PostCommand(cmd);
        }
        public virtual void PostCommandSignalDrivesPortNoLonger(CommandReference port, int index)
        {
            SignalUndrivePortCommand cmd = new SignalUndrivePortCommand();
            cmd.PortReference = port;
            cmd.Index = index;
            PostCommand(cmd);
        }
        public virtual void PostCommandPortDrivesSignalNoLonger(CommandReference port, int index)
        {
            PortUndriveSignalCommand cmd = new PortUndriveSignalCommand();
            cmd.PortReference = port;
            cmd.Index = index;
            PostCommand(cmd);
        }
        #endregion
    }
}
