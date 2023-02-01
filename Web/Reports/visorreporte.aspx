<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="visorreporte.aspx.cs" Inherits="Simco.Web.Reportes.visorreporte" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:reportviewer id="ReportViewer" runat="server" font-names="Verdana" font-size="8pt"
            interactivedeviceinfos="(Collection)" waitmessagefont-names="Verdana" waitmessagefont-size="14pt"
            backcolor="#F7F7F7" bordercolor="#E9E9E9" borderstyle="Solid" borderwidth="1px"
            pagecountmode="Actual" showbackbutton="False" showfindcontrols="False" width="100%"
            showzoomcontrol="False" hyperlinktarget="_blank" Height="850px">
                <LocalReport EnableHyperlinks="True">
            </LocalReport>
        </rsweb:reportviewer>
    </div>
    </form>
</body>
</html>
