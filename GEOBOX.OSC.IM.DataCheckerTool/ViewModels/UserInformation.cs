namespace GEOBOX.OSC.IM.DataCheckerTool.ViewModels
{
    public sealed class UserInformation
    {
        public string Caption { get; private set; }
        public string Message { get; private set; }
        public string UserInformationType { get; set; }

        public UserInformation(string caption, string message, string userInformationType)
        {
            Caption = caption;
            Message = message;
            UserInformationType = userInformationType;
        }

        public static string UserInformationTypeError => "ERROR";
        public static string UserInformationTypeInformation => "INFORMATION";

    }
}
