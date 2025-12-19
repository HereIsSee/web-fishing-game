namespace Api.Models.Interpreter
{
    /// <summary>
    /// INTERPRETER PATTERN - Abstract Expression
    /// Base interface for all command expressions
    /// </summary>
    public interface IExpression
    {
        void Interpret(GameAdminContext context);
        string GetDescription();
    }
}
