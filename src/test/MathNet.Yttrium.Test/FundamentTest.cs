#region Math.NET Yttrium (GPL) by Christoph Ruegg
// Math.NET Yttrium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MathNet.Symbolics;
using MathNet.Symbolics.Events;
using MathNet.Symbolics.AutoEvaluation;

namespace Yttrium.UnitTests
{
    [TestFixture]
    public class FundamentTest
    {
        public class NodeObject
            : Node
        {
            protected override void OnAutoEvaluateFlag(NodeFlag flag)
            {
            }

            protected override void OnAutoEvaluateProperty(NodeProperty property)
            {
            }
        }

        private int _counter = 0;
        private NodePropertyChangedEventArgs _lastPropertyChangedEventArgs;
        private NodeFlagChangedEventArgs _lastFlagChangedEventArgs;
        private void OnPropertyChanged(object sender, NodePropertyChangedEventArgs e)
        {
            _lastPropertyChangedEventArgs = e;
            _counter++;
        }
        private void OnFlagChanged(object sender, NodeFlagChangedEventArgs e)
        {
            _lastFlagChangedEventArgs = e;
            _counter--;
        }

        [Test]
        public void PropertyAspectTest()
        {
            MathIdentifier propertyId = new MathIdentifier("T0_PA", "FundamentTest");
            NodeProperty property = NodeProperty.Register(propertyId, typeof(string), typeof(FundamentTest));

            NodeObject n = new NodeObject();
            _counter = 0;
            n.AddHandler(property.PropertyChangedEvent, new EventHandler<NodePropertyChangedEventArgs>(OnPropertyChanged));

            Assert.AreEqual(0, _counter, "X1");
            Assert.IsFalse(n.IsPropertySet(property, false), "A01");
            Assert.IsFalse(n.IsPropertySet(property, true), "A02");
            Assert.IsFalse(n.IsPropertyDirty(property), "A03");
            Assert.AreEqual("nothing", n.GetProperty(property, "nothing", false), "A04");
            Assert.AreEqual("nothing", n.GetProperty(property, "nothing", true), "A05");

            n.SetProperty(property, "myvalue");
            Assert.AreEqual(1, _counter, "X2");
            Assert.AreEqual(null, _lastPropertyChangedEventArgs.OldValue, "X2a");
            Assert.AreEqual("myvalue", _lastPropertyChangedEventArgs.NewValue, "X2b");
            Assert.IsTrue(n.IsPropertySet(property, false), "B01");
            Assert.IsTrue(n.IsPropertySet(property, true), "B02");
            Assert.IsFalse(n.IsPropertyDirty(property), "B03");
            Assert.AreEqual("myvalue", n.GetProperty(property, "nothing", false), "B04");
            Assert.AreEqual("myvalue", n.GetProperty(property, "nothing", true), "B05");

            n.DirtyPropertyIfSet(property);
            Assert.AreEqual(1, _counter, "X3");
            Assert.IsFalse(n.IsPropertySet(property, false), "C01");
            Assert.IsTrue(n.IsPropertySet(property, true), "C02");
            Assert.IsTrue(n.IsPropertyDirty(property), "C03");
            Assert.AreEqual("nothing", n.GetProperty(property, "nothing", false), "C04");
            Assert.AreEqual("myvalue", n.GetProperty(property, "nothing", true), "C05");

            n.SetProperty(property, "newvalue");
            Assert.AreEqual(2, _counter, "X4");
            Assert.AreEqual("myvalue", _lastPropertyChangedEventArgs.OldValue, "X4a");
            Assert.AreEqual("newvalue", _lastPropertyChangedEventArgs.NewValue, "X4b");
            Assert.IsTrue(n.IsPropertySet(property, false), "D01");
            Assert.IsTrue(n.IsPropertySet(property, true), "D02");
            Assert.IsFalse(n.IsPropertyDirty(property), "D03");
            Assert.AreEqual("newvalue", n.GetProperty(property, "nothing", false), "D04");
            Assert.AreEqual("newvalue", n.GetProperty(property, "nothing", true), "D05");

            n.ClearProperty(property);
            Assert.AreEqual(3, _counter, "X5");
            Assert.AreEqual("newvalue", _lastPropertyChangedEventArgs.OldValue, "X5a");
            Assert.AreEqual(null, _lastPropertyChangedEventArgs.NewValue, "X5b");
            Assert.IsFalse(n.IsPropertySet(property, false), "E01");
            Assert.IsFalse(n.IsPropertySet(property, true), "E02");
            Assert.IsFalse(n.IsPropertyDirty(property), "E03");
            Assert.AreEqual("nothing", n.GetProperty(property, "nothing", false), "E04");
            Assert.AreEqual("nothing", n.GetProperty(property, "nothing", true), "E05");
        }

        [Test]
        public void FlagAspectTest()
        {
            MathIdentifier flagId = new MathIdentifier("T1_FA", "FundamentTest");
            NodeFlag flag = NodeFlag.Register(flagId, typeof(FundamentTest));

            NodeObject n = new NodeObject();
            _counter = 0;
            n.AddHandler(flag.FlagChangedEvent, new EventHandler<NodeFlagChangedEventArgs>(OnFlagChanged));

            Assert.AreEqual(0, _counter, "X1");
            Assert.IsFalse(n.IsFlagSet(flag, false), "A01");
            Assert.IsFalse(n.IsFlagSet(flag, true), "A02");
            Assert.IsFalse(n.IsFlagDirty(flag), "A03");
            Assert.IsFalse(n.IsFlagEnabled(flag, false), "A04");
            Assert.IsFalse(n.IsFlagEnabled(flag, true), "A05");
            Assert.IsFalse(n.IsFlagDisabled(flag, false), "A06");
            Assert.IsFalse(n.IsFlagDisabled(flag, true), "A07");
            Assert.IsTrue(n.IsFlagUnknown(flag), "A08");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flag), "A09");

            n.EnableFlag(flag);
            Assert.AreEqual(-1, _counter, "X2");
            Assert.AreEqual(FlagState.Unknown, _lastFlagChangedEventArgs.OldState, "X2a");
            Assert.AreEqual(FlagState.Enabled, _lastFlagChangedEventArgs.NewState, "X2b");
            Assert.IsTrue(n.IsFlagSet(flag, false), "B01");
            Assert.IsTrue(n.IsFlagSet(flag, true), "B02");
            Assert.IsFalse(n.IsFlagDirty(flag), "B03");
            Assert.IsTrue(n.IsFlagEnabled(flag, false), "B04");
            Assert.IsTrue(n.IsFlagEnabled(flag, true), "B05");
            Assert.IsFalse(n.IsFlagDisabled(flag, false), "B06");
            Assert.IsFalse(n.IsFlagDisabled(flag, true), "B07");
            Assert.IsFalse(n.IsFlagUnknown(flag), "B08");
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flag), "B09");

            n.DisableFlag(flag);
            Assert.AreEqual(-2, _counter, "X3");
            Assert.AreEqual(FlagState.Enabled, _lastFlagChangedEventArgs.OldState, "X3a");
            Assert.AreEqual(FlagState.Disabled, _lastFlagChangedEventArgs.NewState, "X3b");
            Assert.IsTrue(n.IsFlagSet(flag, false), "C01");
            Assert.IsTrue(n.IsFlagSet(flag, true), "C02");
            Assert.IsFalse(n.IsFlagDirty(flag), "C03");
            Assert.IsFalse(n.IsFlagEnabled(flag, false), "C04");
            Assert.IsFalse(n.IsFlagEnabled(flag, true), "C05");
            Assert.IsTrue(n.IsFlagDisabled(flag, false), "C06");
            Assert.IsTrue(n.IsFlagDisabled(flag, true), "C07");
            Assert.IsFalse(n.IsFlagUnknown(flag), "C08");
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flag), "C09");

            n.DirtyFlagIfSet(flag);
            Assert.AreEqual(-2, _counter, "X4");
            Assert.IsFalse(n.IsFlagSet(flag, false), "D01");
            Assert.IsTrue(n.IsFlagSet(flag, true), "D02");
            Assert.IsTrue(n.IsFlagDirty(flag), "D03");
            Assert.IsFalse(n.IsFlagEnabled(flag, false), "D04");
            Assert.IsFalse(n.IsFlagEnabled(flag, true), "D05");
            Assert.IsFalse(n.IsFlagDisabled(flag, false), "D06");
            Assert.IsTrue(n.IsFlagDisabled(flag, true), "D07");
            Assert.IsFalse(n.IsFlagUnknown(flag), "D08");
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flag), "D09");

            n.EnableFlag(flag);
            Assert.AreEqual(-3, _counter, "X5");
            Assert.AreEqual(FlagState.Disabled, _lastFlagChangedEventArgs.OldState, "X5a");
            Assert.AreEqual(FlagState.Enabled, _lastFlagChangedEventArgs.NewState, "X5b");
            Assert.IsTrue(n.IsFlagSet(flag, false), "E01");
            Assert.IsTrue(n.IsFlagSet(flag, true), "E02");
            Assert.IsFalse(n.IsFlagDirty(flag), "E03");
            Assert.IsTrue(n.IsFlagEnabled(flag, false), "E04");
            Assert.IsTrue(n.IsFlagEnabled(flag, true), "E05");
            Assert.IsFalse(n.IsFlagDisabled(flag, false), "E06");
            Assert.IsFalse(n.IsFlagDisabled(flag, true), "E07");
            Assert.IsFalse(n.IsFlagUnknown(flag), "E08");
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flag), "E09");

            n.ClearFlag(flag);
            Assert.AreEqual(-4, _counter, "X6");
            Assert.AreEqual(FlagState.Enabled, _lastFlagChangedEventArgs.OldState, "X6a");
            Assert.AreEqual(FlagState.Unknown, _lastFlagChangedEventArgs.NewState, "X6b");
            Assert.IsFalse(n.IsFlagSet(flag, false), "F01");
            Assert.IsFalse(n.IsFlagSet(flag, true), "F02");
            Assert.IsFalse(n.IsFlagDirty(flag), "F03");
            Assert.IsFalse(n.IsFlagEnabled(flag, false), "F04");
            Assert.IsFalse(n.IsFlagEnabled(flag, true), "F05");
            Assert.IsFalse(n.IsFlagDisabled(flag, false), "F06");
            Assert.IsFalse(n.IsFlagDisabled(flag, true), "F07");
            Assert.IsTrue(n.IsFlagUnknown(flag), "F08");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flag), "F09");
        }

        [Test]
        public void PropertyTriggerTest()
        {
            MathIdentifier propertyId = new MathIdentifier("T2_PT", "FundamentTest");

            NodeEvent clearEvent = NodeEvent.Register(propertyId.DerivePostfix("ClearTrigger"), typeof(EventHandler), typeof(FundamentTest));
            NodeEvent clear2Event = NodeEvent.Register(propertyId.DerivePostfix("Clear2Trigger"), typeof(EventHandler), typeof(FundamentTest));
            NodeEvent clear3Event = NodeEvent.Register(propertyId.DerivePostfix("Clear3Trigger"), typeof(EventHandler), typeof(FundamentTest));
            NodeEvent reevaluateEvent = NodeEvent.Register(propertyId.DerivePostfix("ReevaluateTrigger"), typeof(EventHandler), typeof(FundamentTest));

            NodeProperty property = NodeProperty.Register(propertyId, typeof(string), typeof(FundamentTest),
                new NodeEventTrigger(EventTriggerAction.Clear, clearEvent, clear2Event),
                new NodeEventTrigger(EventTriggerAction.Clear, clear3Event),
                new NodeEventTrigger(EventTriggerAction.Reevaluate, reevaluateEvent));

            NodeObject n = new NodeObject();
            Assert.IsFalse(n.IsPropertySet(property), "A01");

            n.SetProperty(property, "test");
            Assert.IsTrue(n.IsPropertySet(property), "B01");

            n.RaiseEvent(clearEvent, EventArgs.Empty);
            Assert.IsFalse(n.IsPropertySet(property), "C01");

            n.SetProperty(property, "test2");
            Assert.IsTrue(n.IsPropertySet(property), "D01");
            Assert.AreEqual("test2", n.GetProperty(property), "D02");

            n.RaiseEvent(reevaluateEvent, EventArgs.Empty);
            Assert.IsTrue(n.IsPropertySet(property), "E01");
            Assert.AreEqual("test2", n.GetProperty(property), "E02");

            n.RaiseEvent(clear2Event, EventArgs.Empty);
            Assert.IsFalse(n.IsPropertySet(property), "F01");

            n.SetProperty(property, "test3");
            Assert.IsTrue(n.IsPropertySet(property), "G01");

            n.RaiseEvent(clear3Event, EventArgs.Empty);
            Assert.IsFalse(n.IsPropertySet(property), "H01");
        }

        [Test]
        public void FlagTriggerTest()
        {
            MathIdentifier flagId = new MathIdentifier("T3_FT", "FundamentTest");
            
            NodeEvent clearEvent = NodeEvent.Register(flagId.DerivePostfix("ClearTrigger"), typeof(EventHandler), typeof(FundamentTest));
            NodeEvent enableEvent = NodeEvent.Register(flagId.DerivePostfix("EnableTrigger"), typeof(EventHandler), typeof(FundamentTest));
            NodeEvent disableEvent = NodeEvent.Register(flagId.DerivePostfix("DisableTrigger"), typeof(EventHandler), typeof(FundamentTest));
            NodeEvent disable2Event = NodeEvent.Register(flagId.DerivePostfix("Disable2Trigger"), typeof(EventHandler), typeof(FundamentTest));
            NodeEvent reevaluateEvent = NodeEvent.Register(flagId.DerivePostfix("ReevaluateTrigger"), typeof(EventHandler), typeof(FundamentTest));

            NodeFlag flag = NodeFlag.Register(flagId, typeof(FundamentTest), FlagKind.Default,
                new NodeEventTrigger(EventTriggerAction.Clear, clearEvent),
                new NodeEventTrigger(EventTriggerAction.Enable, enableEvent),
                new NodeEventTrigger(EventTriggerAction.Disable, disableEvent, disable2Event),
                new NodeEventTrigger(EventTriggerAction.Reevaluate, reevaluateEvent));

            NodeObject n = new NodeObject();
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flag), "A01");

            n.EnableFlag(flag);
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flag), "B01");

            n.RaiseEvent(disableEvent, EventArgs.Empty);
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flag), "C01");

            n.RaiseEvent(enableEvent, EventArgs.Empty);
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flag), "D01");

            n.RaiseEvent(reevaluateEvent, EventArgs.Empty);
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flag), "E01");

            n.RaiseEvent(disable2Event, EventArgs.Empty);
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flag), "F01");

            n.RaiseEvent(clearEvent, EventArgs.Empty);
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flag), "G01");
        }

        [Test]
        public void RemoteEventTriggersTest()
        {
            MathIdentifier flagId = new MathIdentifier("TX_F", "FundamentTest");

            NodeFlag flag = NodeFlag.Register(flagId, typeof(FundamentTest));

            NodeFlag flagEnableRemote = NodeFlag.Register(flagId.DerivePostfix("EnableRemote"), typeof(FundamentTest), FlagKind.Default,
                new NodeEventTrigger(EventTriggerAction.Enable, flag, flag.FlagEnabledEvent));
            NodeFlag flagEnableLocal = NodeFlag.Register(flagId.DerivePostfix("EnableLocal"), typeof(FundamentTest), FlagKind.Default,
                new NodeEventTrigger(EventTriggerAction.Enable, flag.FlagEnabledEvent));

            NodeFlag flagDisableRemote = NodeFlag.Register(flagId.DerivePostfix("DisableRemote"), typeof(FundamentTest), FlagKind.Default,
                new NodeEventTrigger(EventTriggerAction.Disable, flag, flag.FlagChangedEvent));
            NodeFlag flagDisableLocal = NodeFlag.Register(flagId.DerivePostfix("DisableLocal"), typeof(FundamentTest), FlagKind.Default,
                new NodeEventTrigger(EventTriggerAction.Disable, flag.FlagChangedEvent));

            NodeObject n = new NodeObject();
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flag), "A01");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagEnableRemote), "A02");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagEnableLocal), "A03");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagDisableRemote), "A04");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagDisableLocal), "A05");

            n.EnableFlag(flag);
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flag), "B01");
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flagEnableRemote), "B02");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagEnableLocal), "B03");
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flagDisableRemote), "B04");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagDisableLocal), "B05");

            n.ClearFlag(flag);
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flag), "C01");
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flagEnableRemote), "C02");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagEnableLocal), "C03");
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flagDisableRemote), "C04");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagDisableLocal), "C05");

            n.EnableFlag(flagDisableLocal);
            n.DisableFlag(flagEnableLocal);
            n.ClearFlag(flagDisableRemote);
            n.ClearFlag(flagEnableRemote);
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flag), "D01");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagEnableRemote), "D02");
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flagEnableLocal), "D03");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagDisableRemote), "D04");
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flagDisableLocal), "D05");

            n.DisableFlag(flag);
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flag), "E01");
            Assert.AreEqual(FlagState.Unknown, n.GetFlagState(flagEnableRemote), "E02");
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flagEnableLocal), "E03");
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flagDisableRemote), "E04");
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flagDisableLocal), "E05");

            n.EnableFlag(flag);
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flag), "F01");
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flagEnableRemote), "F02");
            Assert.AreEqual(FlagState.Enabled, n.GetFlagState(flagEnableLocal), "F03");
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flagDisableRemote), "F04");
            Assert.AreEqual(FlagState.Disabled, n.GetFlagState(flagDisableLocal), "F05");
        }
    }
}
