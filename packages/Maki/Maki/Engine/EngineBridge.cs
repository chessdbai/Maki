//-----------------------------------------------------------------------
// <copyright file="EngineBridge.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maki.Engine.UCI;
    using Maki.Model;
    using Maki.Model.Options;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// A class to execute engine commands.
    /// </summary>
    public class EngineBridge
    {
        private const string DefaultEngineName = "stockfish";
        private const string NumLinesOptionMultiPv = "MultiPV";
        private static readonly int DefaultThreadCount = Environment.ProcessorCount;

        private readonly ILogger<EngineBridge> logger;
        private readonly List<EngineOption> supportedOptions;
        private readonly string engineIdName;
        private readonly string engineIdAuthor;

        private readonly ChessEngineProcess engineProcess;
        private readonly EngineUciManager engineManager;
        private readonly Dictionary<int, EvaluatedLine> bestLines = new Dictionary<int, EvaluatedLine>();
        private readonly LineParser lineParser = new LineParser();
        private bool isEval = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineBridge"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public EngineBridge(
            ILogger<EngineBridge> logger)
        {
            string engineName = Environment.GetEnvironmentVariable("ENGINE_NAME") ?? DefaultEngineName;
            this.engineProcess = this.CreateProcessForEngine(engineName);
            this.engineProcess.OutputReceivedAction = this.HandleEngineOutputReceived;
            this.logger = logger;
            this.engineProcess.ErrorReceivedAction = (str) => this.ReceivedEngineError?.Invoke(this, str);
            this.engineManager = new EngineUciManager(this.logger, this.engineProcess);
            this.engineManager.WaitUntilReady();
            var uciHeader = this.engineManager.InitializeUci();
            (string idName, string idAuthor, List<EngineOption> options) = this.ParseUciHeader(uciHeader);
            this.engineIdName = idName;
            this.engineIdAuthor = idAuthor;
            this.supportedOptions = options;
        }

        /// <summary>
        /// An event that fires whenever output from the engine is received.
        /// </summary>
        public event EventHandler<EngineOutputSegment> ReceivedEvaluationUpdate;

        /// <summary>
        /// An event that fires whenever error output from the engine is received.
        /// </summary>
        public event EventHandler<string> ReceivedEngineError;

        /// <summary>
        /// Gets the current position fen.
        /// </summary>
        public string PositionFen { get; private set; }

        /// <summary>
        /// Gets the options supported by the engine.
        /// </summary>
        public List<EngineOption> SupportedOptions => this.supportedOptions;

        /// <summary>
        /// Gets the engine name.
        /// </summary>
        public string Name => this.engineIdName;

        /// <summary>
        /// Gets the engine author name.
        /// </summary>
        public string Author => this.engineIdAuthor;

        /// <summary>
        /// Sets the engine debug mode either on (true) or off (false.
        /// </summary>
        /// <param name="debugModeOn">Whether or not debug mode should be turned on.</param>
        public void SetDebugMode(bool debugModeOn) => this.engineManager.SetDebugMode(debugModeOn);

        /// <summary>
        /// Start the engine evaluation at the current position.
        /// </summary>
        /// <param name="startEvaluationRequest">The evaluation response.</param>
        /// <returns>The response object containing the results of the evaluation.</returns>
        public StartEvaluationResponse StartEvaluation(StartEvaluationRequest startEvaluationRequest)
        {
            this.logger.LogInformation($"Starting evaluation:");
            this.logger.LogInformation($"{JsonConvert.SerializeObject(startEvaluationRequest)}");
            this.isEval = true;
            this.bestLines.Clear();
            var position = StartPositionExtrapolator.ExtrapolateStartingPosition(startEvaluationRequest.Position);
            this.PositionFen = position.Fen;
            this.engineManager.Evaluate(startEvaluationRequest);
            this.isEval = false;
            return new StartEvaluationResponse()
            {
                Lines = this.bestLines,
            };
        }

        /// <summary>
        /// Stops the engine evaluation at the current position.
        /// </summary>
        public void StopEvaluation()
        {
            this.engineManager.StopEvaluation();
        }

        private ChessEngineProcess CreateProcessForEngine(string engineFile)
        {
            return new ChessEngineProcess(engineFile);
        }

        private (string Name, string Author, List<EngineOption> Options) ParseUciHeader(string uciHeader)
        {
            string idName = null;
            string idAuthor = null;
            var options = new List<EngineOption>();
            var lines = uciHeader.Split('\n');
            foreach (var l in lines)
            {
                if (l.StartsWith("id name"))
                {
                    idName = string.Join(' ', l.Split(' ').Skip(2));
                }
                else if (l.StartsWith("id author"))
                {
                    idAuthor = string.Join(' ', l.Split(' ').Skip(2));
                }
                else if (l.StartsWith("option"))
                {
                    options.Add(OptionParser.ParseOptionLine(l));
                }
            }

            return (idName, idAuthor, options);
        }

        private void HandleEngineOutputReceived(string outputText)
        {
            if (outputText == null || !this.isEval)
            {
                return;
            }

            EvaluatedLine line = null;
            lock (this.bestLines)
            {
                if (this.PositionFen == null)
                {
                    throw new ArgumentException($"Cannot handle engine output because PositionFen is null.");
                }

                line = this.lineParser.ParseLineText(this.PositionFen, outputText);

                if (line == null)
                {
                    return;
                }

                bool containsUpdate = false;
                if (!this.bestLines.ContainsKey(line.LineRank))
                {
                    this.bestLines.Add(line.LineRank, line);
                    containsUpdate = true;
                }
                else if (line.LineSan.Split().Length >= this.bestLines[line.LineRank].LineSan.Split().Length)
                {
                    this.bestLines[line.LineRank] = line;
                    containsUpdate = true;
                }

                if (!containsUpdate)
                {
                    return;
                }
            }

            var segment = new EngineOutputSegment()
            {
                Output = outputText,
                Line = line,
                BestLines = this.bestLines,
            };

            this.ReceivedEvaluationUpdate?.Invoke(this, segment);
        }
    }
}