namespace Api.Models.ChainOfResponsibility
{
    /// <summary>
    /// CHAIN OF RESPONSIBILITY PATTERN - Base Handler
    /// Abstract handler for catch attempt validation chain
    /// </summary>
    public abstract class CatchAttemptHandler
    {
        protected CatchAttemptHandler? _nextHandler;

        public CatchAttemptHandler SetNext(CatchAttemptHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual void Handle(CatchAttemptContext context)
        {
            if (context.IsValid && _nextHandler != null)
            {
                _nextHandler.Handle(context);
            }
        }

        protected abstract void ProcessRequest(CatchAttemptContext context);
    }
}
