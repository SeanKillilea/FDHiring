namespace FDHiring.Web.Helpers
{
    public static class SessionKeys
    {
        public const string UserId = "UserId";
        public const string UserFirstName = "UserFirstName";
        public const string UserLastName = "UserLastName";
        public const string CandidateId = "CandidateId";
        public const string SearchName = "SearchName";
        public const string SearchPositionId = "SearchPositionId";
        public const string SearchCurrent = "SearchCurrent";
        public const string SearchActive = "SearchActive";
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

        public static void SetSearchState(this ISession session, string? name, int? positionId, bool current, bool active)
        {
            session.SetString(SessionKeys.SearchName, name ?? "");
            session.SetInt32(SessionKeys.SearchPositionId, positionId ?? 0);
            session.SetInt32(SessionKeys.SearchCurrent, current ? 1 : 0);
            session.SetInt32(SessionKeys.SearchActive, active ? 1 : 0);
        }

        public static (string name, int positionId, bool current, bool active) GetSearchState(this ISession session)
        {
            return (
                session.GetString(SessionKeys.SearchName) ?? "",
                session.GetInt32(SessionKeys.SearchPositionId) ?? 0,
                (session.GetInt32(SessionKeys.SearchCurrent) ?? 0) == 1,
                (session.GetInt32(SessionKeys.SearchActive) ?? 0) == 1
            );
        }
    }
}