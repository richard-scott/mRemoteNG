// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using AxWFICALib;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using AxMSTSCLib;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using mRemoteNG.Tools;
// End of VB project level imports

//using mRemoteNG.App.Runtime;
//using mRemoteNG.Tools.LocalizedAttributes;


namespace mRemoteNG.Connection.Protocol
{
	public class HTTPBase : Base
	{
				
#region Private Properties
		private Control wBrowser;
		public string httpOrS;
		public int defaultPort;
		private string tabTitle;
#endregion
				
#region Public Methods
		public HTTPBase(RenderingEngine RenderingEngine)
		{
			try
			{
				if (RenderingEngine == RenderingEngine.Gecko)
				{
					this.Control = new MiniGeckoBrowser.MiniGeckoBrowser();
					(this.Control as MiniGeckoBrowser.MiniGeckoBrowser).XULrunnerPath = System.Convert.ToString(My.Settings.Default.XULRunnerPath);
				}
				else
				{
					this.Control = new WebBrowser();
				}
						
				NewExtended();
			}
			catch (Exception ex)
			{
				MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg, My.Language.strHttpConnectionFailed + Constants.vbNewLine + ex.Message, true);
			}
		}
				
		public virtual void NewExtended()
		{
		}
				
		public override bool SetProps()
		{
			base.SetProps();
					
			try
			{
				Crownwood.Magic.Controls.TabPage objTabPage = this.InterfaceControl.Parent as Crownwood.Magic.Controls.TabPage;
				this.tabTitle = objTabPage.Title;
			}
			catch (Exception)
			{
				this.tabTitle = "";
			}
					
			try
			{
				this.wBrowser = this.Control;
						
				if (InterfaceControl.Info.RenderingEngine == RenderingEngine.Gecko)
				{
					MiniGeckoBrowser.MiniGeckoBrowser objMiniGeckoBrowser = wBrowser as MiniGeckoBrowser.MiniGeckoBrowser;
							
					objMiniGeckoBrowser.TitleChanged += wBrowser_DocumentTitleChanged;
					objMiniGeckoBrowser.LastTabRemoved += wBrowser_LastTabRemoved;
				}
				else
				{
					WebBrowser objWebBrowser = wBrowser as WebBrowser;
					SHDocVw.WebBrowser objAxWebBrowser = (SHDocVw.WebBrowser) objWebBrowser.ActiveXInstance;
							
					objWebBrowser.ScrollBarsEnabled = true;
							
					objWebBrowser.Navigated += wBrowser_Navigated;
					objWebBrowser.DocumentTitleChanged += wBrowser_DocumentTitleChanged;
					objAxWebBrowser.NewWindow3 += wBrowser_NewWindow3;
				}
						
				return true;
			}
			catch (Exception ex)
			{
				MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg, My.Language.strHttpSetPropsFailed + Constants.vbNewLine + ex.Message, true);
				return false;
			}
		}
				
		public override bool Connect()
		{
			try
			{
				string strHost = System.Convert.ToString(this.InterfaceControl.Info.Hostname);
				string strAuth = "";
						
				if (!((Force & Info.Force.NoCredentials) == (int) Info.Force.NoCredentials) && !string.IsNullOrEmpty(InterfaceControl.Info.Username) && !string.IsNullOrEmpty(InterfaceControl.Info.Password))
				{
					strAuth = "Authorization: Basic " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(this.InterfaceControl.Info.Username + ":" + this.InterfaceControl.Info.Password)) + Constants.vbNewLine;
				}
						
				if (this.InterfaceControl.Info.Port != defaultPort)
				{
					if (strHost.EndsWith("/"))
					{
						strHost = strHost.Substring(0, strHost.Length - 1);
					}
							
					if (strHost.Contains(httpOrS + "://") == false)
					{
						strHost = httpOrS + "://" + strHost;
					}
							
					if (InterfaceControl.Info.RenderingEngine == RenderingEngine.Gecko)
					{
						(wBrowser as MiniGeckoBrowser.MiniGeckoBrowser).Navigate(strHost + ":" + this.InterfaceControl.Info.Port);
					}
					else
					{
						object temp_Flags = null;
						object temp_TargetFrameName = null;
						object null_object = null;
						(wBrowser as WebBrowser).Navigate(strHost + ":" + this.InterfaceControl.Info.Port, ref temp_Flags, ref temp_TargetFrameName, ref strAuth, ref null_object);
					}
				}
				else
				{
					if (strHost.Contains(httpOrS + "://") == false)
					{
						strHost = httpOrS + "://" + strHost;
					}
							
					if (InterfaceControl.Info.RenderingEngine == RenderingEngine.Gecko)
					{
						(wBrowser as MiniGeckoBrowser.MiniGeckoBrowser).Navigate(strHost);
					}
					else
					{
						object temp_Flags2 = null;
						object temp_TargetFrameName2 = null;
						object null_object2 = null;
						(wBrowser as WebBrowser).Navigate(strHost, ref temp_Flags2, ref temp_TargetFrameName2, ref strAuth, ref null_object2);
					}
				}
						
				base.Connect();
				return true;
			}
			catch (Exception ex)
			{
				MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg, My.Language.strHttpConnectFailed + Constants.vbNewLine + ex.Message, true);
				return false;
			}
		}
#endregion
				
#region Private Methods
#endregion
				
#region Events
		private void wBrowser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
		{
			WebBrowser objWebBrowser = wBrowser as WebBrowser;
			if (objWebBrowser == null)
			{
				return ;
			}
					
			// This can only be set once the WebBrowser control is shown, it will throw a COM exception otherwise.
			objWebBrowser.AllowWebBrowserDrop = false;
					
			objWebBrowser.Navigated -= wBrowser_Navigated;
		}
				
		private void wBrowser_NewWindow3(object ppDisp, ref bool Cancel, long dwFlags, string bstrUrlContext, string bstrUrl)
		{
			if (dwFlags & NWMF.NWMF_OVERRIDEKEY)
			{
				Cancel = false;
			}
			else
			{
				Cancel = true;
			}
		}
				
		private void wBrowser_LastTabRemoved(object sender)
		{
			this.Close();
		}
				
		private void wBrowser_DocumentTitleChanged(System.Object sender, System.EventArgs e)
		{
			try
			{
				Crownwood.Magic.Controls.TabPage tabP = default(Crownwood.Magic.Controls.TabPage);
				tabP = InterfaceControl.Parent as Crownwood.Magic.Controls.TabPage;
						
				if (tabP != null)
				{
					string shortTitle = "";
							
					if (this.InterfaceControl.Info.RenderingEngine == RenderingEngine.Gecko)
					{
						if ((wBrowser as MiniGeckoBrowser.MiniGeckoBrowser).Title.Length >= 30)
						{
							shortTitle = (wBrowser as MiniGeckoBrowser.MiniGeckoBrowser).Title.Substring(0, 29) + " ...";
						}
						else
						{
							shortTitle = (wBrowser as MiniGeckoBrowser.MiniGeckoBrowser).Title;
						}
					}
					else
					{
						if ((wBrowser as WebBrowser).DocumentTitle.Length >= 30)
						{
							shortTitle = (wBrowser as WebBrowser).DocumentTitle.Substring(0, 29) + " ...";
						}
						else
						{
							shortTitle = (wBrowser as WebBrowser).DocumentTitle;
						}
					}
							
					if (!string.IsNullOrEmpty(this.tabTitle))
					{
						tabP.Title = tabTitle + " - " + shortTitle;
					}
					else
					{
						tabP.Title = shortTitle;
					}
				}
			}
			catch (Exception ex)
			{
				MessageCollector.AddMessage(Messages.MessageClass.WarningMsg, My.Language.strHttpDocumentTileChangeFailed + Constants.vbNewLine + ex.Message, true);
			}
		}
#endregion
				
#region Enums
		public enum RenderingEngine
		{
            [LocalizedAttributes.LocalizedDescription("strHttpInternetExplorer")]
            IE = 1,
            [LocalizedAttributes.LocalizedDescription("strHttpGecko")]
            Gecko = 2
		}
				
		private enum NWMF
		{
			// ReSharper disable InconsistentNaming
			NWMF_UNLOADING = 0x1,
			NWMF_USERINITED = 0x2,
			NWMF_FIRST = 0x4,
			NWMF_OVERRIDEKEY = 0x8,
			NWMF_SHOWHELP = 0x10,
			NWMF_HTMLDIALOG = 0x20,
			NWMF_FROMDIALOGCHILD = 0x40,
			NWMF_USERREQUESTED = 0x80,
			NWMF_USERALLOWED = 0x100,
			NWMF_FORCEWINDOW = 0x10000,
			NWMF_FORCETAB = 0x20000,
			NWMF_SUGGESTWINDOW = 0x40000,
			NWMF_SUGGESTTAB = 0x80000,
			NWMF_INACTIVETAB = 0x100000
			// ReSharper restore InconsistentNaming
		}
#endregion
	}
}
