<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DocSiteIndex.ascx.cs" Inherits="MathNet.Iridium.DocSite.Controls.DocSiteIndex" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:UpdatePanel ID="indexUpdatePanel" runat="server">
	<ContentTemplate>
		<div class="index_filter_top">
			<asp:CheckBox runat="server" ID="indexFilterCheckbox" Checked="True" AutoPostBack="True" AccessKey="F" Text="Filter Index" meta:resourcekey="indexFilterCheckboxResource1"
				ToolTip="When this box is checked the index is filtered by the specified keywords.  Unchecked, the entire index is displayed unfiltered." />
			<asp:UpdateProgress runat="server" ID="indexUpdateProgress" AssociatedUpdatePanelID="indexUpdatePanel">
				<ProgressTemplate>
					<div class="index_filter_progress"><asp:Localize runat="server" ID="pleaseWaitLocalize" Text='<%$ Resources:General,PleaseWait %>' /></div>
				</ProgressTemplate>
			</asp:UpdateProgress>
		</div>
		<div id="indexFilterDiv" class="index_filter_bottom" runat="server">
			<div class="index_input_container">
				<asp:TextBox runat="server" ID="indexFilterTextBox" CssClass="index_input" onfocus="this.select();" meta:resourcekey="indexFilterTextBoxResource1" />
			</div>
			<div class="index_button_container">
				<asp:ImageButton runat="server" SkinID="IndexFilterImageButton" ID="indexFilterButton" OnClick="indexFilterButton_Click" CausesValidation="False"
				                 ToolTip='<%$ Resources:General,DocSiteFilterButtonToolTip %>' meta:resourcekey="indexFilterButtonResource1" />
			</div>
		</div>
		<div runat="server" id="index_list_container">
			<asp:Repeater runat="server" ID="indexRepeater" EnableViewState="False">
				<ItemTemplate>
					<div class='<%# ((currentItemSelected = SelectedHelpFile.Equals(Eval("File") as string, StringComparison.Ordinal)) ? "index_item_selected" : "index_item") %>'>
						<a class="index_item_link" href='<%# (currentItemSelected) ? "#" : GetPostBackClientHyperlink(Eval("File") as string) %>' title='<%# Eval("Name") %>'>
              <%# Eval("Name") %>
						</a>
					</div>
				</ItemTemplate>
			</asp:Repeater>
		</div>
	</ContentTemplate>
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="indexFilterCheckbox" EventName="CheckedChanged" />
		<asp:AsyncPostBackTrigger ControlID="indexFilterButton" EventName="Click" />
	</Triggers>
</asp:UpdatePanel>