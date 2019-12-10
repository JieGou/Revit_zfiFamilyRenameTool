using System;
using Xunit;

namespace VmTests
{
    using FluentAssertions;
    using Moq;
    using zfiFamilyRenameTool.Abstractions;
    using zfiFamilyRenameTool.ViewModel;

    public class Tests
    {
        private string Dest { get; set; }

        [Fact]
        public void PrefixTest()
        {
            var source = "source";
            var prefix = "TEST_";

            var renameable = GetRenameable(source);

            var optionsVm = new OptionsVm { Prefix = prefix };
            var action = optionsVm.Rename;
            action(renameable);
            renameable.Destination.Should().Be(prefix + source);
        }

        [Fact]
        public void SuffixTest()
        {
            var source = "source";
            var suffix = "_TEST";

            var renameable = GetRenameable(source);

            var optionsVm = new OptionsVm { Suffix = suffix };
            var action = optionsVm.Rename;
            action(renameable);
            renameable.Destination.Should().Be(source + suffix);
        }

        [Fact]
        public void PrefixSuffixTest()
        {
            var source = "source";
            var prefix = "TEST_";
            var suffix = "_TEST";

            var renameable = GetRenameable(source);

            var optionsVm = new OptionsVm { Prefix = prefix, Suffix = suffix };
            var action = optionsVm.Rename;
            action(renameable);
            renameable.Destination.Should().Be(prefix + source + suffix);
        }

        [Fact]
        public void StartWithTest()
        {
            var source = "XXXsource";
            var result = "source";
            var find = "XXX";

            var renameable = GetRenameable(source);

            var optionsVm = new OptionsVm { Find = find };
            optionsVm.OptionCheckedCmd.Execute(nameof(RenameOption.StartWith));
            var action = optionsVm.Rename;
            action(renameable);

            renameable.Destination.Should().Be(result);
        }

        [Fact]
        public void EndWithTest()
        {
            var source = "sourceXXX";
            var result = "source";
            var find = "XXX";

            var renameable = GetRenameable(source);

            var optionsVm = new OptionsVm { Find = find };
            optionsVm.OptionCheckedCmd.Execute(nameof(RenameOption.EndWith));
            var action = optionsVm.Rename;
            action(renameable);

            renameable.Destination.Should().Be(result);
        }

        [Fact]
        public void ContainsTest()
        {
            var source = "souXXXrce";
            var result = "source";
            var find = "XXX";

            var renameable = GetRenameable(source);

            var optionsVm = new OptionsVm { Find = find };
            optionsVm.OptionCheckedCmd.Execute(nameof(RenameOption.Contains));
            var action = optionsVm.Rename;
            action(renameable);

            renameable.Destination.Should().Be(result);
        }

        [Fact]
        public void MatchesWholeTest()
        {
            var source = "source";
            var result = "XXX";
            var find = "source";

            var renameable = GetRenameable(source);

            var optionsVm = new OptionsVm { Find = find, ReplaceValue = result };
            optionsVm.OptionCheckedCmd.Execute(nameof(RenameOption.MatchesWhole));
            var action = optionsVm.Rename;
            action(renameable);

            renameable.Destination.Should().Be(result);
        }

        [Fact]
        public void RegexTest()
        {
            var source = "sou1245rce";
            var result = "source";
            var find = @"\d{4}";

            var renameable = GetRenameable(source);

            var optionsVm = new OptionsVm { Find = find };
            optionsVm.OptionCheckedCmd.Execute(nameof(RenameOption.Regex));
            var action = optionsVm.Rename;
            action(renameable);

            renameable.Destination.Should().Be(result);
        }

        [Fact]
        public void CaseSensitiveTest()
        {
            var source = "souXxXrce";
            var result = "source";
            var find = "xxx";

            var renameable = GetRenameable(source);

            var optionsVm = new OptionsVm { Find = find, IsCaseSensitive = true };
            optionsVm.OptionCheckedCmd.Execute(nameof(RenameOption.Contains));
            var action = optionsVm.Rename;
            action(renameable);

            renameable.Destination.Should().Be(result);
        }

        [Fact]
        public void AllInTest()
        {
            var source = "souXxXrce";
            var result = "TEST_source_TEST";
            var find = "xxx";
            var prefix = "TEST_";
            var suffix = "_TEST";

            var renameable = GetRenameable(source);

            var optionsVm = new OptionsVm { Find = find, IsCaseSensitive = true, Prefix = prefix, Suffix = suffix };
            optionsVm.OptionCheckedCmd.Execute(nameof(RenameOption.Contains));
            var action = optionsVm.Rename;
            action(renameable);

            renameable.Destination.Should().Be(result);
        }

        private IRenameable GetRenameable(string source)
        {
            var mock = new Mock<IRenameable>();
            mock.Setup(x => x.Source).Returns(source);
            mock.Setup(x => x.Destination).Returns(() => Dest);
            mock.SetupSet(x => x.Destination).Callback(value => Dest = value);
            mock.Setup(x => x.CanRename()).Returns(true);
            return mock.Object;
        }
    }
}