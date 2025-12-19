using Microsoft.Extensions.Hosting;

namespace Api.Models.Interpreter
{
    /// <summary>
    /// INTERPRETER PATTERN - Console Command Interpreter Service
    /// Runs as a background service that reads console commands and interprets them
    /// </summary>
    public class ConsoleCommandInterpreterService : IHostedService
    {
        private readonly CommandParser _parser;
        private Task? _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new();

        public ConsoleCommandInterpreterService()
        {
            _parser = new CommandParser();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   CONSOLE COMMAND INTERPRETER STARTED (Interpreter Pattern) ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine("Type 'help' for available commands or 'exit' to quit\n");

            _executingTask = Task.Run(() => RunInterpreterLoop(_stoppingCts.Token), cancellationToken);
            return Task.CompletedTask;
        }

        private async Task RunInterpreterLoop(CancellationToken cancellationToken)
        {
            var context = new GameAdminContext(Session.Instance);
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Console.Write("\nGame> ");
                    var input = await Task.Run(() => Console.ReadLine(), cancellationToken);

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    if (input.Trim().ToLower() == "exit")
                    {
                        Console.WriteLine("👋 Exiting command interpreter...");
                        break;
                    }

                    var expression = _parser.Parse(input);
                    if (expression != null)
                    {
                        expression.Interpret(context);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }

            Console.WriteLine("\n🛑 Console Command Interpreter stopped");
        }
    }
}
