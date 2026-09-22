<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="SuperAdmin.aspx.cs" Inherits="LoginRegisterV2.SuperAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-shell">
        <div class="card">
            <div class="dashboard-header">
                <div>
                    <span class="badge badge-role-super">👑 Super Admin</span>
                    <h2 class="mt-sm">Super Admin Dashboard</h2>
                    <p class="dashboard-user-line">Logged in as: <strong><asp:Label ID="lblUserEmail" runat="server" /></strong></p>
                </div>
            </div>

            <div class="divider"></div>

            <div class="btn-row">
                <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" OnClick="btnChangePassword_Click" CssClass="btn btn-outline" />
                <asp:Button ID="btnLogout" runat="server" Text="Log Out" OnClick="btnLogout_Click" CausesValidation="false" CssClass="btn btn-danger" />
            </div>
        </div>
    </div>
  </asp:Content>
