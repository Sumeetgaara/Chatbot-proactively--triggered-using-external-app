using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExternalProgram.Models;

namespace ExternalProgram
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private async void Form1_LoadAsync(object sender, EventArgs e)
		{
			await TriggerAsync();

		}

		static async Task TriggerAsync()
		{
			string fromId;
			string fromName;
			string toId;
			string toName;
			string serviceUrl;
			string channelId;
			string conversationId;
			string date;
			string status;
			butterChicken.Models.Database3Entities DB = new butterChicken.Models.Database3Entities();
			var members = (from UserLog in DB.UserLogs
						   where UserLog.conID != string.Empty
						   select UserLog)
						.ToList();

			foreach (var member in members)
			{
				fromId = member.FrmID;
				fromName = member.FrmName;
				toId = member.toId;
				toName = member.toName;
				serviceUrl = member.svcURL;
				channelId = member.Channel;
				conversationId = member.conID;
				date = member.Date;
				status = member.Status;

				var userAccount = new ChannelAccount(toId, toName);
				var botAccount = new ChannelAccount(fromId, fromName);
				var connector = new ConnectorClient(new Uri(serviceUrl));

				IMessageActivity message = Activity.CreateMessageActivity();
				if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(channelId))
				{
					message.ChannelId = channelId;
				}
				else
				{
					conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
				}
				message.From = botAccount;
				message.Recipient = userAccount;
				message.Conversation = new ConversationAccount(id: conversationId);
				message.Text = "Delivery Date: " + date + ". Status:" + status;
				message.Locale = "en-Us";
				await connector.Conversations.SendToConversationAsync((Activity)message);
			}
		}
	}
}
