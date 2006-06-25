//using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework;
using NUnit.Framework;

using System;
using System.Collections.Generic;

using MathNet.Numerics;
using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Patterns;
using MathNet.Symbolics.StdPackage;
using MathNet.Symbolics.StdPackage.Structures;
using MathNet.Symbolics.StdPackage.Properties;

namespace Yttrium.UnitTests
{
    [TestFixture]
    public class PatternTest
    {
        [Test]
        public void Pattern_Conditions()
        {
            Project p = new Project();
            MathSystem s = p.CurrentSystem;
            Builder b = p.Builder;
            Context c = p.Context;

            // sin(x^2)
            Signal x = new Signal(c); x.Label = "x";
            x.AddConstraint(RealSetProperty.Instance);
            Signal x2 = b.Power(x, IntegerValue.ConstantTwo(c)); x2.Label = "x2";
            Signal sinx2 = Std.Sine(c, x2); sinx2.Label = "sinx2";

            AlwaysTrueCondition ctrue = AlwaysTrueCondition.Instance;
            Assert.AreEqual(true, ctrue.FulfillsCondition(x, x.DrivenByPort), "A01");

            EntityCondition centity = new EntityCondition(new MathIdentifier("Sine", "Std"));
            Assert.AreEqual(true, centity.FulfillsCondition(sinx2, sinx2.DrivenByPort), "A02");
            Assert.AreEqual(false, centity.FulfillsCondition(x2, x2.DrivenByPort), "A03");

            InputSignalsPropertyCondition cinputconst = new InputSignalsPropertyCondition(new MathIdentifier("Constant", "Std"), CombinationMode.AtLeastOne);
            Assert.AreEqual(true, cinputconst.FulfillsCondition(x2, x2.DrivenByPort), "A04");
            Assert.AreEqual(false, cinputconst.FulfillsCondition(sinx2, sinx2.DrivenByPort), "A05");

            OrCondition cor = new OrCondition(centity, cinputconst);
            Assert.AreEqual(true, cor.FulfillsCondition(x2, x2.DrivenByPort), "A06");
            Assert.AreEqual(true, cor.FulfillsCondition(sinx2, sinx2.DrivenByPort), "A07");
            Assert.AreEqual(false, cor.FulfillsCondition(x, x.DrivenByPort), "A08");

            AndCondition cand = new AndCondition(ctrue, centity);
            Assert.AreEqual(true, cand.FulfillsCondition(sinx2, sinx2.DrivenByPort), "A09");
            Assert.AreEqual(false, cand.FulfillsCondition(x2, x2.DrivenByPort), "A10");
        }

        [Test]
        public void Pattern_TreePattern()
        {
            Project p = new Project();
            MathSystem s = p.CurrentSystem;
            Builder b = p.Builder;
            Context c = p.Context;

            // sin(x^2)
            Signal x = new Signal(c); x.Label = "x";
            x.AddConstraint(RealSetProperty.Instance);
            Signal x2 = b.Square(x); x2.Label = "x2";
            Signal sinx2 = Std.Sine(c, x2); sinx2.Label = "sinx2";

            Pattern psimp = new Pattern(new EntityCondition(new MathIdentifier("Sine", "Std")));
            Assert.AreEqual(true, psimp.Match(sinx2, sinx2.DrivenByPort), "B01");
            Assert.AreEqual(false, psimp.Match(x2, x2.DrivenByPort), "B02");

            TreePattern psinsqr = new TreePattern(new EntityCondition(new MathIdentifier("Sine", "Std")));
            psinsqr.Add(new Pattern(new EntityCondition(new MathIdentifier("Square", "Std"))));
            Assert.AreEqual(true, psinsqr.Match(sinx2, sinx2.DrivenByPort), "B03");
            Assert.AreEqual(false, psinsqr.Match(x2, x2.DrivenByPort), "B04");

            TreePattern psinadd = new TreePattern(new EntityCondition(new MathIdentifier("Sine", "Std")));
            psinadd.Add(new Pattern(new EntityCondition(new MathIdentifier("Add", "Std"))));
            Assert.AreEqual(false, psinadd.Match(sinx2, sinx2.DrivenByPort), "B03");
            Assert.AreEqual(false, psinadd.Match(x2, x2.DrivenByPort), "B04");
        }

        [Test]
        public void Pattern_CoalescedTreeMatching()
        {
            Project p = new Project();
            MathSystem s = p.CurrentSystem;
            Builder b = p.Builder;
            Context c = p.Context;

            // sin(x^2)
            Signal x = new Signal(c); x.Label = "x";
            x.AddConstraint(RealSetProperty.Instance);
            Signal x2 = b.Square(x); x2.Label = "x2";
            Signal sinx2 = Std.Sine(c, x2); sinx2.Label = "sinx2";

            CoalescedTreeNode root = new CoalescedTreeNode(AlwaysTrueCondition.Instance);
            root.Subscribe(new MathIdentifier("A", "Test"));

            CoalescedTreeNode sin = new CoalescedTreeNode(new EntityCondition(new MathIdentifier("Sine", "Std")));
            sin.AddGroup(new MathIdentifier("B", "Test"), "sin");
            root.ConditionAxis.Add(sin);

            CoalescedTreeNode sqr = new CoalescedTreeNode(new EntityCondition(new MathIdentifier("Square", "Std")));
            sqr.AddGroup(new MathIdentifier("B", "Test"), "sqr");
            sqr.Subscribe(new MathIdentifier("B", "Test"));
            CoalescedChildPattern sqrPattern = new CoalescedChildPattern();
            sqrPattern.AddChild(sqr);
            sin.PatternAxis.Add(sqrPattern);

            CoalescedTreeNode tan = new CoalescedTreeNode(new EntityCondition(new MathIdentifier("Tangent", "Std")));
            tan.AddGroup(new MathIdentifier("B", "Test"), "tan");
            tan.Subscribe(new MathIdentifier("C", "Test"));
            root.ConditionAxis.Add(tan);

            MatchCollection res = root.MatchAll(sinx2, sinx2.DrivenByPort);
            Assert.AreEqual(true, res.Contains(new MathIdentifier("A", "Test")), "C01");
            Assert.AreEqual(true, res.Contains(new MathIdentifier("B", "Test")), "C02");
            Assert.AreEqual(false, res.Contains(new MathIdentifier("C", "Test")), "C03");

            Match mA = res[new MathIdentifier("A", "Test")];
            Assert.AreEqual(new MathIdentifier("A", "Test"), mA.PatternId, "C04");
            Assert.AreEqual(0, mA.GroupCount, "C05");

            Match mB = res[new MathIdentifier("B", "Test")];
            Assert.AreEqual(new MathIdentifier("B", "Test"), mB.PatternId, "C06");
            Assert.AreEqual(2, mB.GroupCount, "C07");

            Group mBsqr = mB["sqr"];
            Assert.AreEqual(1, mBsqr.Count, "C08");
            Assert.AreEqual(x2.InstanceId, mBsqr[0].First.InstanceId, "C09");
            Assert.AreEqual(x2.DrivenByPort.InstanceId, mBsqr[0].Second.InstanceId, "C10");

            Group mBsin = mB["sin"];
            Assert.AreEqual(1, mBsin.Count, "C11");
            Assert.AreEqual(sinx2.InstanceId, mBsin[0].First.InstanceId, "C12");
            Assert.AreEqual(sinx2.DrivenByPort.InstanceId, mBsin[0].Second.InstanceId, "C13");
        }

        [Test]
        public void Pattern_CoalescedTreeDeduction()
        {
            Project p = new Project();
            MathSystem s = p.CurrentSystem;
            Builder b = p.Builder;
            Context c = p.Context;

            // sin(x^2)
            Signal x = new Signal(c); x.Label = "x";
            x.AddConstraint(RealSetProperty.Instance);
            Signal x2 = b.Square(x); x2.Label = "x2";
            Signal sinx2 = Std.Sine(c, x2); sinx2.Label = "sinx2";

            TreePattern psinadd = new TreePattern(new EntityCondition(new MathIdentifier("Sine", "Std")));
            Pattern psinadd_add = new Pattern(new EntityCondition(new MathIdentifier("Add", "Std")));
            psinadd.Add(psinadd_add);
            psinadd_add.Group = "add";

            TreePattern psinsqr = new TreePattern(new EntityCondition(new MathIdentifier("Sine", "Std")));
            Pattern psinsqr_sqr = new Pattern(new EntityCondition(new MathIdentifier("Square", "Std")));
            psinsqr.Add(psinsqr_sqr);
            psinsqr.Group = "sin";
            psinsqr_sqr.Group = "sqr";

            // generate coalesced tree
            CoalescedTreeNode root;
            List<CoalescedTreeNode> list = CoalescedTreeNode.CreateRootTree(out root);
            psinadd.MergeToCoalescedTree(new MathIdentifier("SinAdd", "Test"), list);
            psinsqr.MergeToCoalescedTree(new MathIdentifier("SinSqr", "Test"), list);

            // test whether the tree was generated correctly
            Assert.AreEqual(1, root.ConditionAxis.Count, "D01");
            Assert.AreEqual(0, root.PatternAxis.Count, "D02");

            CoalescedTreeNode csin = root.ConditionAxis[0];
            Assert.AreEqual(true, csin.Condition is EntityCondition, "D03");
            Assert.AreEqual(new MathIdentifier("Sine", "Std"), ((EntityCondition)csin.Condition).EntityId, "D04");
            Assert.AreEqual(1, csin.GroupAxis.Count, "D05");
            Assert.AreEqual(0, csin.SubscriptionAxis.Count, "D06");
            Assert.AreEqual("sin", csin.GroupAxis[new MathIdentifier("SinSqr", "Test")], "D07");
            Assert.AreEqual(1, csin.PatternAxis.Count, "D08");
            Assert.AreEqual(1, csin.PatternAxis[0].ChildrenAxis.Count, "D09");
            Assert.AreEqual(true, csin.PatternAxis[0].ChildrenAxis[0].Condition is AlwaysTrueCondition, "D10");

            CoalescedTreeNode cadd = csin.PatternAxis[0].ChildrenAxis[0].ConditionAxis[0];
            Assert.AreEqual(true, cadd.Condition is EntityCondition, "D11");
            Assert.AreEqual(new MathIdentifier("Add", "Std"), ((EntityCondition)cadd.Condition).EntityId, "D12");
            Assert.AreEqual(1, cadd.GroupAxis.Count, "D13");
            Assert.AreEqual(1, cadd.SubscriptionAxis.Count, "D14");
            Assert.AreEqual("add", cadd.GroupAxis[new MathIdentifier("SinAdd", "Test")], "D15");
            Assert.AreEqual(new MathIdentifier("SinAdd", "Test"), cadd.SubscriptionAxis[0], "D16");
            Assert.AreEqual(0, cadd.PatternAxis.Count, "D18");

            CoalescedTreeNode csqr = csin.PatternAxis[0].ChildrenAxis[0].ConditionAxis[1];
            Assert.AreEqual(true, csqr.Condition is EntityCondition, "D19");
            Assert.AreEqual(new MathIdentifier("Square", "Std"), ((EntityCondition)csqr.Condition).EntityId, "D20");
            Assert.AreEqual(1, csqr.GroupAxis.Count, "D21");
            Assert.AreEqual(1, csqr.SubscriptionAxis.Count, "D22");
            Assert.AreEqual("sqr", csqr.GroupAxis[new MathIdentifier("SinSqr", "Test")], "D23");
            Assert.AreEqual(new MathIdentifier("SinSqr", "Test"), csqr.SubscriptionAxis[0], "D24");
            Assert.AreEqual(0, csqr.PatternAxis.Count, "D26");

            // test whether the tree works as expected
            MatchCollection res = root.MatchAll(sinx2, sinx2.DrivenByPort);
            Assert.AreEqual(true, res.Contains(new MathIdentifier("SinSqr", "Test")), "D27");
            Assert.AreEqual(false, res.Contains(new MathIdentifier("SinAdd", "Test")), "D28");

            Match match = res[new MathIdentifier("SinSqr", "Test")];
            Assert.AreEqual(new MathIdentifier("SinSqr", "Test"), match.PatternId, "D29");
            Assert.AreEqual(2, match.GroupCount, "D30");
            Assert.AreEqual(1, match["sin"].Count, "D31");
            Assert.AreEqual(sinx2.InstanceId, match["sin"][0].First.InstanceId, "D32");
            Assert.AreEqual(1, match["sqr"].Count, "D33");
            Assert.AreEqual(x2.InstanceId, match["sqr"][0].First.InstanceId, "D34");
        }
    }
}
