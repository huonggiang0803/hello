namespace FinalProjectMisa.Core.MisaAttribute;
[AttributeUsage(AttributeTargets.Class)]
public class MISATableName : Attribute
{
    public string TableName { get; set; }   
    public MISATableName(string name)
    {
        TableName = name;
    }
}