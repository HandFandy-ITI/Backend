namespace OstaFandy.PL.Constants
{
  

    public enum IsActive
    {
        Active=1,
        Inactive = 0
    }

    public enum AddressType
    {
        Home=0,
        Work=1,
        Other=2
    }

    public enum IsDefault
    {
        Yes = 1,
        No = 0
    }

    public enum IsRead
    {
        Read = 1,
        Unread = 0
    }

    public enum TokenUsed
    {
        Used = 1,
        NotUsed = 0
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded
    }

    public enum PaymentMethod
    {
        Cash,
        Stripe
    }


}
