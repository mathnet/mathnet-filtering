using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
namespace Netron.Diagramming.Core
{
    #region ConnectorAttachmentEventArgs
    /// <summary>
    /// Event argument on connecting a connection to a connector
    /// </summary>
    public sealed class ConnectorAttachmentEventArgs : EventArgs
    {
        /// <summary>
        /// the child connector
        /// </summary>
        IConnector child;
        /// <summary>
        /// the parent connector
        /// </summary>
        IConnector parent;
        /// <summary>
        /// the actual connection
        /// </summary>
        IConnection connection;


        /// <summary>
        /// Gets the child connector.
        /// </summary>
        /// <value>The child.</value>
        public IConnector Child
        {
            get
            {
                return child;
            }
        }

        /// <summary>
        /// Gets the parent connector.
        /// </summary>
        /// <value>The parent.</value>
        public IConnector Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public IConnection Connection
        {
            get
            {
                return connection;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ConnectorAttachmentEventArgs"/> class.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="connection">The connection.</param>
        public ConnectorAttachmentEventArgs(IConnector child, IConnector parent, IConnection connection)
        {
            this.child = child;
            this.parent = parent;
            this.connection = connection;
        }
    }
    #endregion

    #region EntityMenuEventArgs
    /// <summary>
    /// Event argument on creation of the contextmenu f an entity
    /// </summary>
    public sealed class EntityMenuEventArgs : EventArgs
    {
        #region Fields
        /// <summary>
        /// the entity
        /// </summary>
        private IDiagramEntity entity;
        /// <summary>
        /// the mouse event argument containing location etc.
        /// </summary>
        private MouseEventArgs e;
        /// <summary>
        /// the additional menu items to add
        /// </summary>
        private MenuItem[] additionalItems;
        #endregion;

        #region Properties
        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public IDiagramEntity Entity
        {
            get
            {
                return entity;
            }
        }

        /// <summary>
        /// Gets the mouse event args.
        /// </summary>
        /// <value>The mouse event args.</value>
        public MouseEventArgs MouseEventArgs
        {
            get
            {
                return e;
            }
        }

        /// <summary>
        /// Gets the additional items.
        /// </summary>
        /// <value>The additional items.</value>
        public MenuItem[] AdditionalItems
        {
            get
            {
                return additionalItems;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EntityMenuEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="additionalItems">The additional items.</param>
        public EntityMenuEventArgs(IDiagramEntity entity, MouseEventArgs e, ref MenuItem[] additionalItems)
        {
            this.entity = entity;
            this.e = e;
            this.additionalItems = additionalItems;
        }
        #endregion

    }
    #endregion

    #region CursorEventArgs
    /// <summary>
    /// Cursor event argument
    /// </summary>
    public sealed class CursorEventArgs : EventArgs
    {
        /// <summary>
        /// the Empty argument
        /// </summary>
        public static readonly new CursorEventArgs Empty = new CursorEventArgs();
        /// <summary>
        /// the Tool field
        /// </summary>
        private Cursor mCursor;
        /// <summary>
        /// Gets or sets the Cursor
        /// </summary>
        public Cursor Cursor
        {
            get
            {
                return mCursor;
            }
            set
            {
                mCursor = value;
            }
        }



        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public CursorEventArgs(Cursor cursor)
        {
            this.mCursor = cursor;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CursorEventArgs"/> class.
        /// </summary>
        public CursorEventArgs()
        {
        }
        #endregion

    }
    #endregion


    #region CursorEventArgs
    /// <summary>
    /// Cursor event argument
    /// </summary>
    public sealed class ColorEventArgs : EventArgs
    {
        /// <summary>
        /// the Empty argument
        /// </summary>
        public static readonly new CursorEventArgs Empty = new CursorEventArgs();
        /// <summary>
        /// the Color field
        /// </summary>
        private Color mColor;
        /// <summary>
        /// Gets or sets the Color
        /// </summary>
        public Color Color
        {
            get
            {
                return mColor;
            }
            set
            {
                mColor = value;
            }
        }

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public ColorEventArgs(Color color)
        {
            this.mColor = color;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CursorEventArgs"/> class.
        /// </summary>
        public ColorEventArgs()
        {
        }
        #endregion

    }
    #endregion


    #region ToolEventArgs
    /// <summary>
    /// Tool event argument
    /// </summary>
    public sealed class ToolEventArgs : EventArgs
    {

        /// <summary>
        /// the Tool field
        /// </summary>
        private ITool mTool;
        /// <summary>
        /// Gets or sets the Properties
        /// </summary>
        public ITool Properties
        {
            get
            {
                return mTool;
            }
            set
            {
                mTool = value;
            }
        }
        /// <summary>
        /// The empty argument.
        /// </summary>
        public static readonly new ToolEventArgs Empty = new ToolEventArgs();

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public ToolEventArgs(ITool tool)
        {
            this.mTool = tool;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ToolEventArgs"/> class.
        /// </summary>
        public ToolEventArgs()
        {
        }
        #endregion

    }
    #endregion

    #region PropertiesEventArgs
    /// <summary>
    /// Properties event argument
    /// </summary>
    public sealed class PropertiesEventArgs : EventArgs
    {

        /// <summary>
        /// the Properties field
        /// </summary>
        private Document mProperties;
        /// <summary>
        /// Gets or sets the Properties
        /// </summary>
        public Document Properties
        {
            get
            {
                return mProperties;
            }
            set
            {
                mProperties = value;
            }
        }
        /// <summary>
        /// The empty argument.
        /// </summary>
        public static readonly new PropertiesEventArgs Empty = new PropertiesEventArgs();

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public PropertiesEventArgs(Document document)
        {
            this.mProperties = document;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PropertiesEventArgs"/> class.
        /// </summary>
        public PropertiesEventArgs()
        {
        }
        #endregion

    }
    #endregion

    #region AmbienceEventArgs
    /// <summary>
    /// Ambience event argument
    /// </summary>
    public sealed class AmbienceEventArgs : EventArgs
    {


        /// <summary>
        /// the Ambience field
        /// </summary>
        private Ambience mAmbience;
        /// <summary>
        /// Gets or sets the Ambience
        /// </summary>
        public Ambience Ambience
        {
            get
            {
                return mAmbience;
            }
            set
            {
                mAmbience = value;
            }
        }

        /// <summary>
        /// The Empty event argument
        /// </summary>
        public static readonly new AmbienceEventArgs Empty = new AmbienceEventArgs();


        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public AmbienceEventArgs()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AmbienceEventArgs"/> class.
        /// </summary>
        /// <param name="ambience">The ambience.</param>
        public AmbienceEventArgs(Ambience ambience)
        {
            this.mAmbience = ambience;
        }
        #endregion

    }
    #endregion

    #region ConnectionCollectionEventArgs
    /// <summary>
    /// ConnectionCollection event argument
    /// </summary>
    public sealed class ConnectionCollectionEventArgs : EventArgs
    {

        /// <summary>
        /// the Connection field
        /// </summary>
        private IConnection mConnection;
        /// <summary>
        /// Gets or sets the Connection
        /// </summary>
        public IConnection Connection
        {
            get
            {
                return mConnection;
            }
            set
            {
                mConnection = value;
            }
        }

        /// <summary>
        /// The Empty event argument
        /// </summary>
        public static readonly new ConnectionCollectionEventArgs Empty = new ConnectionCollectionEventArgs();


        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public ConnectionCollectionEventArgs()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ConnectionCollectionEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public ConnectionCollectionEventArgs(IConnection connection)
        {
            this.mConnection = connection;
        }
        #endregion

    }
    #endregion

    #region DiagramInformationEventArgs
    /// <summary>
    /// Event argument on passing diagram information (metdata)
    /// </summary>
    public sealed class DiagramInformationEventArgs : EventArgs
    {

        /// <summary>
        /// the Information field
        /// </summary>
        private DocumentInformation mInformation;
        /// <summary>
        /// Gets or sets the Information
        /// </summary>
        public DocumentInformation Information
        {
            get
            {
                return mInformation;
            }
            set
            {
                mInformation = value;
            }
        }

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        private DiagramInformationEventArgs()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DiagramInformationEventArgs"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        public DiagramInformationEventArgs(DocumentInformation info)
        {
            this.mInformation = info;
        }
        #endregion
        /// <summary>
        /// The Empty event argument
        /// </summary>
        public static readonly new DiagramInformationEventArgs Empty = new DiagramInformationEventArgs();
    }
    #endregion

    #region CollectionEventArgs
    /// <summary>
    /// Event argument to pass <see cref="CollectionBase"/> information via events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionEventArgs<T> : EventArgs
    {
        private T item;

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public T Item
        {
            get { return item; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectionEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="item">A parameter of the generics Type T</param>
        public CollectionEventArgs(T item)
        {
            this.item = item;
        }
    }
    #endregion

    #region RectangleEventArgs
    /// <summary>
    /// Event argument to pass <see cref="Rectangle"/> information via events
    /// </summary>
    public sealed class RectangleEventArgs : EventArgs
    {
        /// <summary>
        /// the rectangle
        /// </summary>
        private Rectangle mRectangle;

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        public Rectangle Rectangle
        {
            get
            {
                return mRectangle;
            }
        }
        /// <summary>
        /// The Empty event argument
        /// </summary>
        public static readonly new RectangleEventArgs Empty = new RectangleEventArgs();


        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public RectangleEventArgs(Rectangle rectangle)
        {
            this.mRectangle = rectangle;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:RectangleEventArgs"/> class.
        /// </summary>
        private RectangleEventArgs()
        {
        }
        #endregion

    }
    #endregion

    #region EntityEventArgs
    /// <summary>
    /// Event argument carrying an item
    /// </summary>
    public sealed class EntityEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the entity
        /// </summary>
        IDiagramEntity mEntity;

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public IDiagramEntity Entity
        {
            get
            {
                return mEntity;
            }
            set
            {
                mEntity = value;
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="entity"></param>
        public EntityEventArgs(IDiagramEntity entity)
        {
            this.Entity = entity;
        }
    }
    #endregion

    #region ConnectorsEventArgs
    /// <summary>
    /// Event argument carrying two connectors
    /// </summary>
    public sealed class ConnectorsEventArgs : EventArgs
    {
        IConnector mConnectorSubject;
        IConnector mConnectorObject;

        public IConnector SubjectConnector
        {
            get { return mConnectorSubject; }
            set { mConnectorSubject = value; }
        }

        public IConnector ObjectConnector
        {
            get { return mConnectorObject; }
            set { mConnectorObject = value; }
        }

        public ConnectorsEventArgs(IConnector subjectConnector, IConnector objectConnector)
        {
            mConnectorSubject = subjectConnector;
            mConnectorObject = objectConnector;
        }
    }
    #endregion

    #region EntityMouseEventArgs
    /// <summary>
    /// MouseEvent argument carrying an item
    /// </summary>
    public sealed class EntityMouseEventArgs : MouseEventArgs
    {


        /// <summary>
        /// Gets or sets the entity
        /// </summary>
        IDiagramEntity mEntity;

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public IDiagramEntity Entity
        {
            get
            {
                return mEntity;
            }
            set
            {
                mEntity = value;
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="button">The button.</param>
        /// <param name="clicks">The clicks.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="delta">The delta.</param>
        public EntityMouseEventArgs(IDiagramEntity entity, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            this.mEntity = entity;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EntityMouseEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public EntityMouseEventArgs(IDiagramEntity entity, MouseEventArgs e)
            : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
            if(e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            this.mEntity = entity;
        }
    }
    #endregion

    #region StringEventArgs
    /// <summary>
    /// Contains a string event argument
    /// </summary>
    public sealed class StringEventArgs : EventArgs
    {

        string mData;
        /// <summary>
        /// Gets or sets the string data 
        /// </summary>
        public string Data
        {
            get
            {
                return mData;
            }

        }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data"></param>
        public StringEventArgs(string data)
        {
            this.mData = data;
        }
    }
    #endregion

    #region SingleDataEventArgs
    /// <summary>
    /// A single-bucket data transfer event argument
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SingleDataEventArgs<T> : EventArgs
    {
        /// <summary>
        /// whatever data
        /// </summary>
        T mData;

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public T Data
        {
            get
            {
                return mData;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SingleDataEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="data">A parameter of the generics Type T</param>
        public SingleDataEventArgs(T data)
        {
            //if (data is default(T))
            //    throw new ArgumentNullException("The argument does not contain any data.");

            this.mData = data;
        }
    }

    #endregion

    #region HistoryChangeEventArgs

    /// <summary>
    /// Event argument to communicate history changes in the undo/redo mechanism
    /// </summary>
    public sealed class HistoryChangeEventArgs : EventArgs
    {
        /// <summary>
        /// the RedoText field
        /// </summary>
        private string mRedoText;
        /// <summary>
        /// Gets or sets the RedoText
        /// </summary>
        public string RedoText
        {
            get
            {
                return mRedoText;
            }
            set
            {
                mRedoText = value;
            }
        }

        /// <summary>
        /// the UndoText field
        /// </summary>
        private string mUndoText;
        /// <summary>
        /// Gets or sets the UndoText
        /// </summary>
        public string UndoText
        {
            get
            {
                return mUndoText;
            }
            set
            {
                mUndoText = value;
            }
        }

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public HistoryChangeEventArgs(string redoText, string undoText)
        {
            this.mRedoText = redoText;
            this.mUndoText = undoText;
        }

        #endregion

    }
    #endregion

    #region SelectionEventArgs
    /// <summary>
    /// Properties event argument
    /// </summary>
    public sealed class SelectionEventArgs : EventArgs
    {
        /// <summary>
        /// the Properties field
        /// </summary>
        private object[] mObjects;
        /// <summary>
        /// Gets or sets the selected objects
        /// </summary>
        public object[] SelectedObjects
        {
            get
            {
                return mObjects;
            }
            set
            {
                mObjects = value;
            }
        }
        /// <summary>
        /// The empty argument.
        /// </summary>
        public static readonly new PropertiesEventArgs Empty = new PropertiesEventArgs();

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public SelectionEventArgs(object[] objects)
        {
            this.mObjects = objects;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SelectionEventArgs"/> class.
        /// </summary>
        public SelectionEventArgs()
        {
        }
        #endregion

    }
    #endregion

    #region CancelableEntityEventArgs
    /// <summary>
    /// Event argument carrying an item
    /// </summary>
    public sealed class CancelableEntityEventArgs : EventArgs
    {

        /// <summary>
        /// the Cancel field
        /// </summary>
        private bool mCancel;
        /// <summary>
        /// Gets or sets the Cancel
        /// </summary>
        public bool Cancel
        {
            get
            {
                return mCancel;
            }
            set
            {
                mCancel = value;
            }
        }


        /// <summary>
        /// Gets or sets the entity
        /// </summary>
        IDiagramEntity mEntity;

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public IDiagramEntity Entity
        {
            get
            {
                return mEntity;
            }
            set
            {
                mEntity = value;
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="entity"></param>
        public CancelableEntityEventArgs(IDiagramEntity entity)
        {
            this.Entity = entity;
        }
    }
    #endregion

    #region FileEventArgs
    /// <summary>
    /// Contains a string event argument
    /// </summary>
    public sealed class FileEventArgs : EventArgs
    {


        /// <summary>
        /// the File field
        /// </summary>
        private FileInfo mFile;
        /// <summary>
        /// Gets or sets the file information
        /// </summary>
        public FileInfo File
        {
            get
            {
                return mFile;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="info">The info.</param>
        public FileEventArgs(FileInfo info)
        {
            this.mFile = info;
        }
    }
    #endregion
}
