using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTutorials.Module
{
    public class SlashCommandModule : InteractionModuleBase<SocketInteractionContext>
    {
        private static bool spamMessage = false;

        [SlashCommand("hello", "Basic bot ping like Hello World")]
        public async Task Ping()
        {
            await RespondAsync("Hello " + this.Context.User.Mention + ". I am a bot!");
        }

        [SlashCommand("delete", "Delete number of messages or all if not provided anything")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteMessages([Summary(description: "Downloads and removes X messages from the current channel with max 100.")] int count = 0)
        {
            await RespondAsync("Message getting deleted");
            if(count < 0)
            {
                await RespondAsync("How can I delete negative number of messages. Please teach me sensei");
            }
            else if (count > 0)
            {
                await DeleteNMessage(count);
            }
            else
            {
                await DeleteAllMessages();
            }
        }

        [SlashCommand("startspam", "Start Message Spamming")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartMessageSpam([Summary(description: "Duration in seconds after which message will spam minimum is 5s")] int duration = 5)
        {
            if(duration < 5)
            {
                duration = 0;
            }
            await RespondAsync("Message spam start");
            spamMessage = true;
            while (spamMessage)
            {
                await MessageSpammer();
                await Task.Delay(TimeSpan.FromSeconds(duration));
            }
        }

        [SlashCommand("stopspam", "Stop Message Spamming")]
        public async Task StopMessageSpam()
        {
            spamMessage = false;
            await RespondAsync("Message spam stopped");
        }

        private async Task MessageSpammer()
        {
            await this.Context.Channel.SendMessageAsync("Message will spam now");
        }

        private async Task DeleteNMessage(int count)
        {
            if(count > 100)
            {
                count = 100;
            }
            var messages = await this.Context.Channel.GetMessagesAsync(count).FlattenAsync();
            await (this.Context.Channel as ITextChannel).DeleteMessagesAsync(messages);
        }

        private async Task DeleteAllMessages()
        {
            var messages = await this.Context.Channel.GetMessagesAsync(100).FlattenAsync();
            await (this.Context.Channel as ITextChannel).DeleteMessagesAsync(messages);
        }
    }
}
