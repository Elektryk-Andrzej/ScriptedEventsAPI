namespace ScriptedEventsAPI.ConditionAPI.ConditionElements;

public class Clause : IConditionElement
{
    public required Operand FirstOperand;
    public required Operator Operator;
    public required Operand SecondOperand;
}