//-----------------------------------------------------------------------
// <copyright file="EngineUciManager.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine.UCI
{
    using Maki.Model;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A class used to manage the UCI layer on top of the engine process.
    /// </summary>
    public class EngineUciManager
    {
        private readonly ILogger<EngineBridge> bridgeLogger;
        private readonly ChessEngineProcess engineProcess;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineUciManager"/> class.
        /// </summary>
        /// <param name="bridgeLogger">The logger for the parent <see cref="EngineBridge" />.</param>
        /// <param name="process">The underlying chess engine process.</param>
        public EngineUciManager(
            ILogger<EngineBridge> bridgeLogger,
            ChessEngineProcess process)
        {
            this.bridgeLogger = bridgeLogger;
            this.engineProcess = process;
        }

        /// <summary>
        /// Initializes the UCI interface mode of the chess engine.
        /// </summary>
        /// <returns>The UCI header.</returns>
        public string InitializeUci()
        {
            this.WaitUntilReady();
            this.engineProcess.PopOutputBuffer();
            this.engineProcess.WriteCommand("uci");
            this.engineProcess.WaitForText("uciok", 1000);
            return this.engineProcess.PopOutputBuffer();
        }

        /// <summary>
        /// Sets a UCI option.
        /// </summary>
        /// <param name="optionName">The name of the option to set.</param>
        /// <param name="optionValue">The value to set the option to.</param>
        public void SetOption(string optionName, string optionValue)
        {
            this.engineProcess.WriteCommand($"setoption name {optionName} value {optionValue}");
        }

        /// <summary>
        /// Sets the value of the engine's debug mode.
        /// </summary>
        /// <param name="debugModeOn">If true, debug mode will be turned on.</param>
        public void SetDebugMode(bool debugModeOn)
        {
            string debugValue = debugModeOn ? "on" : "off";
            this.engineProcess.WriteCommand($"debug {debugValue}");
        }

        /// <summary>
        /// Waits until the engine is ready by using the 'isready' UCI command.
        /// </summary>
        public void WaitUntilReady()
        {
            this.engineProcess.WriteCommand("isready");
            if (!this.engineProcess.PeekOutputBuffer().Contains("readyok"))
            {
                this.engineProcess.WaitForText("readyok", 10000);
            }
        }

        /// <summary>
        /// Stops evaluation. Useful if engine is currently in an boundless evaluation.
        /// </summary>
        public void StopEvaluation()
        {
            this.engineProcess.WriteCommand("stop");
        }

        /// <summary>
        /// Evaluates a position with stop parameters.
        /// </summary>
        /// <param name="evalReq">The eval request.</param>
        public void Evaluate(StartEvaluationRequest evalReq)
        {
            if (evalReq.EngineOptions != null)
            {
                foreach (var op in evalReq.EngineOptions)
                {
                    this.SetOption(op.Key, op.Value.ToUciValueString());
                }
            }

            var position = StartPositionExtrapolator.ExtrapolateStartingPosition(evalReq.Position);
            this.SetPosition(position.Fen);

            var goCmd = evalReq.ToGoString();
            this.bridgeLogger.LogInformation($"Using gocmd: '{goCmd}'.");
            this.engineProcess.WriteCommand(goCmd);

            if (evalReq?.CompletionCriteria?.Infinite ?? false)
            {
                // infinite
            }
            else
            {
                this.engineProcess.WaitForText("bestmove", int.MaxValue);
            }

            this.engineProcess.PopOutputBuffer();
        }

        private void TryStopAnyRunningWork()
        {
            this.engineProcess.WriteCommand("stop");
            this.engineProcess.PopErrorBuffer();
        }

        private void SetPosition(string fen)
        {
            string cmd = $"position fen {fen}";
            this.bridgeLogger.LogInformation($"Using position cmd: '{cmd}'.");
            this.TryStopAnyRunningWork();
            this.engineProcess.WriteCommand(cmd);
        }
    }
}