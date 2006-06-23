//using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework;
using NUnit.Framework;

using System;
using System.Collections.Generic;

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
            psinsqr.Add(new Pattern(new EntityCondition(new MathIdentifier("Square", "Std"))), false);
            Assert.AreEqual(true, psinsqr.Match(sinx2, sinx2.DrivenByPort), "B03");
            Assert.AreEqual(false, psinsqr.Match(x2, x2.DrivenByPort), "B04");

            TreePattern psinadd = new TreePattern(new EntityCondition(new MathIdentifier("Sine", "Std")));
            psinadd.Add(new Pattern(new EntityCondition(new MathIdentifier("Add", "Std"))), false);
            Assert.AreEqual(false, psinadd.Match(sinx2, sinx2.DrivenByPort), "B03");
            Assert.AreEqual(false, psinadd.Match(x2, x2.DrivenByPort), "B04");
        }
    }
}
