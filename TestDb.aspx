<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestDb.aspx.cs" Inherits="CampusApp.TestDb" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Diagnostik Koneksi MySQL</title>
    <style>
        body { font-family: 'Consolas', 'Courier New', monospace; padding: 20px;
               background: #1e1e1e; color: #d4d4d4; font-size: 13px; }
        .container { max-width: 1000px; margin: auto; background: #252526;
                     padding: 25px; border-radius: 8px; }
        h2 { color: #4ec9b0; margin-top: 0; }
        h3 { color: #dcdcaa; margin-top: 25px; border-bottom: 1px solid #3e3e42;
             padding-bottom: 5px; }
        .box { background: #1e1e1e; padding: 15px; border-left: 4px solid #569cd6;
               margin: 10px 0; border-radius: 4px; }
        .ok { border-left-color: #4ec9b0; color: #4ec9b0; }
        .fail { border-left-color: #f48771; color: #f48771; }
        .warn { border-left-color: #dcdcaa; color: #dcdcaa; }
        .label { color: #9cdcfe; }
        .value { color: #ce9178; word-break: break-all; }
        .btn { background: #0e639c; color: white; border: none; padding: 12px 24px;
               cursor: pointer; border-radius: 4px; font-size: 14px;
               font-family: 'Segoe UI', sans-serif; margin-right: 10px; }
        .btn:hover { background: #1177bb; }
        pre { background: #1e1e1e; padding: 12px; overflow-x: auto;
              border-radius: 4px; color: #ce9178; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>🔍 Diagnostik Koneksi MySQL — Database visual_ii</h2>
            
            <asp:Button ID="btnTest" runat="server" Text="▶ Jalankan Diagnostik"
                        CssClass="btn" OnClick="btnTest_Click" />
            <asp:Button ID="btnTestRaw" runat="server" Text="🔧 Test Koneksi Langsung (hardcoded)"
                        CssClass="btn" OnClick="btnTestRaw_Click" />
            
            <asp:Panel ID="pnlHasil" runat="server" />
        </div>
    </form>
</body>
</html>