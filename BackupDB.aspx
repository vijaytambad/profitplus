<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BackupDB.aspx.cs" Inherits="Backup_BackupDB" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <style type="text/css">
        .style2
        {
            width: 216px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td align="center">
                    <table cellpadding="0" cellspacing="0" border="0" width="50%">
                        <tr>
                            <td>
                                <h3>
                                    <b>Backup and Restore the Database from SQL Server</b></h3>
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 20px;">
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <table cellpadding="0" cellspacing="0" border="0" width="70%">
                                    <tr>
                                        <td align="right">
                                            <b>Select Database:</b>
                                        </td>
                                        <td align="left">
                                            &nbsp;&nbsp;<asp:DropDownList ID="ddlDatabases" runat="server" AutoPostBack="false"
                                                Height="23px" Width="197px">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left">
                                            <asp:Button ID="btnBackup" runat="server" Text="Backup..." OnClick="btnBackup_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 20px;">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblMessage" ForeColor="Red" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 20px;">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Backup Databases List:</b>
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 10px;">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:ListBox ID="lstBackupfiles" runat="server" Height="236px" Width="354px"></asp:ListBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 10px;">
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnRestore" runat="server" Text="Restore..." 
                                    onclick="btnRestore_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 10px;">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
