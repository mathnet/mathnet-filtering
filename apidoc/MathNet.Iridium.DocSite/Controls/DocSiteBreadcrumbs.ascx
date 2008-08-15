<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocSiteBreadcrumbs.ascx.cs" Inherits="MathNet.Iridium.DocSite.Controls.DocSiteBreadcrumbs" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<div id="docsite_breadcrumbs_content">
  <asp:UpdatePanel runat="server" ID="breadCrumbsUpdatePanel" ChildrenAsTriggers="true">
	  <ContentTemplate>
		  <asp:SiteMapPath runat="server" ID="breadCrumbs" SiteMapProvider="DocSiteContentsSiteMapProvider">
			  <CurrentNodeStyle CssClass="docsite_breadcrumb docsite_breadcrumbs_active" />
			  <NodeStyle CssClass="docsite_breadcrumb docsite_breadcrumbs_link" />
			  <RootNodeStyle CssClass="docsite_breadcrumb docsite_breadcrumbs_root" />
		  </asp:SiteMapPath>
	  </ContentTemplate>
  </asp:UpdatePanel>
</div>