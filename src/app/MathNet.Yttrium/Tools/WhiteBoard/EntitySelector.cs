using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MathNet.Symbolics.Whiteboard
{
    public partial class EntitySelector : UserControl
    {
        private IEntity _currentEntity;
        private ICollection<IEntity> _table;

        public event EventHandler CompiledEntitySelected;
        public event EventHandler Canceled;

        public EntitySelector()
        {
            InitializeComponent();
        }

        public ICollection<IEntity> Entities
        {
            get { return _table; }
            set { _table = value; }
        }

        public IEntity Entity
        {
            get { return _currentEntity; }
        }

        public int NumberOfInputs
        {
            get { return (int)udInputs.Value; }
        }

        public int NumberOfBuses
        {
            get { return (int)udBuses.Value; }
        }

        public void UpdateEntities()
        {
            cmbEntities.Items.Clear();
            foreach(IEntity et in _table)
                cmbEntities.Items.Add(et);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(CompiledEntitySelected != null)
                CompiledEntitySelected(this, EventArgs.Empty);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(Canceled != null)
                Canceled(this, EventArgs.Empty);
        }

        private void cmbEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentEntity = cmbEntities.SelectedItem as IEntity;
            if(_currentEntity.IsGeneric)
            {
                udInputs.Enabled = true;
                udBuses.Enabled = true;
            }
            else
            {
                udInputs.Enabled = false;
                udBuses.Enabled = false;
            }
        }

        
    }
}
