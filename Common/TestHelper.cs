using System.Linq;
using System.Numerics;
using LunarParser;
using Neo.Emulation;
using Neo.VM;
using NUnit.Framework;

namespace Common
{
    public static class TestHelper
    {
        public const string Nep5ContractFilePath =
            "../../../../NEP5.Contract/bin/Release/netcoreapp2.0/publish/NEP5.Contract.avm";
        public const string CrowdsaleContractFilePath =
            "../../../../Crowdsale.Contract/bin/Release/netcoreapp2.0/publish/Crowdsale.Contract.avm";

        public static StackItem Execute(this Emulator emulator, string operation, params object[] args)
        {
            var inputs = DataNode.CreateArray();
            inputs.AddValue(operation);

            if (args.Length > 0)
            {
                var parameters = DataNode.CreateArray();
                foreach (var a in args)
                {
                    switch (a)
                    {
                        case byte[] bytes:
                            var bytesArray = DataNode.CreateArray();
                            bytes.Reverse().ToList().ForEach(b => bytesArray.AddValue(b));
                            parameters.AddNode(bytesArray);
                            break;
                        case int _:
                        case long _:
                        case BigInteger _:
                            var arr = DataNode.CreateArray();
                            arr.AddValue(a);
                            parameters.AddNode(arr);
                            break;
                        default:
                            parameters.AddValue(a);
                            break;
                    }
                }

                inputs.AddNode(parameters);
            }
            else
            {
                inputs.AddValue(null);
            }

            var script = emulator.GenerateLoaderScriptFromInputs(inputs, new ABI());
            emulator.Reset(script, null, null);
            emulator.Run();

            var result = emulator.GetOutput();

            Assert.NotNull(result);
            return result;
        }
    }
}