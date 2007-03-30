using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing.Design; 

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This editor is used to edit items from e.g. the <see cref="FolderMaterial"/> via the property grid. This is a generic editor so it
    /// can be used to edit any collection of <see cref="IShapeMaterial"/> items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ShapeMaterialCollectionEditor<T> : CollectionEditor where T : IShapeMaterial
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ShapeMaterialCollectionEditor&lt;T&gt;"/> class.
        /// </summary>
        public ShapeMaterialCollectionEditor()
            : base(typeof(List<T>))
        { }

        /// <summary>
        /// Sets the items.
        /// </summary>
        /// <param name="editValue">The edit value.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override object SetItems(object editValue, object[] value)
        {
            CollectionBase<IShapeMaterial> entries = editValue as CollectionBase<IShapeMaterial>;
            entries.Clear();
            for (int k = 0; k < value.Length; k++)
            {
                entries.Add(value[k] as IShapeMaterial);
            }
            object retValue = base.SetItems(entries, value);
            return retValue;
        }
    } 
}
