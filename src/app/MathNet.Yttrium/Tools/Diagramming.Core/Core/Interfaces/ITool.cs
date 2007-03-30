using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
/*    
    http://www.theserverside.net/blogs/showblog.tss?id=pluginArchitectures
    http://www.martinfowler.com/articles/injection.html
    http://www.codeproject.com/csharp/components.asp
 */
    /// <summary>
    /// Describes a tool's members. <seealso cref="AbstractTool"/>
    /// </summary>
    public interface ITool : IServiceProvider
    {

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:ITool"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the tool.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this tool is active. If true the tool is actually performing work via the various mouse or keyboard event handlers.
        /// If <see cref="Enabled"/> is false the tool can never be active. Furthermore, if the tool <see cref="IsSuspended"/> it means another tool has suspended the activity of this tool.
        /// 
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether this tool can activated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can activate; otherwise, <c>false</c>.
        /// </value>
        bool CanActivate { get; }

        /// <summary>
        /// Deactivates the tool.
        /// </summary>
        /// <returns></returns>
        bool DeactivateTool();

        /// <summary>
        /// Activates the tool.
        /// </summary>
        /// <returns></returns>
        bool ActivateTool();

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>The controller.</value>
        IController Controller { get; set;}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is suspended. A tool enters in a suspended mode when another tool has been activated and disallows another to continue its normal activity. For example, the <see cref="MoveTool"/> and <see cref="SelectionTool"/> are 
        /// mutually exclusive and similarly for the drawing tools and the selection tool. 
        /// <para>This suspended state is independent of the <see cref="IsActive"/> and the <see cref="Enabled"/> states.</para>
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is suspended; otherwise, <c>false</c>.
        /// </value>
        bool IsSuspended { get; set;}
    }

    
}
