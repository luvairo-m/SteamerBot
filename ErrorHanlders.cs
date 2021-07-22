using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using System;
using System.Threading.Tasks;

namespace Steamer
{
    internal static class ErrorHanlder
    {
        public static async Task CmdErroredHandler(CommandsNextExtension _, CommandErrorEventArgs e)
        {
            try
            {
                var failedChecks = ((ChecksFailedException)e.Exception).FailedChecks;
                foreach (var failedCheck in failedChecks)
                {
                    if (failedCheck is CooldownAttribute)
                    {
                        CooldownAttribute attr = failedCheck as CooldownAttribute;
                        await e.Context.Channel.SendMessageAsync(
                            $"Команда на перезарядке: `{Math.Round(attr.GetRemainingCooldown(e.Context).TotalSeconds, 2)}` сек."
                        ).ConfigureAwait(false);
                    }
                }
            } catch (InvalidCastException)
            {
                Console.WriteLine(e.Exception.StackTrace);
            }
        }
    }
}

