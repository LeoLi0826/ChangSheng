namespace ET
{
    public sealed partial class NumericConfigData
    {
        private NumericData m_NumericData;

        public NumericData NumericData
        {
            get
            {
                return m_NumericData ??= EventSystem.Instance.Invoke<CreateNumericDataInvoke, NumericData>(new CreateNumericDataInvoke
                {
                    ConfigData = ConfigData
                });
            }
        }
    }
}