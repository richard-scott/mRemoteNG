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
// End of VB project level imports

using System.ComponentModel;
//using mRemoteNG.App.Runtime;


namespace mRemoteNG.Connection
{
	public class Icon : StringConverter
		{
			
			public static string[] Icons = new string[] {};
			
			public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
			{
				return new StandardValuesCollection(Icons);
			}
			
			public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
			{
				return true;
			}
			
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				return true;
			}
			
			public static System.Drawing.Icon FromString(string IconName)
			{
				try
				{
					string IconPath = (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\Icons\\" + IconName +".ico";
					
					if (System.IO.File.Exists(IconPath))
					{
						System.Drawing.Icon nI = new System.Drawing.Icon(IconPath);
						
						return nI;
					}
				}
				catch (Exception ex)
				{
					MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg, "Couldn\'t get Icon from String" + Constants.vbNewLine + ex.Message);
				}
				
				return null;
			}
		}
}
