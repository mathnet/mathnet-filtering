using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MathNet.Symbolics.Mediator;
using MathNet.Symbolics.Packages.Standard;
using MathNet.Symbolics.Packages.Standard.Properties;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Presentation.WinDiagram;

using Netron.NetronLight;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Whiteboard
{
    public partial class WhiteboardForm : Form
    {
        private NetronController _ctrl;

        public WhiteboardForm()
        {
            InitializeComponent();
            _ctrl = new NetronController(netron);

            Service<IPackageLoader>.Instance.LoadStdPackage();

            //// attach a system logger (with console output), something
            //// we get for free thanks to the channels subsystem (mediator/observer).
            //LogSystemObserver lo = new LogSystemObserver(new TextLogWriter(Console.Out));
            //_project.AttachLocalObserver(lo);



            Ambience amb = netron.Document.Model.DefaultPage.Ambience;
            amb.BackgroundType = CanvasBackgroundTypes.Gradient;
            amb.GradientColor1 = Color.WhiteSmoke; //Color.Gold;
            amb.GradientColor2 = Color.SteelBlue; //Color.Goldenrod;
        }

        private void WhiteboardForm_Load(object sender, EventArgs e)
        {
            entitySelector.Entities = Service<ILibrary>.Instance.GetAllEntities();
            entitySelector.UpdateEntities();

            IMathSystem system = Binder.CreateSystem();
            ISystemMediator mediator = Binder.GetInstance<ISystemMediator, IMathSystem>(system);

            // attach a system logger (with console output), something
            // we get for free thanks to the channels subsystem (mediator/observer).
            LogSystemObserver lo = new LogSystemObserver(new TextLogWriter(Console.Out));
            mediator.AttachObserver(lo);

            _ctrl.Load(system);
        }

        private void btnBuildSample_Click(object sender, EventArgs e)
        {
            Signal x = Binder.CreateSignal(); x.Label = "x";
            x.AddConstraint(RealSetProperty.Instance);
            Signal x2 = StdBuilder.Square(x); x2.Label = "x2";
            Signal sinx2 = StdBuilder.Sine(x2); sinx2.Label = "sinx2";
            Signal sinx2t2 = sinx2 * IntegerValue.ConstantTwo;
            _ctrl.CurrentSystem.AddSignalTree(sinx2t2, true, true);
        }

        private void btnNewSignal_Click(object sender, EventArgs e)
        {
            _ctrl.PostCommandNewSignal();
        }

        private void btnNewBus_Click(object sender, EventArgs e)
        {
            _ctrl.PostCommandNewBus();
        }

        private void btnNewPort_Click(object sender, EventArgs e)
        {
            entitySelector.Visible = true;
        }
        
        private void entitySelector_Canceled(object sender, EventArgs e)
        {
            entitySelector.Visible = false;
        }

        private void entitySelector_CompiledEntitySelected(object sender, EventArgs e)
        {
            NewPortCommand cmd = new NewPortCommand();
            cmd.EntityId = entitySelector.Entity.EntityId;
            cmd.NumberOfInputs = entitySelector.NumberOfInputs;
            cmd.NumberOfBuses = entitySelector.NumberOfBuses;
            _ctrl.PostCommand(cmd);
            entitySelector.Visible = false;
        }
    }
}