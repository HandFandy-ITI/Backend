namespace OstaFandy.PL.General
{

    public static class UserType
    {

        public const string Admin = "Admin";
        public const string Customer = "Customer";
        public const string Handyman = "Handyman";
    }
    public static class AddressTypes
    {
        public const string Home = "Home";
        public const string Work = "Work";
        public const string Other = "Other";
    }

    public static class ServiceType
    {
        public const string Fixed = "Fixed";
        public const string Inspection = "Inspection";
    }

    public static class HandymenStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }

    public static class BookingStatus
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
    }

    public static class JobAssignmentsStatus
    {
        public const string Assigned = "Assigned";
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
    }

    public static class QuotesStatus
    {
        public const string Pending = "Pending";
        public const string Accepted = "Accepted";
        public const string Rejected = "Rejected";
    }

    public static class PaymentsMethod
    {
        public const string Cash = "Cash";
        public const string Stripe = "Stripe";//card
    }

    public static class PaymentsStatus
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Failed = "Failed";
        public const string Refunded = "Refunded";
    }

}
