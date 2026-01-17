namespace FinalProjectMisa.Core.MisaAttribute;

[AttributeUsage(AttributeTargets.Property)]
public class MISARequired : Attribute
{
    public string ErrorMsg { get; set; }
    public MISARequired(string fieldName)
    {
        ErrorMsg = $"Cần phải nhập thông tin {fieldName}.";
    }
}