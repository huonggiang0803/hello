namespace FinalProjectMisa.Core.MisaAttribute;


    [AttributeUsage(AttributeTargets.Property)]
    public class MISAMaxLength : Attribute
    {
        public int Length { get; set; }
        public string ErrorMsg { get; set; }
        public MISAMaxLength(int length, string errorMsg)
        {
            Length = length;
            ErrorMsg = errorMsg;
        }
    }