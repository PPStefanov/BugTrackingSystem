namespace BugTrackingSystem.GCommon
{
    public static class ValidationConstants
    {
        public static class BugReportValidation
        {
            public const int TitleMinLength = 5;
            public const int TitleMaxLength = 100;

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 1000;
        }

        public static class CommentValidation
        {
            public const int ContentMinLength = 3;
            public const int ContentMaxLength = 500;
        }

        public static class ApplicationNameValidation
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;
        }

        public static class AppUserValidation
        {
            public const int AppUserMinLength = 3;
            public const int AppUserMaxLength = 50;

            public const int PasswordMinLength = 6;
            public const int PasswordMaxLength = 100;
        }
    }
}
