namespace ET
{
    [Invoke]
    public class CreateNumericDataInvokeHandler : AInvokeHandler<CreateNumericDataInvoke, NumericData>
    {
        public override NumericData Handle(CreateNumericDataInvoke args)
        {
            return args.ConfigData.CreateNumericData();
        }
    }
}