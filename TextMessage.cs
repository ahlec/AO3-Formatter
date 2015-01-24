namespace AO3_Formatter
{
    public class TextMessage
    {
        public CellPhone Sender { get; set; }
        public CellPhone Recipient { get; set; }
        public TextMessageDirection Direction { get; set; }
        public int IndicatorLength { get; set; }
    }
}
