using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.12.31616")]
internal sealed class _003CRegexGenerator_g_003EF11660F4C4287BA7818937A1327E7BF54DE483EC4FD6485CA7D07F3CF6DB3AE81__SpecialCharRegex_3 : Regex
{
    private sealed class RunnerFactory : RegexRunnerFactory
    {
        private sealed class Runner : RegexRunner
        {
            protected override void Scan(ReadOnlySpan<char> inputSpan)
            {
                if (TryFindNextPossibleStartingPosition(inputSpan))
                {
                    int num = runtextpos;
                    Capture(0, num, runtextpos = num + 1);
                }
            }

            private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
            {
                int num = runtextpos;
                if ((uint)num < (uint)inputSpan.Length)
                {
                    int num2 = inputSpan.Slice(num).IndexOfAnyExcept(_003CRegexGenerator_g_003EF11660F4C4287BA7818937A1327E7BF54DE483EC4FD6485CA7D07F3CF6DB3AE81__Utilities.s_ascii_FF03FEFFFF8700000000);
                    if (num2 >= 0)
                    {
                        runtextpos = num + num2;
                        return true;
                    }
                }
                runtextpos = inputSpan.Length;
                return false;
            }
        }

        protected override RegexRunner CreateInstance()
        {
            return new Runner();
        }
    }

    internal static readonly _003CRegexGenerator_g_003EF11660F4C4287BA7818937A1327E7BF54DE483EC4FD6485CA7D07F3CF6DB3AE81__SpecialCharRegex_3 Instance = new _003CRegexGenerator_g_003EF11660F4C4287BA7818937A1327E7BF54DE483EC4FD6485CA7D07F3CF6DB3AE81__SpecialCharRegex_3();

    private _003CRegexGenerator_g_003EF11660F4C4287BA7818937A1327E7BF54DE483EC4FD6485CA7D07F3CF6DB3AE81__SpecialCharRegex_3()
    {
        pattern = "[^A-Z0-9_]";
        roptions = RegexOptions.None;
        Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EF11660F4C4287BA7818937A1327E7BF54DE483EC4FD6485CA7D07F3CF6DB3AE81__Utilities.s_defaultTimeout);
        internalMatchTimeout = _003CRegexGenerator_g_003EF11660F4C4287BA7818937A1327E7BF54DE483EC4FD6485CA7D07F3CF6DB3AE81__Utilities.s_defaultTimeout;
        factory = new RunnerFactory();
        capsize = 1;
    }
}
