namespace CashManager.WPF.Messages.App
{
    internal class ApplicationUpdateMessage
    {
        internal string Content { get; }

        internal string Tooltip { get; }

        internal ApplicationUpdateMessage(string content, string tooltip)
        {
            Content = content;
            Tooltip = tooltip;
        }
    }
}