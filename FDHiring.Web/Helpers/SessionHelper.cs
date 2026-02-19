namespace FDHiring.Web.Helpers
{
    public static class SessionKeys
    {
        public const string UserId = "UserId";
        public const string UserFirstName = "UserFirstName";
        public const string UserLastName = "UserLastName";
        public const string CandidateId = "CandidateId";
    }

    public static class SessionExtensions
    {
        public static void SetUser(this ISession session, int id, string firstName, string lastName)
        {
            session.SetInt32(SessionKeys.UserId, id);
            session.SetString(SessionKeys.UserFirstName, firstName);
            session.SetString(SessionKeys.UserLastName, lastName);
        }

        public static int GetUserId(this ISession session)
        {
            return session.GetInt32(SessionKeys.UserId) ?? 0;
        }

        public static string GetUserFullName(this ISession session)
        {
            var first = session.GetString(SessionKeys.UserFirstName) ?? "";
            var last = session.GetString(SessionKeys.UserLastName) ?? "";
            return $"{first} {last}".Trim();
        }

        public static void SetCandidateId(this ISession session, int id)
        {
            session.SetInt32(SessionKeys.CandidateId, id);
        }

        public static int GetCandidateId(this ISession session)
        {
            return session.GetInt32(SessionKeys.CandidateId) ?? 0;
        }
    }
}