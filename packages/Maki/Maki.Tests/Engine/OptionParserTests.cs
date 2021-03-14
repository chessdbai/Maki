namespace Maki.Tests.Engine
{
    using Maki.Engine;
    using Maki.Model.Options;
    using Xunit;

    public class OptionParserTests
    {
        const string ComboOption =
            "option name Analysis Contempt type combo default Both var Off var White var Black var Both";

        private const string CheckBoxOption =
            "option name Syzygy50MoveRule type check default true";

        private const string SpinnerOption =
            "option name UCI_Elo type spin default 1350 min 1350 max 2850";

        private const string StringOption =
            "option name SyzygyPath type string default <empty>";

        [Fact(DisplayName = "Parses combo option with default value")]
        public void ParsesComboOptionWithDefault()
        {
            var option = OptionParser.ParseOptionLine(ComboOption);
            Assert.Equal(OptionType.MultipleChoice, option.Type);
            Assert.Equal("Analysis Contempt", option.Name);
            Assert.Equal("Both", option.DefaultValue.TextValue);
            Assert.Equal(4, option.ValidValues.Count);
            Assert.Equal("Off", option.ValidValues[0]);
            Assert.Equal("White", option.ValidValues[1]);
            Assert.Equal("Black", option.ValidValues[2]);
            Assert.Equal("Both", option.ValidValues[3]);
        }

        [Fact(DisplayName = "Parses CheckBox option with default value")]
        public void ParsesCheckBoxOption()
        {
            var option = OptionParser.ParseOptionLine(CheckBoxOption);
            Assert.Equal("Syzygy50MoveRule", option.Name);
            Assert.Equal(true, option.DefaultValue.BooleanValue!);
            Assert.Equal(OptionType.Boolean, option.Type);
        }

        [Fact(DisplayName = "Parses Spinner option with default value")]
        public void ParsesSpinnerOption()
        {
            var option = OptionParser.ParseOptionLine(SpinnerOption);
            Assert.Equal("UCI_Elo", option.Name);
            Assert.Equal(1350, option.Minimum!);
            Assert.Equal(2850, option.Maximum!);
            Assert.Equal(1350, option.DefaultValue.IntegerValue!);
            Assert.Equal(OptionType.NumberRange, option.Type);
        }

        [Fact(DisplayName = "Parses String option with empty default value")]
        public void ParsesStringOption()
        {
            var option = OptionParser.ParseOptionLine(StringOption);
            Assert.Equal("SyzygyPath", option.Name);
            Assert.Equal(string.Empty, option.DefaultValue.TextValue!);
            Assert.Equal(OptionType.Text, option.Type);
        }
    }
}